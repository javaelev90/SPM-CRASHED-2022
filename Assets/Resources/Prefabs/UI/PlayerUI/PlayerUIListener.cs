using System.Collections.Generic;
using UnityEngine;
using System;
using EventCallbacksSystem;
using UnityEngine.InputSystem;

public class PlayerUIListener : MonoBehaviour
{
    [SerializeField] private List<SlotItem> slotItems;
    [SerializeField] private ObjectiveViewer objectiveViewer;
    [SerializeField] private GameObject amountEffect; // stoppa in partikeleffekten här
    private GameObject particleEffectAmount;
    private Dictionary<Pickup_Typs.Pickup, SlotItem> slots;
    private int selectedIndex;
    private bool isShowingObjective = true;

    private void OnEnable()
    {
        EventSystem.Instance.RegisterListener<UpdateUIAmountsEvent>(UpdateAmounts);
        EventSystem.Instance.RegisterListener<ShipUpgradeProgressionEvent>(UpdateShipPartCompleted);
        EventSystem.Instance.RegisterListener<ShipUpgradeProgressionEvent>(InitializeShipParts);

        slots = new Dictionary<Pickup_Typs.Pickup, SlotItem>();
        if (slotItems != null)
        {
            foreach (SlotItem si in slotItems)
            {
                slots.Add(si.PickupType, si);
            }
        }
        slotItems[selectedIndex].SelectItem();
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
                //Destroy(Instantiate(amountEffect, slots[Pickup_Typs.Pickup.AlienMeat].EffectPosition.position, Quaternion.identity), 3f);
                
            }
            if (keyValuePair.Key == typeof(Metal))
            {
                slots[Pickup_Typs.Pickup.Metal].UpdateNumberOfItems(keyValuePair.Value);
                //Destroy(Instantiate(amountEffect, slots[Pickup_Typs.Pickup.Metal].EffectPosition.position, Quaternion.identity), 3f);
                
            }
            if (keyValuePair.Key == typeof(GreenGoo))
            {
                slots[Pickup_Typs.Pickup.GreenGoo].UpdateNumberOfItems(keyValuePair.Value);
                //Destroy(Instantiate(amountEffect, slots[Pickup_Typs.Pickup.GreenGoo].EffectPosition.position, Quaternion.identity), 3f);
                
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
        var vfx = Instantiate(amountEffect, effectTransform.position, Quaternion.identity) as GameObject;
        Debug.Log(vfx.name);
        vfx.transform.SetParent(effectTransform);
        var ps = vfx.GetComponent<ParticleSystem>();
        Destroy(vfx, ps.main.duration + ps.main.startLifetime.constantMax);
    }

    public void PreviousItem(InputAction.CallbackContext ctx) // use as previous
    {
        if (ctx.started)
        {
            slotItems[selectedIndex].DeselectItem();
            selectedIndex--;

            if (selectedIndex >= 0)
                slotItems[selectedIndex].SelectItem();

            if (selectedIndex < 0)
            {
                selectedIndex = slotItems.Count - 1;
                slotItems[selectedIndex].SelectItem();
            }
        }

        TypeToInventoryEvent te = new TypeToInventoryEvent(slotItems[selectedIndex].PickupType);
        EventSystem.Instance.FireEvent(te);
    }

    public void NextItem(InputAction.CallbackContext ctx) // use as next selected
    {
        if (ctx.started)
        {
            slotItems[selectedIndex].DeselectItem();
            selectedIndex++;

            if (selectedIndex <= slotItems.Count - 1)
                slotItems[selectedIndex].SelectItem();

            if (selectedIndex > slotItems.Count - 1)
            {
                selectedIndex = 0;
                slotItems[selectedIndex].SelectItem();
            }

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
