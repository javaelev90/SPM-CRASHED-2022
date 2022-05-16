using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using EventCallbacksSystem;

public class PlayerUIListener : MonoBehaviour
{

    [SerializeField] private SlotItem cookedMeat;
    [SerializeField] private SlotItem greenGoo;
    [SerializeField] private bool IsSoldier;
    [SerializeField] private List<SlotItem> slotItems;
    private Dictionary<Pickup_Typs.Pickup, SlotItem> slots;
    private Dictionary<Type, int> uiAmounts = new Dictionary<Type, int>();

    private SlotItem previousSelected;

    private void OnEnable()
    {
        EventSystem.Instance.RegisterListener<UpdateUIAmountsEvent>(UpdateAmounts);
        slots = new Dictionary<Pickup_Typs.Pickup, SlotItem>();
        if (slotItems != null)
        {
            foreach (SlotItem si in slotItems)
            {
                slots.Add(si.PickupType, si);
            }
        }

        if (IsSoldier)
        {
            slots[Pickup_Typs.Pickup.Metal].gameObject.SetActive(false);
        }
        else
        {
            slots[Pickup_Typs.Pickup.AlienMeat].gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        EventSystem.Instance.UnregisterListener<UpdateUIAmountsEvent>(UpdateAmounts);
    }

    public void UpdateAmounts(UpdateUIAmountsEvent e)
    {
        uiAmounts = e.Amounts;

        foreach (KeyValuePair<Type, int> keyValuePair in uiAmounts)
        {
            if(keyValuePair.Key == typeof(AlienMeat))
            {
                slots[Pickup_Typs.Pickup.AlienMeat].UpdateNumberOfItems(keyValuePair.Value);
            }
            if (keyValuePair.Key == typeof(Metal))
            {
                slots[Pickup_Typs.Pickup.Metal].UpdateNumberOfItems(keyValuePair.Value);
            }
            if (keyValuePair.Key == typeof(CookedAlienMeat))
            {
                slots[Pickup_Typs.Pickup.CookedAlienMeat].UpdateNumberOfItems(keyValuePair.Value);
            }
            if (keyValuePair.Key == typeof(GreenGoo))
            {
                slots[Pickup_Typs.Pickup.GreenGoo].UpdateNumberOfItems(keyValuePair.Value);
            }
        }
    }

    public void OnCookedMeatSelected()
    {
        previousSelected = greenGoo;
        cookedMeat.SelectItem();
        previousSelected.DeselectItem();
        previousSelected = cookedMeat;
        TypeToInventoryEvent te = new TypeToInventoryEvent(Pickup_Typs.Pickup.CookedAlienMeat);
        EventSystem.Instance.FireEvent(te);
    }

    public void OnGreenGoSelected()
    {
        previousSelected = cookedMeat;
        greenGoo.SelectItem();
        previousSelected.DeselectItem();
        previousSelected = cookedMeat;
        TypeToInventoryEvent te = new TypeToInventoryEvent(Pickup_Typs.Pickup.GreenGoo);
        EventSystem.Instance.FireEvent(te);
    }


}
