using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventCallbacksSystem;
using System.Linq;
using UnityEngine.UI;
public class Ship : MonoBehaviour
{
    [SerializeField] private ShipUpgradePanel shipUpgradePanel;
    [SerializeField] private Button shipUpgradeButton;
    [SerializeField] private GameObject playerUpgradePanal;
    [SerializeField] private float radius = 10f;
    private bool triggerActive = false;
    [SerializeField] Engineer player;
    public int nextUpgrade;
    public bool allShipPartsCollected = false;
    public List<ShipUpgradeCost> shipUpgradeCost;
    AudioSource source;
    public AudioClip connect;
    public float minTimeUntilDaw = 120f;
    private InventorySystem inventory;



    [Serializable]
    public class ShipUpgradeCost
    {
        public int metalCost;
        public int gooCost;
        public bool partAvalibul = false;
        public GameObject partMissing;
        public GameObject partAttached;
    }

    private void Start()
    {
        Minimap.Instance.Ship = gameObject;
        EventSystem.Instance.RegisterListener<AttachPartEvent>(newPartObtained);
        EventSystem.Instance.RegisterListener<ShipUppgradPanelEvent>(OpenShipUpgradePanel);
        nextUpgrade = 0;

        source = GetComponent<AudioSource>();
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
        if (TakeResources())
        {
            shipUpgradeCost[nextUpgrade].partMissing.SetActive(false);
            shipUpgradeCost[nextUpgrade].partAttached.SetActive(true);
            nextUpgrade++;
            source.PlayOneShot(connect);
            shipUpgradePanel.gameObject.SetActive(false);
            playerUpgradePanal.SetActive(true);
            allShipPartsCollected = nextUpgrade == shipUpgradeCost.Count;
            return true;
        }
        return false;
    }

    private bool TakeResources()
    {
        if (inventory.Amount<GreenGoo>() >= shipUpgradeCost[nextUpgrade].gooCost && inventory.Amount<Metal>() >= shipUpgradeCost[nextUpgrade].metalCost && shipUpgradeCost[nextUpgrade].partAvalibul)
        {
            inventory.Remove<Metal>(shipUpgradeCost[nextUpgrade].metalCost);
            inventory.Remove<GreenGoo>(shipUpgradeCost[nextUpgrade].gooCost);
            return true;
        }

        return false;
    }

    public void TestUpgrade()
    {
        if (!UppgradeShip())
        {
            shipUpgradePanel.ToggleErrorMessage(true);
            shipUpgradePanel.SetErrorMessage("Something fucked up!");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    private void OpenShipUpgradePanel()
    {
        if (shipUpgradePanel != null)
        {
            inventory = GameManager.playerObject.GetComponent<InventorySystem>();
            shipUpgradePanel.gameObject.SetActive(true);
            if (!shipUpgradeCost[nextUpgrade].partAvalibul)
            {
                shipUpgradeButton.interactable = false;
                shipUpgradePanel.SetCostInfo("No new ship part obtained for upgrade");
            }
            else if (inventory.Amount<Metal>() < shipUpgradeCost[nextUpgrade].metalCost || inventory.Amount<GreenGoo>() < shipUpgradeCost[nextUpgrade].gooCost)
            {
                shipUpgradeButton.interactable = false;
                shipUpgradePanel.SetCostInfo($"Not enough resources \n Metal: {shipUpgradeCost[nextUpgrade].metalCost} \n Green Goo: {shipUpgradeCost[nextUpgrade].gooCost}");
            }
            else
            {
                shipUpgradeButton.interactable = true;
                shipUpgradePanel.SetCostInfo($"Do you want to upgrade? \n Metal: {shipUpgradeCost[nextUpgrade].metalCost} \n Green Goo: {shipUpgradeCost[nextUpgrade].gooCost}");
            }
            shipUpgradePanel.ToggleErrorMessage(false);
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void OpenShipUpgradePanel(ShipUppgradPanelEvent shipUppgradPanelEvent)
    {
        OpenShipUpgradePanel();
    }

    private void OpenPlayerUpgradePanel()
    {
        EventSystem.Instance.FireEvent(new OpenPlayerUpgradePanelEvent());
    }
}