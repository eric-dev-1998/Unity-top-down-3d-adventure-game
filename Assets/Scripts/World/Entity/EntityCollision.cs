using UnityEngine;

/*
    This is the Entity Collision manager.
    Players and most NPC's use the 'Entity' class to move'em around.

    This collision system help 'entities' to check it theres something solid in their way
    to be able to move in a certain direction.
    
    Raycast are used on each direction: up, down, left and right. Up and down match the Z axis
    and left and right match the X axis. These raycast are 1 block long (minimumCollisionDistance)
    and work independently. If any raycast hit something it means the entity cannot move on that 
    specific direction.

    Each raycast work with a collision flag: collisionUp, collisionDown, collisionLeft and collisionRight,
    these flags are the connection with 'Entity' and 'EntityCollision' and tell which directions are free
    to move.
 */

public class EntityCollision : MonoBehaviour
{
    // Show rays in the unity editior?
    public bool showRays = false;

    // Distance on which a raycast check collisions.
    public float minimumCollisionDistance = 1;
    public float maxSlopeAngle = 45f;

    // Entity.
    PlayerAnimations playerAnim;
    Entity entity;

    // Direction vectors to simulate rays.
    Vector3 up, down, left, right = Vector3.zero;
    Vector3 slopeVector = Vector3.zero;
    Vector3 origin = Vector3.zero;

    LayerMask pickupLayer;

    private void Start()
    {
        // Get entity.
        playerAnim = GetComponent<PlayerAnimations>();
        entity = GetComponent<Entity>();

        origin = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);

        // Set direction vectors.
        up = new Vector3(0, 0, minimumCollisionDistance);
        down = new Vector3(0, 0, -minimumCollisionDistance);
        left = new Vector3(-minimumCollisionDistance, 0, 0);
        right = new Vector3(minimumCollisionDistance, 0, 0);

        // Get the pickups layer, a pickup item should not be considered a solid collision.
        pickupLayer = LayerMask.GetMask("Pickups", "SequenceTrigger", "PowerEffects", "Trigger");
    }

    private void Update()
    {
        // Show rays if enabled.
        if(showRays)
            DrawRays();
    }

    void DrawRays()
    {
        // Draw rays in the editor if enabled.

        Debug.DrawRay(origin, up, Color.red);
        Debug.DrawRay(origin, down, Color.green);
        Debug.DrawRay(origin, left, Color.blue);
        Debug.DrawRay(origin, right, Color.yellow);
        Debug.DrawRay(slopeVector, Vector3.down * 10, Color.white);
    }
}
