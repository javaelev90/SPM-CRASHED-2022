using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Pickup : MonoBehaviourPunCallbacks
{
    [SerializeField] private Pickup_Typs.Pickup typ;
    [SerializeField] private GameObject playerToRevive;
    public int amount;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public Pickup_Typs.Pickup getTyp()
    {
        return typ;
    }

    public GameObject getPlayerToRevive()
    {
        return playerToRevive;
    }

    public void setPlayerToRevive(GameObject player)
    {
        playerToRevive = player;
    }
    [PunRPC]
    public void ObjectDestory()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}