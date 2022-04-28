using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EnemyCharacter : MonoBehaviourPunCallbacks
{
    [Header("Health")]
    [SerializeField] private int maxHealth;
    [SerializeField] private int currentHealth;
    [SerializeField] private HealthHandler healthHandler;

    [Header("Damage")]
    [SerializeField] private int damage;
    [SerializeField] private float explosionRadius;
    [SerializeField] private LayerMask layersToHit;
    [SerializeField] private GameObject explosionObject;
    public int Damage { get { return damage; } }
    public float ExplosionRadius { get { return explosionRadius; } }
    public LayerMask LayersToHit { get { return layersToHit; } }

    public void Die()
    {
        photonView.RPC(nameof(DieRPC), RpcTarget.All);
    }

    // Start is called before the first frame update
    void Start()
    {
        healthHandler.MaxHealth = maxHealth;
        healthHandler.CurrentHealth = currentHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [PunRPC]
    private void DieRPC()
    {
        GetComponent<AIMovement>().BlowUp();
        explosionObject.transform.parent = null;
        explosionObject.GetComponent<ParticleSystem>().Play();
        gameObject.SetActive(false);
    }
}
