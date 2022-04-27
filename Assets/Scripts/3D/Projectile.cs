using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Projectile : MonoBehaviourPunCallbacks
{
    [SerializeField] private float timer;
    private float counter;
    public bool IsShot { get; set; }
    public Vector3 Velocity { get; set; }

    private void Start()
    {
        counter = timer;
    }

    private void Update()
    {
        transform.position += Velocity * Time.deltaTime;

        if (IsShot)
        {
            counter -= Time.deltaTime;
            if (counter <= 0f)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }

    [PunRPC]
    public void DestoryProjectile()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
