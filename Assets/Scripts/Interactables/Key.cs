using UnityEngine;
using Assets.Scripts.Dialogue_System;

public class Key : MonoBehaviour
{
    // Key obtain dialogue.
    Dialogue dialogue;

    // Dialogue system.
    Manager dialogueBox;

    private void Start()
    {
        // Key dialogue.
        dialogue = new Dialogue();
        /*
        dialogue.dialogueLines = new string[] {
            "Has conseguido una llave!",
            "Puedes usarla para abrir una puerta que tenga un candado encima."
        };
        */

        // Dialogue system reference.
        dialogueBox = GameObject.Find("DialogueBox").GetComponent<Manager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // This event triggers when player collides with this key.

        if (other.tag == "Player")
        {
            // Show dialogue.
            dialogueBox.center = true;
            //dialogueBox.StartDialogue(dialogue);

            // Add +1 to the player keys counter and destroy this game object.
            PlayerInventory.keys += 1;
            Destroy(gameObject);
        }
    }
}
