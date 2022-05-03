using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PickingUp : MonoBehaviourPunCallbacks
{
    private Transform camera;
    [SerializeField] private float pickUpDistence = 2;
    [SerializeField] private LayerMask pickupLayer;

    [SerializeField] private LayerMask fireLayer;

    [SerializeField] private LayerMask spaceShipLayer;
    [SerializeField] private Inventory inventory;
    [SerializeField] private HealthState healthState;
    private GameObject otherPlayer;
    public Handler handler;
    private RaycastHit pickup;

    // Start is called before the first frame update
    void Start()
    {
        camera = GameObject.FindGameObjectWithTag("MainCamera").transform;
    }

    // Update is called once per frame
    void Update()
    {
        //if (photonView.IsMine)
        //{
        if (Physics.Raycast(camera.position,
        camera.TransformDirection(Vector3.forward),
        out pickup,
        pickUpDistence,
        pickupLayer))
        {
            Pickup_Typs.Pickup typ = pickup.collider.gameObject.GetComponent<Pickup>().getTyp();
            if (Input.GetKey(KeyCode.E))
            {
                    
                if (typ == Pickup_Typs.Pickup.Metal)
                {
                    inventory.addMetal(pickup.transform.gameObject.GetComponent<Pickup>().amount);
                    //pickup.transform.gameObject.GetComponent<Pickup>().ObjectDestory();
                    //PhotonNetwork.Destroy(pickup.transform.gameObject);
                    Destroy(pickup.transform.gameObject);
                    pickup.transform.gameObject.GetComponent<PhotonView>().RPC("ObjectDestory", RpcTarget.All);
                        
                    //photonView.RPC("ObjectDestory", RpcTarget.All, pickup.transform.gameObject.GetComponent<PhotonView>().ViewID);
                }
                else if (typ == Pickup_Typs.Pickup.GreenGoo)
                {
                    inventory.addGreenGoo(pickup.transform.gameObject.GetComponent<Pickup>().amount);
                    //PhotonNetwork.Destroy(pickup.transform.gameObject);
                    Destroy(pickup.transform.gameObject);
                    pickup.transform.gameObject.GetComponent<PhotonView>().RPC("ObjectDestory", RpcTarget.All);
                }
                else if (typ == Pickup_Typs.Pickup.AlienMeat)
                {
                    inventory.addAlienMeat(pickup.transform.gameObject.GetComponent<Pickup>().amount);
                    //PhotonNetwork.Destroy(pickup.transform.gameObject);
                    Destroy(pickup.transform.gameObject);
                    pickup.transform.gameObject.GetComponent<PhotonView>().RPC("ObjectDestory", RpcTarget.All);
                }
                else if (typ == Pickup_Typs.Pickup.Revive)
                {
                    inventory.HasReviveBadge = true;
                    otherPlayer = pickup.transform.gameObject.GetComponent<Pickup>().getPlayerToRevive();
                    //PhotonNetwork.Destroy(pickup.transform.gameObject);
                    //photonView.RPC("ObjectDestory", RpcTarget.All, pickup.transform.gameObject);
                    pickup.transform.gameObject.GetComponent<PhotonView>().RPC("ObjectDestory", RpcTarget.All);
                }
                    
            }
                
        }

        if (Physics.Raycast(camera.position,
                camera.TransformDirection(Vector3.forward),
                out pickup,
                pickUpDistence,
                spaceShipLayer))
        {
            if (Input.GetKey(KeyCode.E))
            {
                if (inventory.HasReviveBadge)
                {
                    inventory.HasReviveBadge = false;
                    //otherPlayer.GetComponent<HealthState>().Revive();
                    otherPlayer.GetComponent<PhotonView>().RPC("Revive", RpcTarget.AllViaServer);
                }
            }
        }

        if (Physics.Raycast(camera.position,
            camera.TransformDirection(Vector3.forward),
            out pickup,
            pickUpDistence,
            fireLayer))
            {
                if(Input.GetKey(KeyCode.C)){
                    inventory.cook();
                }
            }
            if (Input.GetKey(KeyCode.X))
            {
                if(inventory.CookedAlienMeat > 0){
                inventory.eat();
                photonView.RPC("AddHealth", RpcTarget.All, 1);
            }
            //}
        }
    }
}