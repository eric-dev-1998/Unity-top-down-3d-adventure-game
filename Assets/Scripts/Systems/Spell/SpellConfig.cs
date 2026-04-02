using UnityEngine;

[CreateAssetMenu(fileName = "SpellConfig", menuName = "Game/SpellConfig")]
public class SpellConfig : ScriptableObject
{
    public enum MagicElement { Neutral, Fire, Water, Earth, Wind };

    [Header("Primary settings:")]
    public MagicElement element;
    public bool isContinuous = false;
    public bool alternateCastAnimation = false;
    public int power = 1;
    public int manaCost = 10;                   // Mana cost per second will be used instead if continuous is true.
    public int manaCostPerSecond = 1;
    public float timeBetweenEachHit = 0.5f;
    public float cooldownTime = 2.0f;
    public GameObject spellCastPrefab;          // This one represent the visual effects: particles, sprites, sfx, etc.
    public GameObject spellObjectPrefab;        // This one represent a projectile or spawnable object.
}
