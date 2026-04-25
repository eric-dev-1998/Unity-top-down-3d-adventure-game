using Assets.Scripts.GameSerialization;
using Assets.Scripts.Player;
using Assets.Scripts.Systems.Spell;
using UnityEngine;

namespace Assets.Scripts.World
{
    public class Collectible : MonoBehaviour
    {
        public enum CollectibleType { None, Life_Stone, Magic_Crystal, Spirit, Power_Orb };
        public CollectibleType Type;

        private void CollectLifeStone()
        {
            Entity player = FindAnyObjectByType<PlayerCore>().GetEntity();
            player.AddHealth(1);

            HUDManager hudManager = FindAnyObjectByType<HUDManager>();
            hudManager.SetHealth(player.health, player.maxHealth);

            Destroy(gameObject);
        }

        private void CollectMagicCrystal()
        {
            SpellCaster player = FindAnyObjectByType<PlayerCore>().GetComponent<SpellCaster>();
            player.AddMana(2);

            HUDManager hudManager = FindAnyObjectByType<HUDManager>();
            hudManager.SetMagic(player.mana, player.maxMana);

            PlayerInventory.MagicCrystals += 1;

            hudManager.DisplayCollected(Type);
            Destroy(gameObject);
        }

        private void CollectSpirit()
        {
            PlayerInventory.Spirits += 1;

            HUDManager hudManager = FindAnyObjectByType<HUDManager>();
            hudManager.DisplayCollected(Type);

            Destroy(gameObject);
        }

        private void CollectPowerOrb()
        { 
            PlayerInventory.PowerOrbs += 1;

            HUDManager hudManager = FindAnyObjectByType<HUDManager>();
            hudManager.DisplayCollected(Type);

            Destroy(gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                switch (Type) 
                {
                    case CollectibleType.Life_Stone:
                        CollectLifeStone();
                        break;

                    case CollectibleType.Magic_Crystal:
                        CollectMagicCrystal();
                        break;

                    case CollectibleType.Spirit:
                        CollectSpirit();
                        break;

                    case CollectibleType.Power_Orb:
                        CollectPowerOrb();
                        break;
                }
            }
        }
    }
}