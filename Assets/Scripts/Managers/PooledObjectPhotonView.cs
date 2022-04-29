using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PooledObjectPhotonView : MonoBehaviourPunCallbacks
{
    [SerializeField] public GameObject rootObject;

    [PunRPC]
    private void UpdateActiveState(bool active)
    {
        rootObject.SetActive(active);
    }
}
