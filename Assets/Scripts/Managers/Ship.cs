using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventCallbacksSystem;
using System.Linq;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.InputSystem;

public class Ship : MonoBehaviourPunCallbacks
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
    private ShipUpgradeProgressionEvent progressionEvent;
    private bool initialized = false;

    public CapsuleCollider caps;

    public bool hasObtained; 

    public GameObject uiObject;

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
        Initialize();
    }

    public void Initialize()
    {
        if (initialized == false)
        {
            Minimap.Instance.Ship = gameObject;
            EventSystem.Instance.RegisterListener<AttachPartEvent>(newPartObtained);
            EventSystem.Instance.RegisterListener<ShipUppgradPanelEvent>(OpenShipUpgradePanel);
            nextUpgrade = 0;
            progressionEvent = new ShipUpgradeProgressionEvent(nextUpgrade, shipUpgradeCost.Count);
            source = GetComponent<AudioSource>();
            EventSystem.Instance.FireEvent(progressionEvent);
            initialized = true;
             uiObject.SetActive(false);
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
                hasObtained = true;
                uiObject.SetActive(true);
                break;
            }
        }
        EventSystem.Instance.FireEvent(new ShipPartEvent(minTimeUntilDaw));
    }

 /*  private void OnTriggerEnter(Collider collider) {
      
        if(player.gameObject.Equals(GameManager.player) && hasObtained){
              uiObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && UppgradeShip() && caps.isTrigger)
        {
            uiObject.SetActive(false);
        }
    }*/

    public bool UppgradeShip()
    {
        if (TakeResources())
        {
            photonView.RPC(nameof(UpgradeShipRPC), RpcTarget.All);
            hasObtained = false;
            
            return true;
        }
        return false;
    }

    [PunRPC]
    private void UpgradeShipRPC()
    {
        shipUpgradeCost[nextUpgrade].partMissing.SetActive(false);
        shipUpgradeCost[nextUpgrade].partAttached.SetActive(true);
        nextUpgrade++;
        source.PlayOneShot(connect);
        uiObject.SetActive(false);
        shipUpgradePanel.gameObject.SetActive(false);
        OpenPlayerUpgradePanel();
        allShipPartsCollected = nextUpgrade == shipUpgradeCost.Count;
        progressionEvent.UpgradeNumber = nextUpgrade;
        EventSystem.Instance.FireEvent(progressionEvent);
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
            shipUpgradePanel.SetErrorMessage("Not enough resources!");
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
            inventory = GameManager.player.GetComponent<InventorySystem>();
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
                shipUpgradePanel.SetCostInfo($"Do you want to repair the ship? \n Metal: {shipUpgradeCost[nextUpgrade].metalCost} \n Green Goo: {shipUpgradeCost[nextUpgrade].gooCost}");
            }
            shipUpgradePanel.ToggleErrorMessage(false);
            Cursor.lockState = CursorLockMode.None;
            GameManager.player.GetComponent<PlayerInput>().enabled = false;

        }
    }

    public void OpenShipUpgradePanel(ShipUppgradPanelEvent shipUppgradPanelEvent)
    {
        OpenShipUpgradePanel();
    }

    public void OpenPlayerUpgradePanel()
    {
        //photonView.RPC(nameof(OpenUpgradePanelRPC), RpcTarget.All);
        shipUpgradePanel.gameObject.SetActive(false);
        playerUpgradePanal.SetActive(true);
    }
    //[PunRPC]
    //private void OpenUpgradePanelRPC()
    //{
    //    //EventSystem.Instance.FireEvent(new OpenPlayerUpgradePanelEvent());
    //    //OpenPlayerUpgradePanel();
    //    playerUpgradePanal.SetActive(true);
    //}

    /*public void OpenPlayerUpgradePanel()
    {
        playerUpgradePanal.SetActive(true);
    }*/
}