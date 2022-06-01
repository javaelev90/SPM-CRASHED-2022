using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ShowTurret : MonoBehaviourPunCallbacks
{

    public GameObject canvas;
    // Start is called before the first frame update
    void Start()
    {
        canvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(photonView.IsMine){
            canvas.SetActive(true);
        }
    }
}
