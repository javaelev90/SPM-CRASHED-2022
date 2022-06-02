using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using EventCallbacksSystem;
using Photon.Pun;

public class InventorySystem : MonoBehaviourPunCallbacks
{
    private Dictionary<Type, GameObject> prefabs;
    private Dictionary<Type, int> amounts = new Dictionary<Type, int>();
    private UpdateUIAmountsEvent uiEvent = new UpdateUIAmountsEvent();

    private bool initialized = false;

    private void Awake()
    {
        Initialize();
    }

    public void Initialize()
    {
        if (initialized == false)
        {
            LoadPrefabs();
            initialized = true;
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
            uiEvent.type = keyType;
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
            uiEvent.type = keyType;
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

    // load prefabs into dictionary
    public void LoadPrefabs()
    {
        if (prefabs == null)
        {
            prefabs = new Dictionary<Type, GameObject>();
        }

        if (Directory.Exists(Application.dataPath + GlobalSettings.ResourcesPath))
        {
            System.Object[] os = Resources.LoadAll(GlobalSettings.PickupsPath, typeof(Item));

            if (os.Length > 0)
            {
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
    }
}
