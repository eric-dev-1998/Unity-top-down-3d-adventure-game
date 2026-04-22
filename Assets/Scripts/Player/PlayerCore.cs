using Assets.Scripts.Inventory_System;
using Assets.Scripts.Systems.Spell;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class PlayerCore : MonoBehaviour
    {
        private Entity entity;
        private PlayerInput playerInput;
        private PlayerAudio playerAudio;

        public bool canMove = true;
        public bool onGrab = false;
        public float currentWaterHeight = 0f;

        private Dictionary<int, string> _availableMagic = new();

        private float velocity = 0;
        private int _selectedMagicIndex = -1;
        private int _selectedMagicKey = -1;

        private void Awake()
        {
            entity = GetComponent<Entity>();

            if (!entity)
            {
                entity = gameObject.GetComponent<Entity>();
                if (!entity)
                    Application.Quit();
            }

            playerAudio = GetComponent<PlayerAudio>();

            playerInput = GetComponent<PlayerInput>();
            if (!playerInput)
            {
                playerInput = gameObject.GetComponent<PlayerInput>();
                if (!playerInput)
                {
                    gameObject.AddComponent<PlayerInput>();
                    playerInput = gameObject.GetComponent<PlayerInput>();
                }
            }
        }

        public Entity GetEntity()
        {
            return entity;
        }

        public PlayerAudio GetAudio()
        {
            return playerAudio;
        }

        public void LockMovement()
        {
            canMove = false;
            entity.LockMovement();
        }

        public void UnlockMovement()
        {
            canMove = true;
            entity.UnlockMovement();
        }

        public void UnlockMagicPower(int index)
        {
            if (_availableMagic.ContainsKey(index))
                return;

            switch (index)
            {
                case 0:
                    // Neutral
                    _availableMagic.Add(index, "Neutral");
                    break;

                case 1:
                    // Fire
                    _availableMagic.Add(index, "Fire");
                    break;

                case 2:
                    // Water
                    _availableMagic.Add(index, "Water");
                    break;

                case 3:
                    // Neutral
                    _availableMagic.Add(index, "Wind");
                    break;

                case 4:
                    // Neutral
                    _availableMagic.Add(index, "Earth");
                    break;
            }
        }


        public void SelectMagic(int i)
        {
            int newIndex = _selectedMagicIndex + i;

            // Clamp index value.
            if (newIndex > _availableMagic.Count - 1)
                newIndex = 0;

            if (newIndex < 0)
                newIndex = _availableMagic.Count - 1;

            // Select magic.
            if (_availableMagic.Count > 0)
            {
                var magic = _availableMagic.ElementAt(newIndex);
                _selectedMagicIndex = newIndex;
                _selectedMagicKey = magic.Key;

                SpellCaster caster = GetComponent<SpellCaster>();
                caster.SetElement(_selectedMagicKey);
            }
        }

        public int GetSelectedMagicIndex()
        {
            return _selectedMagicKey;
        }

        public string GetSelectedMagicName()
        {
            return _availableMagic[_selectedMagicKey];
        }
    }
}
