using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventCallbacksSystem;
using System.Linq;
using UnityEngine.UI;
public class Ship : MonoBehaviour
{
    [SerializeField] private UpgradePanel Panel;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private float radius = 10f;
    private bool triggerActive = false;
    [SerializeField] Engineer player;
    private int nextUpgrade;
    public bool allShipPartsCollected = false;
    public List<ShipUpgradeCost> shipUpgradeCost;
    AudioSource source;
    public AudioClip connect;
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
        source = GetComponent<AudioSource>();
        

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
                    if (player && player.playerActions.Player.PickUp.IsPressed() && Panel != null && Panel.gameObject.activeSelf == false)
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
        if (TakeResources())
        {
            shipUpgradeCost[nextUpgrade].partMissing.SetActive(false);
            shipUpgradeCost[nextUpgrade].partAttached.SetActive(true);
            nextUpgrade++;           
            source.PlayOneShot(connect);
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
            InventorySystem inventory = player.gameObject.GetComponent<InventorySystem>();
            if (inventory.Amount<GreenGoo>() >= shipUpgradeCost[nextUpgrade].gooCost && inventory.Amount<Metal>() >= shipUpgradeCost[nextUpgrade].metalCost && shipUpgradeCost[nextUpgrade].partAvalibul)
            {
                inventory.Remove<Metal>(shipUpgradeCost[nextUpgrade].metalCost);
                inventory.Remove<GreenGoo>(shipUpgradeCost[nextUpgrade].gooCost);
                return true;
            }
        }
        return false;
    }

    public void TestUpgrade()
    {
        if (!UppgradeShip())
        {
            Panel.ToggleErrorMessage(true);
            Panel.SetErrorMessage("Something fucked up!");
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
            InventorySystem inventory = player.gameObject.GetComponent<InventorySystem>();
            bool isActive = Panel.gameObject.activeSelf;
            Panel.gameObject.SetActive(!isActive);
            if (!shipUpgradeCost[nextUpgrade].partAvalibul)
            {
                upgradeButton.interactable = false;
                Panel.SetCostInfo("No new ship part obtained for upgrade");
            }
            else if (inventory.Amount<Metal>() < shipUpgradeCost[nextUpgrade].metalCost || inventory.Amount<GreenGoo>() < shipUpgradeCost[nextUpgrade].gooCost)
            {
                upgradeButton.interactable = false;
                Panel.SetCostInfo("Not enough resources");
            }
            else
            {
                upgradeButton.interactable = true;
                Panel.SetCostInfo($"Do you want to upgrade? \n Metal: {shipUpgradeCost[nextUpgrade].metalCost} \n Green Goo: {shipUpgradeCost[nextUpgrade].gooCost}");
            }
            Panel.ToggleErrorMessage(false);
            Cursor.lockState = CursorLockMode.None;
        }
    }
}