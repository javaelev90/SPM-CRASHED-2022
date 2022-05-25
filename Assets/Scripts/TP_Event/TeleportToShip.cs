using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventCallbacksSystem;
using Photon.Pun;

public class TeleportToShip : MonoBehaviourPunCallbacks
{
    public GameObject playerObject;
    private PlayerHealthHandler playerHealth;

    public Transform soldierTPPosition;
    public Transform engenerTPPosition;

    private void Start()
    {
        EventSystem.Instance.RegisterListener<TeleportToShipEvent>(Teleport);
        playerHealth = playerObject.GetComponent<PlayerHealthHandler>();
    }


    public void Teleport(TeleportToShipEvent teleport)
    {
        photonView.RPC(nameof(TeleportRPC), RpcTarget.All);
    }

    [PunRPC]
    private void TeleportRPC()
    {
        //soldier.transform.position = soldierTPPosition.transform.position;
        playerObject = GameManager.player;

        if (playerObject.GetComponent<Engineer>())
        {
            playerHealth.Revive(engenerTPPosition.position);
        }
        else
        {
            playerHealth.Revive(soldierTPPosition.position);
        }
    }
}
