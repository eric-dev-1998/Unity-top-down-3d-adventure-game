using Assets.Scripts.Event_system.Events;
using Assets.Scripts.Event_System;
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
            if (PlayerData.FirstLifeStone)
            {
                DisplayFirstTimeDialogue("life_stone");
                PlayerData.FirstLifeStone = false;
            }

            Entity player = FindAnyObjectByType<PlayerCore>().GetEntity();
            player.AddHealth(1);

            HUDManager hudManager = FindAnyObjectByType<HUDManager>();
            hudManager.SetHealth(player.health, player.maxHealth);

            Destroy(gameObject);
        }

        private void CollectMagicCrystal()
        {
            if (PlayerData.FirstMagicCrystal)
            {
                DisplayFirstTimeDialogue("magic_crystal");
                PlayerData.FirstMagicCrystal = false;
            }

            SpellCaster player = FindAnyObjectByType<PlayerCore>().GetComponent<SpellCaster>();
            player.AddMana(2);

            HUDManager hudManager = FindAnyObjectByType<HUDManager>();
            hudManager.SetMagic(player.mana, player.maxMana);

            PlayerData.MagicCrystals += 1;

            hudManager.DisplayCollected(Type);
            Destroy(gameObject);
        }

        private void CollectSpirit()
        {
            if (PlayerData.FirstSpirit)
            {
                DisplayFirstTimeDialogue("spirit");
                PlayerData.FirstSpirit = false;
            }

            PlayerData.Spirits += 1;

            HUDManager hudManager = FindAnyObjectByType<HUDManager>();
            hudManager.DisplayCollected(Type);

            Destroy(gameObject);
        }

        private void CollectPowerOrb()
        {
            if (PlayerData.FirstPowerOrb)
            {
                DisplayFirstTimeDialogue("power_orb");
                PlayerData.FirstPowerOrb = false;
            }

            PlayerData.PowerOrbs += 1;

            HUDManager hudManager = FindAnyObjectByType<HUDManager>();
            hudManager.DisplayCollected(Type);

            Destroy(gameObject);
        }

        private void DisplayFirstTimeDialogue(string item_id)
        {
            EventSequence sequence = new EventSequence();

            GameObjectEvent objectEvent = ScriptableObject.CreateInstance<GameObjectEvent>();
            objectEvent.type = GameObjectEvent.EventType.Disable;
            objectEvent.name = gameObject.name;
            objectEvent.Load();

            HumanGesture gestureEvent = ScriptableObject.CreateInstance<HumanGesture>();
            gestureEvent.characterId = "Player";
            gestureEvent.gestureType = 3;
            gestureEvent.wait = true;

            ItemEvent evt = ScriptableObject.CreateInstance<ItemEvent>();
            evt.type = ItemEvent.ItemEventType.Get;
            evt.id = item_id;
            evt.count = 1;

            GameObjectEvent objectEvent1 = ScriptableObject.CreateInstance<GameObjectEvent>();
            objectEvent1.type = GameObjectEvent.EventType.Destroy;
            objectEvent1.name = gameObject.name;
            objectEvent1.Load();

            gestureEvent.gestureType = 3;
            gestureEvent.next.Add(objectEvent);

            objectEvent.next.Add(evt);
            evt.next.Add(objectEvent1);

            sequence = ScriptableObject.CreateInstance<EventSequence>();
            sequence.startEvent = gestureEvent;

            EventManager eventManager = FindAnyObjectByType<EventManager>();
            eventManager.StartSequence(sequence, true);
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