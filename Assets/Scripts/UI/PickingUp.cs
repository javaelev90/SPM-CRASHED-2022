using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using EventCallbacksSystem;

public class PickingUp : MonoBehaviourPunCallbacks
{
    [Header("Interaction layers")]
    [SerializeField] private LayerMask pickupLayer;
    [SerializeField] private LayerMask fireLayer;
    [SerializeField] private LayerMask spaceShipLayer;

    [SerializeField] private float pickUpDistance = 3;
    [SerializeField] public AudioSource source;
    [SerializeField] public AudioClip Goo;
    [SerializeField] public AudioClip Metal;
    [SerializeField] public AudioClip Meat;


    [SerializeField] private InventorySystem inventorySystem;
    [SerializeField] private Transform dropTransform;
    [SerializeField] private float timeToDrop = 0.5f;
    private float timeToDropCounter;
    private bool canDrop;

    private Transform mainCamera;
    private GameObject otherPlayer;
    private RaycastHit pickup;
    private Pickup_Typs.Pickup itemTypeToDrop;

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
    }

    private void Update()
    {
        if (!canDrop)
        {
            timeToDropCounter += Time.deltaTime;
            if (timeToDropCounter >= timeToDrop)
            {
                canDrop = true;
                timeToDropCounter = 0f;
            }
        }
    }

    public void ItemTypeToDrop(TypeToInventoryEvent e)
    {
        itemTypeToDrop = e.Type;
    }

    public void PickUp()
    {
        if (PickUpHitCheck(pickupLayer))
        {
            Pickup pickUpComponent = pickup.transform.gameObject.GetComponent<Pickup>();
            Pickup_Typs.Pickup typ = pickUpComponent.getTyp();
            PhotonView pickUpPhotonView = pickup.transform.gameObject.GetComponent<PhotonView>();

            if (typ == Pickup_Typs.Pickup.Metal)
            {
                inventorySystem.Add<Metal>(pickUpComponent.amount);
                pickUpPhotonView.RPC("ObjectDestory", RpcTarget.All);
                source.PlayOneShot(Metal);
            }
            else if (typ == Pickup_Typs.Pickup.GreenGoo)
            {
                inventorySystem.Add<GreenGoo>(pickUpComponent.amount);
                pickUpPhotonView.RPC("ObjectDestory", RpcTarget.All);
                source.PlayOneShot(Goo);
            }
            else if (typ == Pickup_Typs.Pickup.AlienMeat)
            {
                inventorySystem.Add<AlienMeat>(pickUpComponent.amount);
                pickUpPhotonView.RPC("ObjectDestory", RpcTarget.All);
                source.PlayOneShot(Meat);
            }
            else if (typ == Pickup_Typs.Pickup.Revive)
            {
                inventorySystem.Add<ReviveBadge>();
                otherPlayer = pickUpComponent.getPlayerToRevive();
                pickUpPhotonView.RPC("ObjectDestory", RpcTarget.All);
            }
        }
        else if (PickUpHitCheck(spaceShipLayer))
        {
            EventSystem.Instance.FireEvent(new ShipUppgradPanelEvent());
        }
    }

    public void Revive()
    {
        if (PickUpHitCheck(spaceShipLayer))
        {
            if (inventorySystem.Amount<ReviveBadge>() > 0)
            {
                inventorySystem.Remove<ReviveBadge>();
                otherPlayer.GetComponent<PhotonView>().RPC("ReviveRPC", RpcTarget.AllViaServer, transform.position);
            }
        }
    }

    //legacy
    public void Cook()
    {
        if (PickUpHitCheck(fireLayer))
        {
        }
    }

    public void Eat()
    {
        if (inventorySystem.Amount<AlienMeat>() > 0)
        {
            inventorySystem.Remove<AlienMeat>();
            photonView.RPC("AddHealth", RpcTarget.All, 1);
        }
    }

    public void DropItem()
    {
        if (canDrop)
            photonView.RPC(nameof(DropItemRPC), RpcTarget.MasterClient);

        canDrop = false;
    }

    [PunRPC]
    private void DropItemRPC()
    {

        if (PhotonNetwork.IsMasterClient)
        {
            GameObject go;
            switch (itemTypeToDrop)
            {
                case Pickup_Typs.Pickup.CookedAlienMeat:
                    if (inventorySystem.Amount<CookedAlienMeat>() > 0)
                    {
                        go = inventorySystem.ItemPrefab<CookedAlienMeat>();
                        PhotonNetwork.InstantiateRoomObject(GlobalSettings.PickupsPath + go.name, dropTransform.position, Quaternion.identity);
                        inventorySystem.Remove<CookedAlienMeat>();
                    }
                    break;
                case Pickup_Typs.Pickup.GreenGoo:
                    if (inventorySystem.Amount<GreenGoo>() > 0)
                    {
                        go = inventorySystem.ItemPrefab<GreenGoo>();
                        PhotonNetwork.InstantiateRoomObject(GlobalSettings.PickupsPath + go.name, dropTransform.position, Quaternion.identity);
                        inventorySystem.Remove<GreenGoo>();
                    }
                    break;

                case Pickup_Typs.Pickup.Metal:
                    if (inventorySystem.Amount<Metal>() > 0)
                    {
                        go = inventorySystem.ItemPrefab<Metal>();
                        PhotonNetwork.InstantiateRoomObject(GlobalSettings.PickupsPath + go.name, dropTransform.position, Quaternion.identity);
                        inventorySystem.Remove<Metal>();
                    }
                    break;
                case Pickup_Typs.Pickup.AlienMeat:
                    if (inventorySystem.Amount<AlienMeat>() > 0)
                    {
                        go = inventorySystem.ItemPrefab<AlienMeat>();
                        PhotonNetwork.InstantiateRoomObject(GlobalSettings.PickupsPath + go.name, dropTransform.position, Quaternion.identity);
                        inventorySystem.Remove<AlienMeat>();
                    }
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
}
