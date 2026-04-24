using Assets.Scripts.GameSerialization;
using Assets.Scripts.Player;
using UnityEngine;

namespace Assets.Scripts.World
{
    public class Collectible : MonoBehaviour
    {
        public enum CollectibleType { Life_Stone, Magic_Crystal, Spirit, Power_Orb };
        public CollectibleType Type;

        private void CollectLifeStone()
        {
            PlayerCore player = FindAnyObjectByType<PlayerCore>();
            player.GetEntity().health += 1;
        }

        private void CollectMagicCrystal()
        {
            PlayerInventory.MagicCrystals += 1;
            Destroy(this);
        }

        private void CollectSpirit()
        {
            PlayerInventory.Spirits += 1;
            Destroy(this);
        }

        private void CollectPowerOrb()
        { 
            PlayerInventory.PowerOrbs += 1;
            Destroy(this);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                switch (Type) 
                {
                    case CollectibleType.Life_Stone:
                        break;

                    case CollectibleType.Magic_Crystal:
                        break;

                    case CollectibleType.Spirit:
                        break;

                    case CollectibleType.Power_Orb:
                        break;
                }
            }
        }
    }
}