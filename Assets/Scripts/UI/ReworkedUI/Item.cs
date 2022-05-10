using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Item : ScriptableObject
{
    [SerializeField] private GameObject itemPrefab;

    [TextArea(20, 20)]
    private string description;
    public string Description { get {return description; } }
    public GameObject ItemPrefab { get { return itemPrefab; } }

}

[CreateAssetMenu(fileName = "New Item Object", menuName = "InventorySystem/Item/AlienMeat")]
public class AlienMeat : Item
{

}

[CreateAssetMenu(fileName = "New Item Object", menuName = "InventorySystem/Item/GreenGoo")]
public class GreenGoo : Item
{

}

[CreateAssetMenu(fileName = "New Item Object", menuName = "InventorySystem/Item/Metal")]
public class Metal : Item
{

}

[CreateAssetMenu(fileName = "New Item Object", menuName = "InventorySystem/Item/CookedAlienMeat")]
public class CookedAlienMeat : Item
{

}

[CreateAssetMenu(fileName = "New Item Object", menuName = "InventorySystem/Item/ReviveBadge")]
public class ReviveBadge : Item
{

}
