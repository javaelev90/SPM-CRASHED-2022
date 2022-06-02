using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using EventCallbacksSystem;
using UnityEngine.InputSystem;


public class PickingUp : MonoBehaviourPunCallbacks
{
    [Header("Interaction layers")]
    [SerializeField] private LayerMask pickupLayer;
    [SerializeField] private LayerMask spaceShipLayer;
    [SerializeField] private LayerMask shipPartLayer;
    [SerializeField] private LayerMask reviveLayer;

    [SerializeField] private float pickUpDistance = 3;
    [SerializeField] public AudioSource source;
    [SerializeField] public AudioClip Goo;
    [SerializeField] public AudioClip Metal;
    [SerializeField] public AudioClip Meat;

    [SerializeField] public AudioClip ReviveSound;


    [SerializeField] private InventorySystem inventorySystem;
    [SerializeField] private Transform dropTransform;
    [SerializeField] private float timeToDrop = 0.5f;
    [SerializeField] private float timeToEatCooldown = 0.5f;
    [SerializeField] private int eatHealthRestore = 10;
    private float timeToDropCounter;
    private float timeToEatCounter;
    private bool canDrop;
    private bool canPickUp;
    private Transform mainCamera;
    private GameObject otherPlayer;
    private RaycastHit pickup;
    private Pickup_Typs.Pickup itemTypeToDrop;

    public GameObject uiObject;

     private  static bool textShown; 

     
     private  static bool hasDropped; 

    private void OnEnable()
    {
        EventSystem.Instance.RegisterListener<TypeToInventoryEvent>(ItemTypeToDrop);
    }

    private void OnDisable()
    {
        EventSystem.Instance.UnregisterListener<TypeToInventoryEvent>(ItemTypeToDrop);
    }

    public DialoguePickups dialog;

    void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").transform;
        source = GetComponent<AudioSource>();
         uiObject.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!canDrop || !canPickUp)
        {
            timeToDropCounter += Time.deltaTime;
            if (timeToDropCounter >= timeToDrop)
            {
                canDrop = true;
                canPickUp = true;
                timeToDropCounter = 0f;
            }
        }

        if (timeToEatCounter >= 0)
        {
            timeToEatCounter -= Time.deltaTime;
        }

        ShowDrop();
    }

    public void ItemTypeToDrop(TypeToInventoryEvent e)
    {
        itemTypeToDrop = e.Type;
    }

    public void PickUp()
    {
        if (PickUpHitCheck(pickupLayer) && canPickUp)
        {
            canPickUp = false;
            Pickup pickUpComponent = pickup.transform.gameObject.GetComponent<Pickup>();
            Pickup_Typs.Pickup typ = pickUpComponent.getTyp();
            PhotonView pickUpPhotonView = pickup.transform.gameObject.GetComponent<PhotonView>();

            if (typ == Pickup_Typs.Pickup.Metal)
            {
                inventorySystem.Add<Metal>(pickUpComponent.amount);
                pickUpPhotonView.RPC(nameof(pickUpComponent.ObjectDestroy), RpcTarget.All);
                source.PlayOneShot(Metal);
            }
            else if (typ == Pickup_Typs.Pickup.GreenGoo)
            {
                inventorySystem.Add<GreenGoo>(pickUpComponent.amount);
                pickUpPhotonView.RPC(nameof(pickUpComponent.ObjectDestroy), RpcTarget.All);
                source.PlayOneShot(Goo);
            }
            else if (typ == Pickup_Typs.Pickup.AlienMeat)
            {
                inventorySystem.Add<AlienMeat>(pickUpComponent.amount);
                pickUpPhotonView.RPC(nameof(pickUpComponent.ObjectDestroy), RpcTarget.All);
                source.PlayOneShot(Meat);
            }
            else if (typ == Pickup_Typs.Pickup.Revive)
            {
                inventorySystem.Add<ReviveBadge>();
                otherPlayer = pickUpComponent.getPlayerToRevive();
                pickUpPhotonView.RPC(nameof(pickUpComponent.ObjectDestroy), RpcTarget.All);
                source.PlayOneShot(ReviveSound);
            }
        }
        else if (PickUpHitCheck(spaceShipLayer))
        {
            EventSystem.Instance.FireEvent(new ShipUppgradPanelEvent());
        }
        else if (PickUpHitCheck(shipPartLayer))
        {
            pickup.transform.gameObject.GetComponent<Pickup_ShipPart>().PickUpPart();
        }
    }

    public void Revive()
    {
        if (PickUpHitCheck(reviveLayer))
        {
            if (inventorySystem.Amount<ReviveBadge>() > 0)
            {
                inventorySystem.Remove<ReviveBadge>();
                otherPlayer.GetComponent<PhotonView>().RPC("ReviveRPC", RpcTarget.AllViaServer, transform.position);
            }
        }
    }

    public void Eat()
    {
        if(timeToEatCounter <= 0)
        {
            if (inventorySystem.Amount<AlienMeat>() > 0)
            {
                inventorySystem.Remove<AlienMeat>();
                gameObject.GetComponent<HealthHandler>().AddHealth(eatHealthRestore);
                timeToEatCounter = timeToEatCooldown;
            }
        }
    }

    public void DropItem()
    {
        if (canDrop)
        {
            if (HasAmountToDrop(itemTypeToDrop))
            {
                canDrop = false;
                photonView.RPC(nameof(DropItemRPC), RpcTarget.MasterClient, itemTypeToDrop);
                hasDropped= true;
            }
        }
        
    }

    public void ShowDrop(){
        InventorySystem inventorySystem = gameObject.GetComponent<InventorySystem>();
        if(inventorySystem.Amount<GreenGoo>() >= 3 && inventorySystem.Amount<Metal>() >= 3 && !textShown &&  photonView.IsMine){
            uiObject.SetActive(true);
            textShown = true;
        }
        if(hasDropped){
            uiObject.SetActive(false);
        }
    }

    private bool HasAmountToDrop(Pickup_Typs.Pickup typ)
    {
        bool hasAmount = false;
        switch (typ)
        {
            case Pickup_Typs.Pickup.GreenGoo:
                if (inventorySystem.Amount<GreenGoo>() > 0)
                {
                    inventorySystem.Remove<GreenGoo>();
                    hasAmount = true;
                }
                break;
            case Pickup_Typs.Pickup.Metal:
                if (inventorySystem.Amount<Metal>() > 0)
                {
                    inventorySystem.Remove<Metal>();
                    hasAmount = true;
                }
                break;
            case Pickup_Typs.Pickup.AlienMeat:
                if (inventorySystem.Amount<AlienMeat>() > 0)
                {
                    inventorySystem.Remove<AlienMeat>();
                    hasAmount = true;
                }
                break;
        }
        return hasAmount;
    }

    [PunRPC]
    private void DropItemRPC(Pickup_Typs.Pickup typ)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GameObject go;
            Debug.Log($"item to drop {typ} is engineer {GetComponent<Engineer>() != null}");
            switch (typ)
            {
                case Pickup_Typs.Pickup.GreenGoo:
                    go = inventorySystem.ItemPrefab<GreenGoo>();
                    PhotonNetwork.InstantiateRoomObject(GlobalSettings.PickupsPath + go.name, dropTransform.position, Quaternion.identity);
                    break;
                case Pickup_Typs.Pickup.Metal:
                    go = inventorySystem.ItemPrefab<Metal>();
                    PhotonNetwork.InstantiateRoomObject(GlobalSettings.PickupsPath + go.name, dropTransform.position, Quaternion.identity);
                    break;
                case Pickup_Typs.Pickup.AlienMeat:
                    go = inventorySystem.ItemPrefab<AlienMeat>();
                    PhotonNetwork.InstantiateRoomObject(GlobalSettings.PickupsPath + go.name, dropTransform.position, Quaternion.identity);
                    break;
            }
        }
    }

    private bool PickUpHitCheck(LayerMask layer)
    {
        return Physics.Raycast(mainCamera.position,
                mainCamera.forward,
                out pickup,
                pickUpDistance,
                layer, QueryTriggerInteraction.Ignore);
    }
#if (UNITY_EDITOR)
    public void AddMetalDEV()
    {
        inventorySystem.Add<Metal>(1);
    }

    public void AddGooDEV()
    {
        inventorySystem.Add<GreenGoo>(1);
    }

    public void AddMeatDEV()
    {
        inventorySystem.Add<AlienMeat>(1);
    }
#endif
}
