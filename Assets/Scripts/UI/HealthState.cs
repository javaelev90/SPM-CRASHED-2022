using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class HealthState : MonoBehaviourPunCallbacks
{
    private bool isMine;
    [Range(1, 100)]
    [SerializeField] public int initialHealth = 10;
    Vector3 startPosition;
    [SerializeField] private bool reviveTestSubject;

    public int Health { get; private set; }

    private void Start()
    {
        Health = initialHealth;
        isMine = photonView.IsMine;
        startPosition = transform.position;
        GameObject.FindGameObjectWithTag("HealthHandler").GetComponent<HealthHandlerTest>().hs = this;
    }

    [PunRPC]
    public void AddHealth(int health)
    {
        if (isMine)
            Health += health;
    }

    [PunRPC]
    public void RemoveHealth(int health)
    {
        //if (isMine)
        //{
        Health -= health;
        //}
            
    }

    [PunRPC]
    public void ResetHealth()
    {
        if (isMine)
            Health = initialHealth;
    }

    [PunRPC]
    public void KillObject()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.InstantiateRoomObject(GlobalSettings.PickupsPath + "ReviveBadge", transform.position, Quaternion.identity, 0, new object[] { GetComponent<PhotonView>().ViewID });
            //badge.GetComponent<Pickup>().setPlayerToRevive(gameObject);
            //Debug.Log(badge.GetComponent<PhotonView>().ViewID);
            /*if(isMine){
                var nextMaster = PhotonNetwork.MasterClient.GetNext();
                badge.GetComponent<PhotonView>().TransferOwnership(nextMaster);
                PhotonNetwork.SetMasterClient(nextMaster);
            }*/
        }
        
        //if(isMine && PhotonNetwork.IsMasterClient){
        //    PhotonNetwork.SetMasterClient(PhotonNetwork.MasterClient.GetNext());
        //}

        Health = 0;
        transform.root.gameObject.SetActive(false);


    }

    [PunRPC]
    public void Revive()
    {
        Debug.Log("Trying to revive");
        transform.root.gameObject.SetActive(true);
        ResetHealth();
        transform.position = startPosition;

    }

    //-- TEMP F�R TEST -- 
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Projectile>())
        {
            photonView.RPC("KillObject", RpcTarget.All);
        }
    }

}