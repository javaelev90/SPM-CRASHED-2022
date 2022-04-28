using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCharacter : MonoBehaviour
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
        GetComponent<AIMovement>().BlowUp();
        explosionObject.transform.parent = null;
        explosionObject.GetComponent<ParticleSystem>().Play();
        gameObject.SetActive(false);
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
}
