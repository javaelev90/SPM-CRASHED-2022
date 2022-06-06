using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SlotItem : MonoBehaviour
{
    [SerializeField] private GameObject bgSelected;
    [SerializeField] private TextMeshProUGUI numberOfItem;
    [SerializeField] private Pickup_Typs.Pickup pickup;
    [SerializeField] private RectTransform effectPosition;
    public Pickup_Typs.Pickup PickupType { get { return pickup; } }
    public RectTransform EffectPosition { get { return effectPosition; } }
    public void SelectItem()
    {
        bgSelected.SetActive(true);
    }

    public void DeselectItem()
    {
        bgSelected.SetActive(false);
    }

    public void UpdateNumberOfItems(int amount)
    {
        numberOfItem.text = amount.ToString();
    }
}
