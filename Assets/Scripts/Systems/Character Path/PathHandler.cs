using Assets.Scripts.World.Npc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Systems.Character_Path
{
    class PathHandler : MonoBehaviour
    {
        private PathContainer container;
        // References the path that is currently set to be followed. Is null by default.
        private Path currentPath;
        
        public bool follow = false;
        private bool begin = false;
        private bool readyToStart = false;
        private bool followLoop = false;
        private bool followingBackwards = false;
        private bool reachedFirst = false;

        private int currentPointIndex = 0;
        private float t = 0f;
        private float playerMovementThreshold = 0f;

        private CharacterController controller;
        private Vector3 startPosition = Vector3.zero;

        // Entity that will follow a path.
        private Entity entity;

        private void Start()
        {
            entity = GetComponent<Entity>();
            if (!entity)
            {
                // Disable this component if no entity was found.

                Debug.LogError($"[Path system][{name}]: Entity component was not found.");
                enabled = false;
            }

            container = FindAnyObjectByType<PathContainer>();
            if (!container)
            {
                // Disable this component if no path container was found.

                Debug.LogError($"[Path system][{name}]: Path container was not found on the scene.");
                enabled = false;
            }
        }

        private void Update()
        {
            if (follow)
            {
                if (currentPath.points.Count >= 4)
                    MoveOverPathCurve();
                else if (currentPath.points.Count == 2)
                    MoveOverPathLine();
                else
                {
                    Debug.LogError($"[Path system][{name}]: Current path points quantity doesnt match the required value, operation aborted.");
                    StopFollowingPath();
                }
            }
        }

        public IEnumerator SetAndFollow(string path)
        {
            // Find the path:
            currentPath = container.paths.Find(p => p.name == path);
            if (currentPath == null)
            {
                Debug.LogError($"[Path Handler]: Path with name '{path}' could not be found on scene.");
                yield break;
            }

            entity.isFollowingPath = true;

            controller = entity.GetComponent<CharacterController>();
            if (controller != null)
                controller.enabled = false;

            Vector3 direction = currentPath.points[0].transform.position - entity.transform.position;
            yield return TurnTowardsFirstPathPoint(Quaternion.LookRotation(direction), 1.0f);

            StartFollowingPath(false);
        }

        private IEnumerator TurnTowardsFirstPathPoint(Quaternion target, float duration)
        {
            Quaternion start = entity.transform.rotation;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float percent = elapsed / duration; // Always goes 0.0 to 1.0

                // This stays smooth even at very slow speeds
                entity.transform.rotation = Quaternion.Slerp(start, target, percent);
                yield return null;
            }

            entity.transform.rotation = target; // Final snap for 100% precision
        }

        private void MoveOverPathLine()
        {
            PlayMotionAnimations(entity.isPlayer());

            Vector3 currentPointPosition = currentPath.points[currentPointIndex].transform.position;
            entity.transform.position = Vector3.MoveTowards(entity.transform.position, currentPointPosition, entity.walkSpeed * Time.deltaTime);

            Vector3 lookDirection = (currentPointPosition - entity.transform.position).normalized;
            lookDirection.y = 0;
            entity.transform.rotation = Quaternion.Slerp(entity.transform.rotation, Quaternion.LookRotation(lookDirection), Time.deltaTime * 5f);

            if (Vector3.Distance(entity.transform.position, currentPointPosition) == 0)
            {
                if (currentPointIndex >= currentPath.points.Count - 1)
                    StopFollowingPath();
                else
                    currentPointIndex++;
            }
        }
        private void MoveOverPathCurve()
        {
            // The path requires at least 4 path points to work;
            if (currentPath.points.Count < 4)
                return;

            // Stop if target entity is already on the last path point.
            if (currentPointIndex >= currentPath.points.Count - 1)
            {
                StopFollowingPath();
                return;
            }

            if (!reachedFirst)
            {
                PlayMotionAnimations(entity.isPlayer());

                Vector3 firstPointPosition = currentPath.points[0].transform.position;
                entity.transform.position = Vector3.MoveTowards(entity.transform.position, firstPointPosition, entity.walkSpeed * Time.deltaTime);

                Vector3 lookDirection = (firstPointPosition - entity.transform.position).normalized;
                entity.transform.rotation = Quaternion.Slerp(entity.transform.rotation, Quaternion.LookRotation(lookDirection), Time.deltaTime * 5f);

                if (Vector3.Distance(entity.transform.position, firstPointPosition) == 0)
                {
                    reachedFirst = true;
                    startPosition = firstPointPosition;
                }
            }
            else
            {
                PlayMotionAnimations(entity.isPlayer());

                Vector3 GetPointClamped(int index)
                {
                    index = Mathf.Clamp(index, 0, currentPath.points.Count - 1);
                    return (index == 0) ? startPosition : currentPath.points[index].transform.position;
                }

                Vector3 p0 = GetPointClamped(currentPointIndex - 1);
                Vector3 p1 = GetPointClamped(currentPointIndex);
                Vector3 p2 = GetPointClamped(currentPointIndex + 1);
                Vector3 p3 = GetPointClamped(currentPointIndex + 2);

                // Move along curve.
                UpdateEntityMoveSpeed();
                t += (entity.moveSpeed * Time.deltaTime) / Vector3.Distance(p1, p2);

                // Calculate curve position.
                Vector3 position = GetCatmullRomPosition(t, p0, p1, p2, p3);
                entity.transform.position = position;

                // Rotate toward velocity direction.
                float lookAheadT = Mathf.Min(t + 0.01f, 1f);
                Vector3 nextPosition = GetCatmullRomPosition(lookAheadT, p0, p1, p2, p3);
                Vector3 direction = (nextPosition - position).normalized;

                if (direction.sqrMagnitude > 0.0001f)
                {
                    direction.y = 0;
                    entity.transform.rotation = Quaternion.Slerp(entity.transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 5f);
                }

                // Move to next position when finished.
                if (t >= 1f)
                {
                    t = 0f;
                    currentPointIndex++;
                }
            }
        }

        public void StartFollowingPath(bool loop)
        {
            // Set a target path to follow as long as both target entity and the selected path are valid.

            if (entity == null)
            {
                Debug.LogError("[Npc path system]: No target entity was defined. Path follow operation aborted.");
                return;
            }

            if (currentPath != null)
            {
                if (currentPath.points.Count != 0)
                {
                    entity.isFollowingPath = true;

                    startPosition = entity.transform.position;

                    follow = true;
                    followLoop = loop;
                    reachedFirst = false;

                    currentPointIndex = 0;
                }
                else
                {
                    Debug.LogError($"[Npc path system]: The selected path '{currentPath.name}' has no valid points that form a path. Path follow operation aborted.");
                    return;
                }

            }
            else
            {
                Debug.LogError($"[Npc path system]: Selected path doesnt exsist or is null. Path follow operation aborted.");
                return;
            }
        }

        private void UpdateEntityMoveSpeed()
        {
            Point point = currentPath.points[currentPointIndex];
            switch (point.motion)
            {
                case Point.EntityMotion.Ignore:
                    break;

                case Point.EntityMotion.Run:
                    entity.moveSpeed = entity.runSpeed;
                    break;

                case Point.EntityMotion.Walk:
                    entity.moveSpeed = entity.walkSpeed;
                    break;

                case Point.EntityMotion.Stop:
                    entity.moveSpeed = 0;
                    break;
            }
        }

        private void PlayMotionAnimations(bool isPlayer)
        {
            Animator anim = entity.GetComponent<Animator>();

            if (isPlayer)
            {
                if (!reachedFirst)
                    playerMovementThreshold = 0.5f;
                else
                    // Handle player animations.
                    switch (currentPath.points[currentPointIndex].motion)
                    {
                        case Point.EntityMotion.Walk:
                            playerMovementThreshold = 0.5f;
                            break;

                        case Point.EntityMotion.Run:
                            playerMovementThreshold = 1f;
                            break;

                        case Point.EntityMotion.Stop:
                            playerMovementThreshold = 0f;
                            break;

                        default:
                            if (anim.GetFloat("MoveInput") == 0)
                            {
                                playerMovementThreshold = 0.5f;
                            }
                            break;
                    }

                anim.SetFloat("MoveInput", playerMovementThreshold);
            }
            else
            {
                if (!reachedFirst)
                {
                    anim.SetBool("Motion/Idle", false);
                    anim.SetBool("Motion/Run", false);
                }
                else
                    // Handle npc animations.
                    switch (currentPath.points[currentPointIndex].motion)
                    {
                        case Point.EntityMotion.Walk:
                            anim.SetBool("Motion/Idle", false);
                            anim.SetBool("Motion/Run", false);
                            break;

                        case Point.EntityMotion.Run:
                            anim.SetBool("Motion/Idle", false);
                            anim.SetBool("Motion/Run", true);
                            break;

                        case Point.EntityMotion.Stop:
                            anim.SetBool("Motion/Idle", true);
                            anim.SetBool("Motion/Run", false);
                            break;

                        default:
                            if (anim.GetBool("Motion/Idle"))
                            {
                                anim.SetBool("Motion/Idle", false);
                                anim.SetBool("Motion/Run", false);
                            }
                            break;
                    }
            }
        }

        private void StopMotionAnimation()
        {
            Animator anim = entity.GetComponent<Animator>();

            if (entity.isPlayer())
            {
                anim.SetFloat("MoveInput", 0f);
                anim.SetBool("Turn/Enabled", false);
            }
            else
            {
                anim.SetBool("Motion/Idle", true);
                anim.SetBool("Motion/Run", false);
            }
        }

        private Vector3 GetCatmullRomPosition(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            return 0.5f * (
                (2 * p1) +
                (-p0 + p2) * t +
                (2 * p0 - 5 * p1 + 4 * p2 - p3) * t * t +
                (-p0 + 3 * p1 - 3 * p2 + p3) * t * t * t
            );
        }

        private void StopFollowingPath()
        {
            Debug.Log($"[Npc path system]: {entity.gameObject.name} Reached path end.");
            entity.transform.position = currentPath.points[^1].transform.position;
            follow = false;
            entity.isFollowingPath = false;
            StopMotionAnimation();

            if (controller != null)
                controller.enabled = true;
        }

        public void SetEntity(Entity entity)
        {
            this.entity = entity;
        }
    }
}
