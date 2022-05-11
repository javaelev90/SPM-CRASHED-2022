using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Item : ScriptableObject
{
    [SerializeField] private GameObject itemPrefab;

    [TextArea(20, 20)]
    public string Description;
    public GameObject ItemPrefab { get { return itemPrefab; } }
    
}
