using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

[RequireComponent(typeof(PhotonView))]
public class PooledObjectPhotonView : MonoBehaviourPunCallbacks, IRecycleable
{
    [SerializeField] public PooledObject rootObject;
    public Action customRecyclingFunction;

    public void Recycle()
    {
        photonView.RPC(nameof(RecycleRPC), RpcTarget.All);
    }

    [PunRPC]
    private void RecycleRPC()
    {
        customRecyclingFunction?.Invoke();
    }

    public void UpdateActiveState(bool active)
    {
        photonView.RPC(nameof(UpdateActiveStateRPC), RpcTarget.All, active);
    }

    [PunRPC]
    private void UpdateActiveStateRPC(bool active)
    {
        rootObject.gameObject.SetActive(active);
    }
}
