using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Main
{
    public class NpcPathSystem : MonoBehaviour
    {
        // Paths info.
        public List<NpcPath> paths = new List<NpcPath>();

        // References the path that is currently set to be followed. Is null by default.
        private NpcPath selectedPath;

        private bool followEnabled = false;
        private bool followLoop = false;
        private bool followingBackwards = false;
        private bool reachedFirst = false;

        private int currentPointIndex = 0;
        private float t = 0f;
        private float playerMovementThreshold = 0f;

        private Vector3 velocity = Vector3.zero;
        private Vector3 startPosition = Vector3.zero;

        private Transform target;

        // Entity that will follow a path.
        private Entity targetEntity;

        public bool isFollowing()
        {
            return followEnabled;
        }

        private void Start()
        {
            // Stop rendering path point display meshes in game mode.
            foreach (var path in paths)
            {
                for (int i = 0; i < path.pathPoints.Count; i++)
                { 
                    Transform currentPoint = path.pathPoints[i].transform;
                    currentPoint.GetComponent<MeshRenderer>().enabled = false;
                }
            }

            // This game object entity component will be set to target entity by default if this
            // game object has one. This can be changed calling 'SetTargetEntity()'.
            Entity localEntity = GetComponent<Entity>();
            if(localEntity != null)
                targetEntity = localEntity;
        }

        private void Update()
        {
            if (followEnabled)
            {
                if (selectedPath.pathPoints.Count >= 4)
                    MoveOverPathCurve();
                else if (selectedPath.pathPoints.Count == 2)
                    MoveOverPathLine();
                else
                {
                    Debug.LogError("[Npc path system]: Current path points quantity doesnt match the required value, operation aborted.");
                    StopFollowingPath();
                }
            }
        }

        private void MoveOverPathLine()
        {
            PlayMotionAnimations(targetEntity.isPlayer());

            Vector3 currentPointPosition = selectedPath.pathPoints[currentPointIndex].transform.position;
            targetEntity.transform.position = Vector3.MoveTowards(targetEntity.transform.position, currentPointPosition, targetEntity.walkSpeed * Time.deltaTime);

            Vector3 lookDirection = (currentPointPosition - targetEntity.transform.position).normalized;
            lookDirection.y = 0;
            targetEntity.transform.rotation = Quaternion.Slerp(targetEntity.transform.rotation, Quaternion.LookRotation(lookDirection), Time.deltaTime * 5f);

            if (Vector3.Distance(targetEntity.transform.position, currentPointPosition) == 0)
            {
                if (currentPointIndex >= selectedPath.pathPoints.Count - 1)
                    StopFollowingPath();
                else
                    currentPointIndex++;
            }
        }
        private void MoveOverPathCurve()
        {
            // The path requires at least 4 path points to work;
            if (selectedPath.pathPoints.Count < 4)
                return;

            // Stop if target entity is already on the last path point.
            if (currentPointIndex >= selectedPath.pathPoints.Count - 1)
            {
                StopFollowingPath();
                return;
            }

            if (!reachedFirst)
            {
                PlayMotionAnimations(targetEntity.isPlayer());

                Vector3 firstPointPosition = selectedPath.pathPoints[0].transform.position;
                targetEntity.transform.position = Vector3.MoveTowards(targetEntity.transform.position, firstPointPosition, targetEntity.walkSpeed * Time.deltaTime);

                Vector3 lookDirection = (firstPointPosition - targetEntity.transform.position).normalized;
                targetEntity.transform.rotation = Quaternion.Slerp(targetEntity.transform.rotation, Quaternion.LookRotation(lookDirection), Time.deltaTime * 5f);

                if (Vector3.Distance(targetEntity.transform.position, firstPointPosition) == 0)
                {
                    reachedFirst = true;
                    startPosition = firstPointPosition;
                }
            }
            else
            {
                PlayMotionAnimations(targetEntity.isPlayer());

                Vector3 GetPointClamped(int index)
                {
                    index = Mathf.Clamp(index, 0, selectedPath.pathPoints.Count - 1);
                    return (index == 0) ? startPosition : selectedPath.pathPoints[index].transform.position;
                }

                Vector3 p0 = GetPointClamped(currentPointIndex - 1);
                Vector3 p1 = GetPointClamped(currentPointIndex);
                Vector3 p2 = GetPointClamped(currentPointIndex + 1);
                Vector3 p3 = GetPointClamped(currentPointIndex + 2);

                // Move along curve.
                UpdateEntityMoveSpeed();
                t += (targetEntity.moveSpeed * Time.deltaTime) / Vector3.Distance(p1, p2);

                // Calculate curve position.
                Vector3 position = GetCatmullRomPosition(t, p0, p1, p2, p3);
                targetEntity.transform.position = position;

                // Rotate toward velocity direction.
                float lookAheadT = Mathf.Min(t + 0.01f, 1f);
                Vector3 nextPosition = GetCatmullRomPosition(lookAheadT, p0, p1, p2, p3);
                Vector3 direction = (nextPosition - position).normalized;

                if (direction.sqrMagnitude > 0.0001f)
                {
                    direction.y = 0;
                    targetEntity.transform.rotation = Quaternion.Slerp(targetEntity.transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 5f);
                }

                // Move to next position when finished.
                if (t >= 1f)
                {
                    t = 0f;
                    currentPointIndex++;
                }
            }
        }

        public void StartFollowingPath(int index, bool loop)
        {
            // Set a target path to follow as long as both target entity and the selected path are valid.

            if (targetEntity == null)
            {
                Debug.LogError("[Npc path system]: No target entity was defined. Path follow operation aborted.");
                return;
            }

            if (paths[index] != null)
            {
                if (paths[index].pathPoints.Count != 0)
                {
                    targetEntity.isFollowingPath = true;
                    selectedPath = paths[index];

                    startPosition = targetEntity.transform.position;

                    followEnabled = true;
                    followLoop = loop;
                    reachedFirst = false;

                    currentPointIndex = 0;
                }
                else
                {
                    Debug.LogErrorFormat("[Npc path system]: The selected path '{0}' has no valid points that form a path. Path follow operation aborted.", paths[index].pathName);
                    return;
                }

            }
            else
            {
                Debug.LogErrorFormat("[Npc path system]: Selected path index '[{0}]' doesnt exsist or is null. Path follow operation aborted.", index);
                return;
            }
        }

        private void UpdateEntityMoveSpeed()
        { 
            PathPoint point = selectedPath.pathPoints[currentPointIndex];
            switch (point.motion)
            {
                case PathPoint.EntityMotion.Ignore:
                    break;

                case PathPoint.EntityMotion.Run:
                    targetEntity.moveSpeed = targetEntity.runSpeed;
                    break;

                case PathPoint.EntityMotion.Walk:
                    targetEntity.moveSpeed = targetEntity.walkSpeed;
                    break;

                case PathPoint.EntityMotion.Stop:
                    targetEntity.moveSpeed = 0;
                    break;
            }
        }

        private void PlayMotionAnimations(bool isPlayer)
        {
            Animator anim = targetEntity.GetComponent<Animator>();

            if(isPlayer)
            {
                if (!reachedFirst)
                    playerMovementThreshold = 0.5f;
                else
                    // Handle player animations.
                    switch (selectedPath.pathPoints[currentPointIndex].motion)
                    {
                        case PathPoint.EntityMotion.Walk:
                            playerMovementThreshold = 0.5f;
                            break;

                        case PathPoint.EntityMotion.Run:
                            playerMovementThreshold = 1f;
                            break;

                        case PathPoint.EntityMotion.Stop:
                            playerMovementThreshold = 0f;
                            break;

                        default:
                            if (anim.GetFloat("MoveInput") == 0)
                            {
                                playerMovementThreshold = 0.5f;
                            }
                            break;
                    }

                anim.SetFloat("MoveInput", Mathf.Lerp(anim.GetFloat("MoveInput"), playerMovementThreshold, Time.deltaTime * 3));
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
                    switch (selectedPath.pathPoints[currentPointIndex].motion)
                    {
                        case PathPoint.EntityMotion.Walk:
                            anim.SetBool("Motion/Idle", false);
                            anim.SetBool("Motion/Run", false);
                            break;

                        case PathPoint.EntityMotion.Run:
                            anim.SetBool("Motion/Idle", false);
                            anim.SetBool("Motion/Run", true);
                            break;

                        case PathPoint.EntityMotion.Stop:
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
            Animator anim = targetEntity.GetComponent<Animator>();

            if (targetEntity.isPlayer())
            {
                anim.SetFloat("MoveInput", 0f);
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
            Debug.Log("[Npc path system]: Reached path end.");
            targetEntity.transform.position = selectedPath.pathPoints[^1].transform.position;
            followEnabled = false;
            targetEntity.isFollowingPath = false;
            StopMotionAnimation();
        }

        public void SetTargetEntity(Entity entity)
        { 
            targetEntity = entity;
        }
    }
}
