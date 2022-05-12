using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventCallbacksSystem;
using System.Linq;

public class Ship : MonoBehaviour
{
    [SerializeField] private UpgradePanel Panel;
    [SerializeField] private float radius = 10f;
    private bool triggerActive = false;
    [SerializeField] Engineer player;
    private int nextUpgrade;
    public bool allShipPartsCollected = false;
    public List<ShipUpgradeCost> shipUpgradeCost;
    public float minTimeUntilDaw = 120f;

    [Serializable]
    public class ShipUpgradeCost
    {
        public int metalCost;
        public int gooCost;
        public bool partAvalibul = false;
        public GameObject partMissing;
        public GameObject partAttached;
    }

    void Start()
    {
        EventSystem.Instance.RegisterListener<AttachPartEvent>(newPartObtained);
        nextUpgrade = 0;
        StartCoroutine(Wait(5));
        //Wait(5);
    }

    IEnumerator Wait(float sec)
    {
        while (player == null)
        {
            yield return new WaitForSeconds(sec);
            player = FindObjectOfType<Engineer>();
        }

    }

    private void Update()
    {
        if(player != null)
        {
            Collider[] colliderHits = Physics.OverlapSphere(transform.position, radius);

            foreach (Collider col in colliderHits)
            {
                if (col.transform.gameObject.GetComponent<Engineer>())
                {
                    if (player && player.playerActions.Player.PickUp.IsPressed() && Panel != null && Panel.gameObject.activeSelf == false && shipUpgradeCost[nextUpgrade].partAvalibul)
                    {
                        OpenUpgradePanel();
                    }
                }
            }
        }

    }

    public void newPartObtained(AttachPartEvent attachPartEvent)
    {
        foreach (ShipUpgradeCost shipUpgradeCost in shipUpgradeCost)
        {
            if (!shipUpgradeCost.partAvalibul)
            {
                shipUpgradeCost.partAvalibul = true;
                shipUpgradeCost.partMissing = attachPartEvent.MissingPart;
                shipUpgradeCost.partAttached = attachPartEvent.AttachedPart;
                break;
            }
        }
        EventSystem.Instance.FireEvent(new ShipPartEvent(minTimeUntilDaw));
    }

    

    public bool UppgradeShip()
    {
        if (shipUpgradeCost[nextUpgrade].partAvalibul)
        {
            shipUpgradeCost[nextUpgrade].partMissing.SetActive(false);
            shipUpgradeCost[nextUpgrade].partAttached.SetActive(true);
            nextUpgrade++;           
            OpenUpgradePanel();
            allShipPartsCollected = nextUpgrade == shipUpgradeCost.Count;
            return true;
        }
        return false;
    }

    private bool TakeResources()
    {
        if (player != null)
        {
            Inventory inventory = player.gameObject.GetComponent<Inventory>();
            if (inventory.GreenGoo >= shipUpgradeCost[nextUpgrade].gooCost && inventory.Metal >= shipUpgradeCost[nextUpgrade].metalCost)
            {
                return inventory.removeMetalAndGreenGoo(shipUpgradeCost[nextUpgrade].metalCost, shipUpgradeCost[nextUpgrade].gooCost);
            }
        }
        return false;
    }

    public void TestUpgrade()
    {
        if (TakeResources() == false)
        {
            Panel.ToggleErrorMessage(true);
            Panel.SetErrorMessage($"Too few resources to upgrade.\n Requires metal: {shipUpgradeCost[nextUpgrade].metalCost}, green goo: {shipUpgradeCost[nextUpgrade].gooCost}");
        }
        else
        {
            UppgradeShip();
            Panel.ClosePanel();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    public void OpenUpgradePanel()
    {
        if(Panel != null)
        {
            bool isActive = Panel.gameObject.activeSelf;
            Panel.gameObject.SetActive(!isActive);
            Panel.ToggleErrorMessage(false);
            Cursor.lockState = CursorLockMode.None;
        }
    }
}