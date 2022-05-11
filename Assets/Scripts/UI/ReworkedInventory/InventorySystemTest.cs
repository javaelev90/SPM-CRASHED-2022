using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySystemTest : MonoBehaviour
{
    [SerializeField] private InventorySystem inventory;


    [ContextMenu("Testing inventory")]
    public void TestingInventory()
    {
        inventory.Add<AlienMeat>(5);
        inventory.Add<Metal>(19);

        Debug.Log(inventory.AvailableAmount<AlienMeat>());
        Debug.Log(inventory.AvailableAmount<Metal>());

    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
