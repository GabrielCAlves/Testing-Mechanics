using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    [Header("Attributes")]
    public int healAmount;

    [Header("Buffs")]
    public float speedBoost;
    public float duration;


    public string id;
    public string itemName;
    public Sprite icon;
    public string itemDescription;
    public int maxStack = 99;
    public ItemType type;
    public GameObject worldPrefab;
    public int pointValue;
    public GameObject collectionEffect;
    public AudioClip collectionSound;
}