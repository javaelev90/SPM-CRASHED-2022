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
    public bool hasCollided;
    

    void Start()
    {
        counter = timer;
    }


    public void InitializeStone()
    {
        
    }


    // Update is called once per frame
    void Update()
    {
        if (IsThrown)
        {
            counter -= Time.deltaTime;
            if (counter <= 0f)
            {
                DestroyProjectile();
            }
        }
    }

    [PunRPC]
    public void DestroyProjectile()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasCollided == false)
        {
            HealthHandler healthHandler;
            if ((healthHandler = collision.gameObject.GetComponent<HealthHandler>()) != null)
            {
                healthHandler.TakeDamage(damageDealer);
                hasCollided = true;
            }
        }

        DestroyProjectile();
    }
}
