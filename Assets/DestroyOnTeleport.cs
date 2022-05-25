using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventCallbacksSystem;
using Photon.Pun;
using System;

public class DestroyOnTeleport : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        EventSystem.Instance.RegisterListener<TeleportToShipEvent>(DestroyOnTP);
    }

    public void DestroyOnTP(TeleportToShipEvent teleportToShipEvent)
    {
        if (PhotonNetwork.IsMasterClient)
            photonView.RPC(nameof(DestroyOnTPRPC), RpcTarget.All);
    }

    [PunRPC]
    private void DestroyOnTPRPC()
    {
        PhotonNetwork.Destroy(gameObject);
    }
}
