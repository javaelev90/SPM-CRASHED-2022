using System.Collections.Generic;
using UnityEngine;
using System;
using EventCallbacksSystem;
using UnityEngine.InputSystem;

public class PlayerUIListener : MonoBehaviour
{
    [SerializeField] private List<SlotItem> slotItems;
    [SerializeField] private ObjectiveViewer objectiveViewer;
    [SerializeField] private GameObject amountEffect;
    private Dictionary<Pickup_Typs.Pickup, SlotItem> slots;
    private int selectedIndex;
    private bool isShowingObjective = true;

    private void OnEnable()
    {
        slots = new Dictionary<Pickup_Typs.Pickup, SlotItem>();
        if (slotItems != null)
        {
            foreach (SlotItem si in slotItems)
            {
                slots.Add(si.PickupType, si);
            }
        }
        slotItems[selectedIndex].SelectItem();

        EventSystem.Instance.RegisterListener<UpdateUIAmountsEvent>(UpdateAmounts);
        EventSystem.Instance.RegisterListener<ShipUpgradeProgressionEvent>(UpdateShipPartCompleted);
        EventSystem.Instance.RegisterListener<ShipUpgradeProgressionEvent>(InitializeShipParts);
    }

    private void OnDisable()
    {
        EventSystem.Instance.UnregisterListener<UpdateUIAmountsEvent>(UpdateAmounts);
        EventSystem.Instance.UnregisterListener<ShipUpgradeProgressionEvent>(UpdateShipPartCompleted);
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
            if (keyValuePair.Key == typeof(GreenGoo))
            {
                slots[Pickup_Typs.Pickup.GreenGoo].UpdateNumberOfItems(keyValuePair.Value);
            }
        }

        Transform effectTransform = null;
        if (e.type == typeof(GreenGoo))
        {
            effectTransform = slots[Pickup_Typs.Pickup.GreenGoo].EffectPosition;
        }
        else if (e.type == typeof(Metal))
        {
            effectTransform = slots[Pickup_Typs.Pickup.Metal].EffectPosition;
        }
        else if (e.type == typeof(AlienMeat))
        {
            effectTransform = slots[Pickup_Typs.Pickup.AlienMeat].EffectPosition;
        }

        if (effectTransform != null)
        {
            var vfx = Instantiate(amountEffect, effectTransform.position, Quaternion.identity) as GameObject;
            vfx.transform.SetParent(effectTransform);
            var ps = vfx.GetComponent<ParticleDestroyer>();
            Destroy(vfx, ps.DestroyDelay);
        }

    }

    

    public void FirstSlot(InputAction.CallbackContext ctx) // use as previous
    {

        if (ctx.started)
        {
            //slotItems[selectedIndex].DeselectItem();
            //selectedIndex--;

            //if (selectedIndex >= 0)
            //    slotItems[selectedIndex].SelectItem();

            //if (selectedIndex < 0)
            //{
            //    selectedIndex = slotItems.Count - 1;
            //    slotItems[selectedIndex].SelectItem();
            //}

            slotItems[selectedIndex].DeselectItem();
            selectedIndex = 0;
            slotItems[selectedIndex].SelectItem();
        }

        TypeToInventoryEvent te = new TypeToInventoryEvent(slotItems[selectedIndex].PickupType);
        EventSystem.Instance.FireEvent(te);
    }

    public void SecondSlot(InputAction.CallbackContext ctx) // use as next selected
    {
        if (ctx.started)
        {
            slotItems[selectedIndex].DeselectItem();
            selectedIndex = 1;
            slotItems[selectedIndex].SelectItem();
        }

        TypeToInventoryEvent te = new TypeToInventoryEvent(slotItems[selectedIndex].PickupType);
        EventSystem.Instance.FireEvent(te);
    }

    public void ThirdSlot(InputAction.CallbackContext ctx) // use as next selected
    {
        if (ctx.started)
        {
            slotItems[selectedIndex].DeselectItem();
            selectedIndex = 2;
            slotItems[selectedIndex].SelectItem();
        }

        TypeToInventoryEvent te = new TypeToInventoryEvent(slotItems[selectedIndex].PickupType);
        EventSystem.Instance.FireEvent(te);
    }

    public void ObjectiveShow(InputAction.CallbackContext ctx)
    {
        if(ctx.started && isShowingObjective == false)
        {
            DisplayObjectivePanel(true);
            return;
        }

        if (ctx.started && isShowingObjective == true)
        {
            DisplayObjectivePanel(false);
            return;
        }
    }

    private void DisplayObjectivePanel(bool canShow)
    {
        isShowingObjective = canShow;
        objectiveViewer.enabled = true;
        objectiveViewer.IsDisplayingPanel = isShowingObjective;
    }

    public void UpdateShipPartCompleted(ShipUpgradeProgressionEvent ev)
    {
        objectiveViewer.enabled = true;
        objectiveViewer.UpdateUpgradedShipParts(ev.UpgradeNumber);
        objectiveViewer.enabled = false;
    }

    public void InitializeShipParts(ShipUpgradeProgressionEvent ev)
    {
        objectiveViewer.enabled = true;
        objectiveViewer.InitializeShipPartsAmount(ev.UpgradeNumber, ev.TotalNumberOfParts);
        objectiveViewer.enabled = false;
        EventSystem.Instance.UnregisterListener<ShipUpgradeProgressionEvent>(InitializeShipParts);
    }
}
