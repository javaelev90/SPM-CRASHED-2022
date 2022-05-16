using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class HealthHandler : MonoBehaviourPunCallbacks
{
    [Header("Required components")]
    [SerializeField] private HealthBarHandler healthBarHandler;

    AudioSource source;
    public AudioClip hit;

    [Header("Item drop")]
    [SerializeField] protected GameObject itemDropPrefab;
    [SerializeField] private float dropOffsetY = 1f;

    [Header("Health")] // Keep these public to enable pooled object recycle functionality
    public int MaxHealth;
    public int CurrentHealth;
    public bool isAlive = true;

    public virtual void TakeDamage(int amount) {}
    public virtual void Die() {}
    public virtual void DropItem() {}

    public override void OnEnable()
    {
        base.OnEnable();
        ResetHealth();
        source = GetComponent<AudioSource>();

    }

    protected void ResetHealth()
    {
        CurrentHealth = MaxHealth;
        isAlive = true;
        UpdateHealthBar();
    }

    public void AddHealth(int amount)
    {
        photonView.RPC(nameof(AddHealthRPC), RpcTarget.All, amount);
    }

    [PunRPC]
    private void AddHealthRPC(int amount)
    {
        if (CurrentHealth + amount > MaxHealth)
        {
            CurrentHealth = MaxHealth;
        }
        else
        {
            CurrentHealth += amount;
        }
    }

    public void RemoveHealth(int amount)
    {
        CurrentHealth -= amount;
        UpdateHealthBar();

        if (CurrentHealth <= 0)
        {
            isAlive = false;
            Die();
        }
    }

    private void UpdateHealthBar()
    {
        if (gameObject.CompareTag("Player"))
        {
            if (photonView.IsMine)
            {
                healthBarHandler.SetHealthBarValue((float)CurrentHealth / MaxHealth);
            }
        }
        else
        {
            healthBarHandler.SetHealthBarValue((float)CurrentHealth / MaxHealth);
        }
    }

    protected void InstantiateRoomObject(object[] parameters)
    {
        Vector3 spawnPosition = new Vector3(transform.position.x, transform.position.y + dropOffsetY, transform.position.z);
        PhotonNetwork.InstantiateRoomObject("Prefabs/Pickups/" + itemDropPrefab.name, spawnPosition, Quaternion.identity, 0, parameters);
    }
}
