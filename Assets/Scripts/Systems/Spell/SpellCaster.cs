using Assets.Scripts.Inventory_System;
using Assets.Scripts.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.Systems.Spell
{
    public class SpellCaster : MonoBehaviour
    {
        public SpellConfig.MagicElement element;

        public int mana = 100;
        public int maxMana = 100;
        public bool infiniteMana = false;       // For debug purposes only.
        public bool inPosition = false;
        public bool ready = false;

        public bool casting = false;
        private bool canCast = true;
        private float timeElapsedUntilReady = 0f;
        private float castCooldown = 0.6f;
        private float elapsedCooldownTime = 0f;
        public int consumedManaPerSecond = 3;
        private float _constantCastingTimer = 0f;

        // Current spell data:
        private SpellConfig spellConfig;
        private GameObject castObject;

        private PlayerCore playerCore;
        private PlayerInput playerInput;
        private EntityAnimator animator;

        private HUDManager _hudManager;

        public void AddMana(int count)
        {
            mana += count;
            if(mana >= maxMana)
                mana = maxMana;
        }

        private void Start()
        {
            if (name == "Player")
                playerCore = GetComponent<PlayerCore>();

            playerInput = GetComponent<PlayerInput>();
            animator = GetComponent<EntityAnimator>();

            _hudManager = FindAnyObjectByType<HUDManager>();
        }

        private void Update()
        {
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
                            if (!ready)
                            {
                                Animator anim = castObject.GetComponent<Animator>();
                                if (anim != null)
                                {
                                    if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
                                    {
                                        ready = true;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        // Consume mana over time
                        _constantCastingTimer += Time.deltaTime;
                        if (_constantCastingTimer >= 1.0f)
                        {
                            mana -= consumedManaPerSecond;
                            _hudManager.SetMagic(mana, maxMana);

                            _constantCastingTimer = 0.0f;
                        }
                    }
                }
                else
                {
                    if (ready)
                    {
                        if (spellConfig.element == SpellConfig.MagicElement.Neutral)
                        {
                            FireOnce();
                            mana -= 5;
                        }

                        if (spellConfig.element == SpellConfig.MagicElement.Earth)
                        {
                            ThrowRock();
                            mana -= 10;
                        }

                        _hudManager.SetMagic(mana, maxMana);
                    }

                    Stop();
                }
            }
        }

        public void SetElement(int index)
        {
            switch (index)
            {
                case 0:
                    // Neutral
                    element = SpellConfig.MagicElement.Neutral;
                    break;

                case 1:
                    // Fire
                    element = SpellConfig.MagicElement.Fire;
                    break;

                case 2:
                    // Water
                    element = SpellConfig.MagicElement.Water;
                    break;

                case 3:
                    // Wind
                    element = SpellConfig.MagicElement.Wind; 
                    break;
                case 4:
                    // Earth
                    element = SpellConfig.MagicElement.Earth;
                    break;
            }

            Debug.Log("Changed element.");
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

            if (name == "Player")
            {
                InventoryManager iManager = FindAnyObjectByType<InventoryManager>();

                switch (spellConfig.element)
                { 
                    case SpellConfig.MagicElement.Neutral:
                        if (iManager.GetItemCount("eo_neutral") <= 0)
                            return;
                        break;

                    case SpellConfig.MagicElement.Earth:
                        if (iManager.GetItemCount("eo_earth") <= 0)
                            return;
                        break;

                    case SpellConfig.MagicElement.Fire:
                        if (iManager.GetItemCount("eo_fire") <= 0)
                            return;
                        break;

                    case SpellConfig.MagicElement.Water:
                        if (iManager.GetItemCount("eo_water") <= 0)
                            return;
                        break;

                    case SpellConfig.MagicElement.Wind:
                        if (iManager.GetItemCount("eo_wind") <= 0)
                            return;
                        break;
                }

                if (mana <= 0)
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
                GameObject objProjectile = Instantiate(spellConfig.spellObjectPrefab, spawnPosition, transform.rotation);
                Projectile projectile = objProjectile.GetComponent<Projectile>();
                projectile.SetCaster(this);

                timeElapsedUntilReady = 0f;
                ready = false;
            }

            Debug.Log("Fired");

            Stop();
        }

        public void Stop()
        {
            Debug.Log("Stopped");

            // Stop casting a spell.
            // This will be called when the player releases the spell cast input.

            if(castObject != null)
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

            if (spellConfig.isContinuous)
            {
                animator.SetBool("Cancel", true);
                yield return new WaitForSeconds(0.001f);
                yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
            }

            ParticleSystem particles = castObject.transform.Find("Particles").GetComponent<ParticleSystem>();
            if (particles != null)
            {
                particles.Stop();
                yield return new WaitUntil(() => particles.particleCount <= 0);
            }

            Destroy(castObject);

            yield return new WaitForSeconds(0.4f);
            if (playerCore)
                playerCore.UnlockMovement();
        }

        private IEnumerator CastEarth()
        {
            Stop();

            yield return DestroyCastVFX(castObject);

            yield return new WaitForSeconds(0.75f);
            if (playerCore)
                playerCore.UnlockMovement();

            ready = true;
        }

        public void ThrowRock()
        {
            Debug.Log("Pium");

            ready = false;

            Vector3 spawnPosition = transform.position + (transform.forward * 0.75f);
            spawnPosition.y += 0.5f;
            GameObject rock = Instantiate(spellConfig.spellObjectPrefab, spawnPosition, transform.rotation);

            Rigidbody rb = rock.GetComponent<Rigidbody>();
            Vector3 forceDirection = transform.forward * 60 + Vector3.up;
            rb.AddForce(forceDirection, ForceMode.Impulse);
        }
    }
}
