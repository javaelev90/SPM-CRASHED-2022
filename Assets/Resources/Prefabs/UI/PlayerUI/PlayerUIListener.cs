using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using EventCallbacksSystem;
using UnityEngine.InputSystem;

public class PlayerUIListener : MonoBehaviour
{
    [SerializeField] private List<SlotItem> slotItems;
    private Dictionary<Pickup_Typs.Pickup, SlotItem> slots;
    private int selectedIndex;

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
    }

    private void OnDisable()
    {
        EventSystem.Instance.UnregisterListener<UpdateUIAmountsEvent>(UpdateAmounts);
    }

    public void UpdateAmounts(UpdateUIAmountsEvent e)
    {
        foreach (KeyValuePair<Type, int> keyValuePair in e.Amounts)
        {
            if (keyValuePair.Key == typeof(AlienMeat))
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

    public void PreviousItem(InputAction.CallbackContext ctx) // use as previous
    {
        if (ctx.started)
        {
            slotItems[selectedIndex].DeselectItem();
            if (selectedIndex > 0)
                slotItems[--selectedIndex].SelectItem();

            if (selectedIndex == 0)
                slotItems[selectedIndex].SelectItem();

        }

        TypeToInventoryEvent te = new TypeToInventoryEvent(slotItems[selectedIndex].PickupType);
        EventSystem.Instance.FireEvent(te);
    }

    public void NextItem(InputAction.CallbackContext ctx) // use as next selected
    {
        if (ctx.started)
        {
            slotItems[selectedIndex].DeselectItem();

            if (selectedIndex < slotItems.Count)
                slotItems[++selectedIndex].SelectItem();

        }

        TypeToInventoryEvent te = new TypeToInventoryEvent(slotItems[selectedIndex].PickupType);
        EventSystem.Instance.FireEvent(te);
    }


}
