using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using EventCallbacksSystem;
using System;

public abstract class HealthHandler : MonoBehaviourPunCallbacks
{
    [Header("Required components")]
    [SerializeField] private HealthBarHandler healthBarHandler;

    protected AudioSource source;
    public AudioClip hit;

    [Header("Item drop")]
    [SerializeField] protected GameObject itemDropPrefab;
    [SerializeField] private float dropOffsetY = 1f;

    [Header("Health")] // Keep these public to enable pooled object recycle functionality
    public int MaxHealth;
    public int CurrentHealth;
    public bool isAlive = true;

    [Header("Lava Damage")]
    private bool inLava;
    public int damage = 5;
    public float intervale = 1.5f;


    public abstract void TakeDamage(int amount);
    public abstract void Die();

    public virtual void DropItem() {}

    public override void OnEnable()
    {
        base.OnEnable();
        ResetHealth();
        source = GetComponent<AudioSource>();
        inLava = false;
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
    public void AddHealthRPC(int amount)
    {
        if (CurrentHealth + amount > MaxHealth)
        {
            CurrentHealth = MaxHealth;

        }
        else
        {
            CurrentHealth += amount;
           
        }
        UpdateHealthBar();
    }

    public void RemoveHealth(int amount)
    {
        CurrentHealth -= amount;
        
        if (CurrentHealth <= 0)
        {
            isAlive = false;
            Die();
        }
        UpdateHealthBar();
    }

    public void SetHealth(int health)
    {
        CurrentHealth = health;
        UpdateHealthBar();
    }

    [PunRPC]
    protected void SetHealthRPC(int health)
    {
        SetHealth(health);
    }

    protected void UpdateHealthBar()
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
        PhotonNetwork.InstantiateRoomObject(GlobalSettings.PickupsPath + itemDropPrefab.name, spawnPosition, transform.rotation, 0, parameters);
    }

    public virtual void IsInLava(bool inLava)
    {
        this.inLava = inLava;
        if (inLava)
            StartCoroutine(IsInLava());
    }

    private IEnumerator IsInLava()
    {
        TakeDamage(damage);
        yield return new WaitForSeconds(intervale);
        if (inLava)
            StartCoroutine(IsInLava());
    }
}
