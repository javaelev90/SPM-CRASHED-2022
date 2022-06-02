using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class StoneProjectile : MonoBehaviourPunCallbacks
{
    [SerializeField] private float timer;
    [SerializeField] private int damageDealer;

    public int DamageDealer { get { return damageDealer; } set { damageDealer = value; } }
    private float counter;

    public bool IsThrown { get; set; }
    // Start is called before the first frame update
    void Start()
    {
        counter = timer;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsThrown)
        {
            counter -= Time.deltaTime;
            if (counter <= 0f)
            {
                PhotonNetwork.Destroy(gameObject);
                Destroy(gameObject);
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

    private void OnCollisionEnter(Collision collision)
    {
        HealthHandler healthHandler;
        if ((healthHandler = collision.gameObject.GetComponent<HealthHandler>()) != null)
        {
            healthHandler.TakeDamage(damageDealer);
        }
    }
}
