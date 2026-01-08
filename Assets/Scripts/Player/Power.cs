using UnityEngine;

namespace Assets.Scripts.Player
{
    public class Power
    {
        // Default cooldown time of 1 second.
        public float cooldownTime = 1.0f;

        // Necesary energy to use this power.
        public float energy = 0.0f;

        // Player powers parent object.
        public GameObject playerPowers;

        // Cooldown counter.
        private float timeCounter = 0f;
        private bool used = false;

        public Power(float cooldownTime, float energy, GameObject playerPowers)
        { 
            this.cooldownTime = cooldownTime;
            this.energy = energy;
            this.playerPowers = playerPowers;
        }

        public virtual void Use()
        {
            // Power usage virtual method.
            if (!ConsumeEnergy())
                return;
        }

        public virtual void Cooldown(float deltaTime)
        {
            if (used)
            {
                timeCounter += deltaTime;
                if (timeCounter >= cooldownTime)
                {
                    used = false;
                    timeCounter = 0f;
                }
            }
        }

        private bool ConsumeEnergy()
        {
            return true;
        }
    }
}
