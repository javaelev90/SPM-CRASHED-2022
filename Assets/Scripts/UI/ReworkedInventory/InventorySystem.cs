using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using EventCallbacksSystem;

[CreateAssetMenu(fileName = "New Inventory Object", menuName = "InventorySystem/Inventory")]
public class InventorySystem : ScriptableObject
{
    [SerializeField] private int alienMeatAmount = 10;
    [SerializeField] private int metalAmount = 10;
    [SerializeField] private int greenGoo = 10;

    // dictionary for prefabs
    private Dictionary<Type, GameObject> prefabs;

    // dictionary for amounts
    private Dictionary<Type, int> amounts = new Dictionary<Type, int>();
    private UpdateUIAmountsEvent uiEvent = new UpdateUIAmountsEvent();

    private void Awake()
    {
        if (prefabs != null)
        {
            Debug.Log("prefabs " + prefabs.Count);
        }
    }

    // add item
    public bool Add<T>(int amount = 1) where T : Item
    {
        return AddAmount<T>(amount);
    }

    private bool AddAmount<T>(int amount = 1) where T : Item
    {
        Type keyType = typeof(T);
        if (amounts.ContainsKey(keyType))
        {
            amounts[keyType] += amount;
            uiEvent.Amounts = amounts;
            EventSystem.Instance.FireEvent(uiEvent);
            return true;
        }
        return false;
    }

    // remove item
    public bool Remove<T>(int amount = 1) where T : Item
    {
        return RemoveAmount<T>(amount);
    }

    private bool RemoveAmount<T>(int amount) where T : Item
    {
        Type keyType = typeof(T);
        int availableAmount = AvailableAmount<T>();
        if (availableAmount != -1 && availableAmount - amount >= 0)
        {
            amounts[keyType] -= amount;
            uiEvent.Amounts = amounts;
            EventSystem.Instance.FireEvent(uiEvent);
            return true;
        }
        return false;
    }

    public int Amount<T>() where T : Item
    {
        return AvailableAmount<T>();
    }

    // get item
    private int AvailableAmount<T>() where T : Item
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
        if (prefabs.ContainsKey(keyType) && AvailableAmount<T>() != -1)
        {
            return prefabs[keyType].gameObject;
        }
        return new GameObject("EmptyObject");
    }

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
                    amounts.Add(keyType, 0);
                }
            }
        }

        uiEvent.Amounts = amounts;
        EventSystem.Instance.FireEvent(uiEvent);

#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
    }
}
