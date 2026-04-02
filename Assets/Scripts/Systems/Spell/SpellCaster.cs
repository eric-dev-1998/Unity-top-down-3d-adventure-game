using Assets.Scripts.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.Systems.Spell
{
    internal class SpellCaster : MonoBehaviour
    {
        public SpellConfig.MagicElement element;

        public int mana = 100;
        public bool infiniteMana = false;       // For debug purposes only.
        public bool inPosition = false;
        public bool ready = false;

        private bool casting = false;
        private bool canCast = true;
        private float timeElapsedUntilReady = 0f;
        private float castCooldown = 1.6f;
        private float elapsedCooldownTime = 0f;

        // Current spell data:
        private SpellConfig spellConfig;
        private GameObject castObject;

        private PlayerCore playerCore;
        private PlayerInput playerInput;
        private EntityAnimator animator;

        private void Start()
        {
            if (name == "Player")
                playerCore = GetComponent<PlayerCore>();

            playerInput = GetComponent<PlayerInput>();
            animator = GetComponent<EntityAnimator>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
                element = SpellConfig.MagicElement.Neutral;
            else if (Input.GetKeyDown(KeyCode.Alpha2))
                element = SpellConfig.MagicElement.Fire;
            else if (Input.GetKeyDown(KeyCode.Alpha3))
                element = SpellConfig.MagicElement.Water;
            else if (Input.GetKeyDown(KeyCode.Alpha4))
                element = SpellConfig.MagicElement.Wind;
            else if (Input.GetKeyDown(KeyCode.Alpha5))
                element = SpellConfig.MagicElement.Earth;

            if (canCast)
            {
                elapsedCooldownTime += Time.deltaTime;
                if (elapsedCooldownTime >= castCooldown)
                {
                    canCast = false;
                    elapsedCooldownTime = 0f;
                }

                return;
            }

            if (!casting)
            {
                if (playerInput.GetSpellCastInput())
                {
                    // Spell cast inout is beign hold.
                    Debug.Log("Casting...");
                    Cast(element);
                }
            }
            else
            {
                if (playerInput.GetSpellCastInput())
                {
                    if (!spellConfig.isContinuous)
                    {
                        // Continue casting untill player fires.
                        if (spellConfig.element == SpellConfig.MagicElement.Neutral)
                        {
                            if (timeElapsedUntilReady < spellConfig.cooldownTime)
                            {
                                ready = false;
                                timeElapsedUntilReady += Time.deltaTime;
                            }
                            else
                            {
                                ready = true;
                            }
                        }
                        else
                        {
                            Animator anim = castObject.GetComponent<Animator>();
                            if (anim != null)
                            {
                                if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
                                {
                                    StartCoroutine(CastEarth());
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (ready && spellConfig.element == SpellConfig.MagicElement.Neutral)
                        FireOnce();

                    Stop();
                }
            }
        }

        public void Cast(SpellConfig.MagicElement element)
        {
            // Start casting a spell.

            // 1. Load spell config.
            LoadSpellConfig(element);
            if (spellConfig == null)
            {
                Debug.LogError($"[Spell caster]: No spell was found for: {element}.");
                return;
            }

            // 2. Set player animation to spell casting.
            animator.StartSpellCastingMotion(spellConfig.alternateCastAnimation);

            if (element == SpellConfig.MagicElement.Earth)
                castObject = Instantiate(spellConfig.spellCastPrefab, transform.position, transform.rotation);

            casting = true;

            if (playerCore)
                playerCore.LockMovement();
        }

        public void FireOnce()
        {
            // Fire or trigger a spell just once.
            if (spellConfig.element == SpellConfig.MagicElement.Neutral)
            {
                if (!ready)
                {
                    StartCoroutine(DestroyCastVFX(castObject));
                    return;
                }

                StartCoroutine(DestroyCastVFX(castObject));

                Vector3 spawnPosition = transform.position + (transform.forward * 0.5f);
                Instantiate(spellConfig.spellObjectPrefab, spawnPosition, transform.rotation);

                timeElapsedUntilReady = 0f;
                ready = false;
            }
            else if (spellConfig.element == SpellConfig.MagicElement.Earth)
            {
                CastEarth();
            }

            Debug.Log("Fired");

            Stop();
        }

        public void Stop()
        {
            Debug.Log("Stopped");

            // Stop casting a spell.
            // This will be called when the player releases the spell cast input.

            if(spellConfig.isContinuous)
                StartCoroutine(DestroyCastVFX(castObject));

            animator.StopSpellCastingMotion();
            inPosition = false;
            ready = false;
            casting = false;
            timeElapsedUntilReady = 0f;

            canCast = true;
            elapsedCooldownTime = 0f;
        }

        public void InPosition()
        {
            inPosition = true;
            if (!spellConfig.isContinuous && spellConfig.element != SpellConfig.MagicElement.Earth)
                castObject = Instantiate(spellConfig.spellCastPrefab, transform.position, transform.rotation);
            else
                castObject = Instantiate(spellConfig.spellCastPrefab, transform);
        }

        private void LoadSpellConfig(SpellConfig.MagicElement element)
        {
            // Load spell config asset.
            switch (element)
            {
                case SpellConfig.MagicElement.Neutral:
                    spellConfig = Resources.Load<SpellConfig>("Spells/Neutral");
                    break;

                case SpellConfig.MagicElement.Fire:
                    spellConfig = Resources.Load<SpellConfig>("Spells/Fire");
                    break;

                case SpellConfig.MagicElement.Water:
                    spellConfig = Resources.Load<SpellConfig>("Spells/Water");
                    break;

                case SpellConfig.MagicElement.Earth:
                    spellConfig = Resources.Load<SpellConfig>("Spells/Earth");
                    break;

                case SpellConfig.MagicElement.Wind:
                    spellConfig = Resources.Load<SpellConfig>("Spells/Wind");
                    break;
            }
        }

        private IEnumerator DestroyCastVFX(GameObject castObject)
        {
            Animator animator = castObject.GetComponent<Animator>();

            if (!spellConfig.isContinuous)
            {
                animator.SetBool("Cancel", true);
                yield return new WaitForSeconds(0.001f);
                yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
            }
            else
            {
                ParticleSystem particles = castObject.transform.Find("Particles").GetComponent<ParticleSystem>();
                particles.Stop();

                yield return new WaitUntil(() => particles.particleCount <= 0);
            }

            Destroy(castObject);

            yield return new WaitForSeconds(0.75f);
            if (playerCore)
                playerCore.UnlockMovement();
        }

        private IEnumerator CastEarth()
        {
            Stop();

            yield return DestroyCastVFX(castObject);

            Vector3 spawnPosition = transform.position + (transform.forward * 0.75f);
            spawnPosition.y += 0.5f;
            Instantiate(spellConfig.spellObjectPrefab, spawnPosition, transform.rotation);

            yield return new WaitForSeconds(0.75f);
            if (playerCore)
                playerCore.UnlockMovement();
        }
    }
}
