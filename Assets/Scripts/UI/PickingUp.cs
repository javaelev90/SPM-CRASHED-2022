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
    [SerializeField] private Inventory inventory;
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
        if (photonView.IsMine)
        {
            GameObject.FindGameObjectWithTag("InventoryHandler").GetComponent<Handler>().inventory = inventory;
        }
        inventorySystem.LoadPrefabs();

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
        if (PickUpHitCheck(pickupLayer) && photonView.IsMine)
        {
            Pickup pickUpComponent = pickup.transform.gameObject.GetComponent<Pickup>();
            Pickup_Typs.Pickup typ = pickUpComponent.getTyp();
            PhotonView pickUpPhotonView = pickup.transform.gameObject.GetComponent<PhotonView>();

            if (typ == Pickup_Typs.Pickup.Metal)
            {
                inventory.addMetal(pickUpComponent.amount);
                inventorySystem.Add<Metal>(pickUpComponent.amount);
                //Destroy(pickup.transform.gameObject);
                pickUpPhotonView.RPC("ObjectDestory", RpcTarget.All);
                source.PlayOneShot(Metal);
            }
            else if (typ == Pickup_Typs.Pickup.GreenGoo)
            {
                inventory.addGreenGoo(pickUpComponent.amount);
                inventorySystem.Add<GreenGoo>(pickUpComponent.amount);
                //Destroy(pickup.transform.gameObject);
                pickUpPhotonView.RPC("ObjectDestory", RpcTarget.All);
                source.PlayOneShot(Goo);
            }
            else if (typ == Pickup_Typs.Pickup.AlienMeat)
            {
                inventory.addAlienMeat(pickUpComponent.amount);
                inventorySystem.Add<AlienMeat>(pickUpComponent.amount);
                //Destroy(pickup.transform.gameObject);
                pickUpPhotonView.RPC("ObjectDestory", RpcTarget.All);
                 source.PlayOneShot(Meat);
            }
            else if (typ == Pickup_Typs.Pickup.Revive)
            {
                inventory.HasReviveBadge = true;
                otherPlayer = pickUpComponent.getPlayerToRevive();
                pickUpPhotonView.RPC("ObjectDestory", RpcTarget.All);
            }
        }
    }

    public void Revive()
    {
        if (PickUpHitCheck(spaceShipLayer))
        {
            if (inventory.HasReviveBadge)
            {
                inventory.HasReviveBadge = false;
                otherPlayer.GetComponent<PhotonView>().RPC("ReviveRPC", RpcTarget.AllViaServer, transform.position);
            }
        }
    }

    public void Cook()
    {
        if (PickUpHitCheck(fireLayer))
        {
            inventory.cook();
        }
    }

    public void Eat()
    {
        if (inventory.CookedAlienMeat > 0)
        {
            inventory.eat();
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
                        inventorySystem.Remove<AlienMeat>();
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
}
