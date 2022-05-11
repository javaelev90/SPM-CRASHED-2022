using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

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



    private Transform mainCamera;
    private GameObject otherPlayer;
    private RaycastHit pickup;

    void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").transform;
        source = GetComponent<AudioSource>();
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
                inventory.addMetal(pickUpComponent.amount);

                //Destroy(pickup.transform.gameObject);
                pickUpPhotonView.RPC("ObjectDestory", RpcTarget.All);
                source.PlayOneShot(Metal);
            }
            else if (typ == Pickup_Typs.Pickup.GreenGoo)
            {
                inventory.addGreenGoo(pickUpComponent.amount);
                //Destroy(pickup.transform.gameObject);
                pickUpPhotonView.RPC("ObjectDestory", RpcTarget.All);
                source.PlayOneShot(Goo);
            }
            else if (typ == Pickup_Typs.Pickup.AlienMeat)
            {
                inventory.addAlienMeat(pickUpComponent.amount);
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
        photonView.RPC(nameof(DropItemRPC), RpcTarget.MasterClient);
    }

    [PunRPC]
    private void DropItemRPC()
    {
        if (PhotonNetwork.IsMasterClient)
        {

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
