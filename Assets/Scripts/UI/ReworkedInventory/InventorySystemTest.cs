using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySystemTest : MonoBehaviour
{
    [SerializeField] private InventorySystem inventory;


    [ContextMenu("Testing inventory")]
    public void TestingInventory()
    {
        Debug.Log("Available amount AlienMeat: " + inventory.AvailableAmount<AlienMeat>());
        Debug.Log("Available amount GreenGoo: " + inventory.AvailableAmount<GreenGoo>());
        Debug.Log("Available amount Metal: "  + inventory.AvailableAmount<Metal>());
        Debug.Log("Available amount ReviveBadge: " + inventory.AvailableAmount<ReviveBadge>());
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
