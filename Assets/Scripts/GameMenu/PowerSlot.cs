using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Main
{
    public class PowerSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public enum Power
        { 
            Wind,
            Earth,
            Water,
            Fire,
            Light,
            Dark
        }

        public Power power;

        private GameObject sprite;
        private GameObject hover;
        private GameObject hudPowers;

        private bool onHover = false;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            sprite = transform.Find("Sprite").gameObject;
            sprite.SetActive(false);

            hover = transform.Find("Hover").gameObject;
            hover.SetActive(false);

            hudPowers = GameObject.Find("HUD").transform.Find("Powers").gameObject;
        }

        // Update is called once per frame
        void Update()
        {
            // Handle input.
            if (onHover) 
            {
                if (Input.GetKeyDown(KeyCode.Mouse0))
                    SelectPowerOnLeft();
                if (Input.GetKeyDown(KeyCode.Mouse1))
                    SelectPowerOnRight();
            }

            // Display orb icon if unlocked.
            if (power == Power.Wind && PlayerInventory.power_wind)
                sprite.SetActive(true);

            if (power == Power.Earth && PlayerInventory.power_earth)
                sprite.SetActive(true);

            if (power == Power.Water && PlayerInventory.power_water)
                sprite.SetActive(true);

            if (power == Power.Fire && PlayerInventory.power_fire)
                sprite.SetActive(true);

            if (power == Power.Light && PlayerInventory.power_light)
                sprite.SetActive(true);

            if (power == Power.Dark && PlayerInventory.power_dark)
                sprite.SetActive(true);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (power == Power.Wind && PlayerInventory.power_wind)
            {
                hover.SetActive(true);
                onHover = true;
            }

            if (power == Power.Earth && PlayerInventory.power_earth)
            {
                hover.SetActive(true);
                onHover = true;
            }

            if (power == Power.Water && PlayerInventory.power_water)
            {
                hover.SetActive(true);
                onHover = true;
            }

            if (power == Power.Fire && PlayerInventory.power_fire)
            {
                hover.SetActive(true);
                onHover = true;
            }

            if (power == Power.Light && PlayerInventory.power_light)
            {
                hover.SetActive(true);
                onHover = true;
            }

            if (power == Power.Dark && PlayerInventory.power_dark)
            {
                hover.SetActive(true);
                onHover = true;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            onHover = false;
            hover.SetActive(false);
        }

        private void SelectPowerOnLeft()
        {
            GameObject leftSlot = hudPowers.transform.Find("Slot_Left").gameObject;
            Image leftIcon = leftSlot.transform.Find("Icon").GetComponent <Image>();
            leftIcon.enabled = true;

            int power_int = (int)power;

            if (power_int == PlayerInventory.selectedPowerRight)
            {
                // The selected power is already on the right slot.

                GameObject rightSlot = hudPowers.transform.Find("Slot_Right").gameObject;
                Image rightIcon = rightSlot.transform.Find("Icon").GetComponent<Image>();
                rightIcon.enabled = true;

                // Get current sprites and inventory data (before doing any changes).
                Sprite left = leftIcon.sprite;
                Sprite right = rightIcon.sprite;
                int left_int = PlayerInventory.selectedPowerLeft;
                int right_int = PlayerInventory.selectedPowerRight;

                // Switch sprites and inventory data.
                leftIcon.sprite = right;
                rightIcon.sprite = left;
                PlayerInventory.selectedPowerLeft = right_int;
                PlayerInventory.selectedPowerRight = left_int;
            }
            else
            { 
                leftIcon.sprite = sprite.GetComponent<Image>().sprite;
                PlayerInventory.selectedPowerLeft = power_int;
            }
        }

        private void SelectPowerOnRight()
        { 
            GameObject rightSlot = hudPowers.transform.Find("Slot_Right").gameObject;
            Image rightIcon = rightSlot.transform.Find("Icon").GetComponent<Image>();
            rightIcon.enabled = true;

            int power_int = (int)power;

            if (power_int == PlayerInventory.selectedPowerLeft)
            {
                // The selected power is already selected in the left slot.

                GameObject leftSlot = hudPowers.transform.Find("Slot_Left").gameObject;
                Image leftIcon = leftSlot.transform.Find("Icon").GetComponent<Image>(); 
                leftIcon.enabled = true;

                // Get current sprites and inventory data (before doing any changes).
                Sprite left = leftIcon.sprite;
                Sprite right = rightIcon.sprite;
                int left_int = PlayerInventory.selectedPowerLeft;
                int right_int = PlayerInventory.selectedPowerRight;

                // Switch sprites and inventory data.
                leftIcon.sprite = right;
                rightIcon.sprite = left;
                PlayerInventory.selectedPowerLeft = right_int;
                PlayerInventory.selectedPowerRight = left_int;
            }
            else
            { 
                rightIcon.sprite = sprite.GetComponent<Image>().sprite;
                PlayerInventory.selectedPowerRight = power_int;
            }
        }
    }
}
