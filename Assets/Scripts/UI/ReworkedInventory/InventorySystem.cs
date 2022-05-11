using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;


[CreateAssetMenu(fileName = "New Inventory Object", menuName = "InventorySystem/Inventory")]
public class InventorySystem : ScriptableObject
{
    [SerializeField] private int alienMeatAmount = 10;
    [SerializeField] private int metalAmount = 10;


    // dictionary for ability
    // dictionary for attributes

    // dictionary for prefabs
    private Dictionary<Type, GameObject> prefabs;

    // dictionary for amounts
    private Dictionary<Type, int> amounts = new Dictionary<Type, int>();

    private void Awake()
    {
        if (prefabs != null)
        {
            Debug.Log("prefabs " + prefabs.Count);
        }
    }

    // add item
    public bool Add<T>(int amount) where T : Item
    {
        Type keyType = typeof(T);
        if (amounts.ContainsKey(keyType))
        {
            amounts[keyType] += amount;
            Debug.Log("Added " + amount + ": " + keyType);
            return true;
        }
        return false;
    }

    // remove item
    public bool Remove<T>(int amount) where T : Item
    {
        Type keyType = typeof(T);
        if (amounts.ContainsKey(keyType))
        {
            int availableAmount = amounts[keyType];
            amounts[keyType] = availableAmount - amount >= 0 ? amounts[keyType] -= amount : availableAmount;
            return true;
        }
        return false;
    }

    // get item
    public int AvailableAmount<T>() where T : Item
    {
        Type keyType = typeof(T);
        if (amounts.ContainsKey(keyType))
        {
            return amounts[keyType];
        }
        return -1;
    }

    // get item prefab to drop 
    public GameObject ItemPrefab<T>() where T : Item
    {
        Type keyType = typeof(T);
        if (prefabs.ContainsKey(keyType) && amounts[keyType] > 0)
        {
            return prefabs[keyType].gameObject;
        }
        return new GameObject("EmptyObject");
    }

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
            //Item[] items = Resources.FindObjectsOfTypeAll<Item>();
            System.Object[] os = Resources.LoadAll("Prefabs/Pickups", typeof(Item));
            
            if (os.Length > 0)
            {
                Debug.Log("Number of items: " + os.Length);
                
                foreach (System.Object i in os)
                {
                    Item item = (Item)i;
                    if (!prefabs.ContainsKey(item.GetType()))
                    {
                        prefabs.Add(item.GetType(), item.ItemPrefab);
                    }
                }

            }
        }

        if (prefabs.Count > 0)
        {
            foreach (KeyValuePair<Type, GameObject> keyValuePair in prefabs)
            {
                Type keyType = keyValuePair.Key;

                if (!amounts.ContainsKey(keyType))
                {
                    amounts.Add(keyType, 10);
                }
            }
        }

#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
    }
}
