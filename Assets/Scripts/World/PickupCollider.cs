using UnityEngine;

namespace Assets.Scripts.World
{
    public class PickupCollider
    {
        // Direction to pick item from:
        public enum PickupDirection
        {
            Any, Up, Down, Left, Right
        }
        private PickupDirection pickupDirection;

        // Direction colliders: 
        // Each collider represent a direction which player can pickup from.
        private BoxCollider collider_up;
        private BoxCollider collider_down;
        private BoxCollider collider_left;
        private BoxCollider collider_right;

        // Collider default size:
        private Vector3 size = new Vector3(0.9f, 0.9f, 0.9f);

        // Parent game object instance:
        private GameObject gameObject;

        public PickupCollider(GameObject gameObject, PickupDirection direction)
        { 
            this.gameObject = gameObject;

            if (direction == PickupDirection.Any)
                CreateColliders();
            else
                CreateCollider(direction);
        }

        private void CreateColliders()
        {
            collider_up = gameObject.AddComponent<BoxCollider>();
            collider_down = gameObject.AddComponent<BoxCollider>();
            collider_left = gameObject.AddComponent<BoxCollider>();
            collider_right = gameObject.AddComponent<BoxCollider>();

            collider_up.isTrigger = true;
            collider_down.isTrigger = true;
            collider_left.isTrigger = true;
            collider_right.isTrigger = true;

            collider_up.center = new Vector3(0, 0.45f, 1);
            collider_down.center = new Vector3(0, 0.45f, -1);
            collider_left.center = new Vector3(-1, 0.45f, 0);
            collider_right.center = new Vector3(1, 0.45f, 0);

            collider_up.size = size;
            collider_down.size = size;
            collider_left.size = size;
            collider_right.size = size;
        }

        private void CreateCollider(PickupDirection direction)
        {
            switch (direction) 
            {
                case PickupDirection.Up:
                    collider_up = gameObject.AddComponent<BoxCollider>();
                    collider_up.isTrigger = true;
                    collider_up.center = new Vector3(0, 0.45f, 1);
                    collider_up.size = size;
                    break;

                case PickupDirection.Down:
                    collider_down = gameObject.AddComponent<BoxCollider>();
                    collider_down.isTrigger = true;
                    collider_down.center = new Vector3(0, 0.45f, -1);
                    collider_down.size = size;
                    break;

                case PickupDirection.Left:
                    collider_left = gameObject.AddComponent<BoxCollider>();
                    collider_left.isTrigger = true;
                    collider_left.center = new Vector3(-1, 0.45f, 0);
                    collider_left.size = size;
                    break;

                case PickupDirection.Right:
                    collider_right = gameObject.AddComponent<BoxCollider>();
                    collider_right.isTrigger = true;
                    collider_right.center = new Vector3(1, 0.45f, 0);
                    collider_right.size = size;
                    break;
            }
        }

        public bool IsPlayerLooking()
        {
            float player_direction = GameObject.Find("Player").transform.Find("Sprite").GetComponent<Animator>().GetFloat("Direction");

            if (pickupDirection != PickupDirection.Any)
            {
                switch (pickupDirection)
                {
                    case PickupDirection.Up:
                        if (player_direction == 1)
                            return true;
                        else
                            return false;

                    case PickupDirection.Down:
                        if (player_direction == 0)
                            return true;
                        else
                            return false;

                    case PickupDirection.Left:
                        if (player_direction == 3)
                            return true;
                        else
                            return false;

                    case PickupDirection.Right:
                        if (player_direction == 2)
                            return true;
                        else
                            return false;
                }

                return false;
            }
            else
            {
                Vector3 player_position = GameObject.Find("Player").transform.position;
                Vector3 position = gameObject.transform.position;

                if (player_position.x == position.x)
                {
                    if (player_position.z > position.z)
                    {
                        if (player_direction == 1)
                            return true;
                        else
                            return false;
                    }
                    else
                    {
                        if (player_direction == 0)
                            return true;
                        else
                            return false;
                    }
                }

                if (player_position.z == position.z)
                {
                    if (player_position.x > position.x)
                    {
                        if (player_direction == 2)
                            return true;
                        else
                            return false;
                    }
                    else
                    {
                        if (player_direction == 3)
                            return true;
                        else
                            return false;
                    }
                }

                return false;
            }
        }
    }
}
