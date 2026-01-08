using UnityEngine;
using Assets.Scripts.Event_System;
using Assets.Scripts;

namespace Assets.Scripts.Player
{
    public class PlayerPowers : MonoBehaviour
    {
        Power_Wind wind;
        Inventory_System.Manager inventory;
        Manager eventManager;

        private void Start()
        {
            inventory = FindAnyObjectByType<Inventory_System.Manager>();
            eventManager = FindAnyObjectByType<Manager>();
        }

        private void Update()
        {
            HandleCooldown(Time.deltaTime);
            //HandleInput();
        }

        private void HandleInput()
        {
            if (!inventory.open && !eventManager.busy)
            {
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    // Use selected power in the left slot.
                    UsePower(PlayerInventory.selectedPowerLeft);
                }
                else if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    // Use slected power in the right slot.
                    UsePower(PlayerInventory.selectedPowerRight);
                }
            }
        }

        private void HandleCooldown(float deltaTime)
        {
            if (wind != null)
                wind.Cooldown(deltaTime);
        }

        public void UsePower(int power)
        {
            switch (power)
            {
                case 0:
                    // Use wind.
                    wind.Use();
                    break;
            }
        }

        public void UnlockPower(int power)
        {
            GameObject playerPowers = GameObject.Find("Player").transform.Find("Powers").gameObject;

            switch (power)
            {
                case 0:
                    // Unlock wind.
                    PlayerInventory.power_wind = true;
                    wind = new Power_Wind(1.0f, 0.0f, playerPowers);
                    break;
            }
        }
    }
}
