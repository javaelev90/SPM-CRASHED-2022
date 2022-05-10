using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;


[CreateAssetMenu(fileName = "New Inventory Object", menuName = "InventorySystem/Inventory")]
public class InventorySystem : ScriptableObject
{
    // dictionary for ability
    // dictionary for attributes

    // dictionary for prefabs
    private Dictionary<Type, GameObject> prefabs;

    // dictionary for amounts
    private Dictionary<Type, int> amounts = new Dictionary<Type, int>();

    // add item
    public void Add<T>(int amount) where T : Item
    {
        if (amounts.ContainsKey(typeof(T)))
        {
            amounts[typeof(T)]++;
        }
    }

    // remove item
    public void Remove<T>(int amount) where T : Item
    {
        if (amounts.ContainsKey(typeof(T)))
        {
            int availableAmount = amounts[typeof(T)];
            amounts[typeof(T)] = availableAmount > 0 ? amounts[typeof(T)]-- : availableAmount;
        }
    }

    // get item
    public int AvailableAmount<T>() where T : Item
    {
        if (amounts.ContainsKey(typeof(T)))
        {
            return amounts[typeof(T)];
        }
        return -1;
    }

    // cook item
    public void Cook() { }
    // drop item
    // upgrade ability
    // add attribute
    // update attribute


    [ContextMenu("LoadPrefabsToInventory")]
    public void LoadPrefabs()
    {
        if (prefabs == null)
        {
            prefabs = new Dictionary<Type, GameObject>();
        }


        if (Directory.Exists("Assets/Resources/Prefabs/Pickups"))
        {
            Debug.Log("Folder exists");
            Item[] items = Resources.FindObjectsOfTypeAll<Item>();
            if (items != null)
            {
                Debug.Log("Number of items: " + items.Length);

                foreach (Item i in items)
                {
                    if (!prefabs.ContainsKey(i.GetType()))
                    {
                        prefabs.Add(i.GetType(), i.ItemPrefab);
                    }
                }

            }
        }

        if(prefabs.Count > 0)
        {
            foreach(KeyValuePair<Type, GameObject> keyValuePair in prefabs)
            {
                Debug.Log(keyValuePair.Value.name);
            }
        }
    }
}
