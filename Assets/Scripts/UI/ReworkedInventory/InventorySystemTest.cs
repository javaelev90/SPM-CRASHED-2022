using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySystemTest : MonoBehaviour
{
    [SerializeField] private InventorySystem inventory;


    [ContextMenu("Testing inventory")]
    public void TestingInventory()
    {
        Debug.Log("Available amount AlienMeat: " + inventory.Amount<AlienMeat>());
        Debug.Log("Available amount GreenGoo: " + inventory.Amount<GreenGoo>());
        Debug.Log("Available amount Metal: "  + inventory.Amount<Metal>());
        Debug.Log("Available amount ReviveBadge: " + inventory.Amount<ReviveBadge>());
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
