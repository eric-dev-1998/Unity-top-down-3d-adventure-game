using UnityEngine;

namespace Main
{
    public class Item_Pickup : MonoBehaviour
    {
        // Event properties.
        public string item_id;
        public int item_count = 1;

        private bool is_player_colliding = false;
        private bool already_picked_up = false;

        private void Start()
        {
            DeleteEditorDisplayCube();
            LoadSprite();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                if (is_player_colliding && !already_picked_up)
                    // Do give item sequence.
                    GiveItem();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
                is_player_colliding = true;
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "Player")
                is_player_colliding = false;
        }

        private void DeleteEditorDisplayCube()
        {
            GameObject cube = transform.Find("Editor_Display").gameObject;
            Destroy(cube);
        }

        private void LoadSprite()
        {

        }

        private void GiveItem()
        {
            
        }
    }
}
