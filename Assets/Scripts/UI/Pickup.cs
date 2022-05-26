using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Pickup : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback
{
    [SerializeField] private Pickup_Typs.Pickup typ;
    [SerializeField] private GameObject playerToRevive;
    public int amount;
    [SerializeField] public AudioSource source;
    [SerializeField] public AudioClip clip;

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();

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
    public void ObjectDestroy()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] instantiationData = info.photonView.InstantiationData;
        if (instantiationData != null && instantiationData.Length > 0)
        {
            int photonViewID = (int)instantiationData[0];
            playerToRevive = PhotonView.Find(photonViewID).gameObject;
        }
    }
}