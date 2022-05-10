using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class HealthHandler : MonoBehaviourPunCallbacks
{
    [Header("Required components")]
    [SerializeField] private HealthBarHandler healthBarHandler;
    [Header("If part of pooled object")]
    [SerializeField] private PooledObject rootObject;

    [Header("Health")] // Keep these public to enable pooled object recycle functionality
    public int MaxHealth;
    public int CurrentHealth;
    public bool isAlive = true;
    [SerializeField] private bool isEnemy;
    [SerializeField] private bool dropsMeat;
    [SerializeField] private float dropOffsetY = 1f;

    public void TakeDamage(int amount)
    {
        photonView.RPC(nameof(TakeDamageRPC), RpcTarget.All, amount);     
    }

    public override void OnEnable()
    {
        base.OnEnable();
        //ResetHealth();
    }

    private void ResetHealth()
    {
        CurrentHealth = MaxHealth;
        isAlive = true;
    }

    [PunRPC]
    private void TakeDamageRPC(int amount)
    {
        if (isAlive == true)
        {
            if (isEnemy == true)
            {
                UpdateHealth(amount);
            }
            else if (isEnemy == false && photonView.IsMine)
            {
                UpdateHealth(amount);
            }
        }
    }

    private void UpdateHealth(int amount)
    {
        CurrentHealth -= amount;
        healthBarHandler.SetHealthBarValue((float)CurrentHealth / MaxHealth);

        if (CurrentHealth <= 0)
        {
            isAlive = false;
            Die();
        }
    }

    public void Die()
    {
        if (isEnemy == true)
        {
            if (dropsMeat == true)
            {
                photonView.RPC(nameof(SpawnRawMeatRPC), RpcTarget.MasterClient);
            }
            rootObject.DeSpawn();
        }
        else
        {
            photonView.RPC(nameof(SpawnReviveBadgeRPC), RpcTarget.MasterClient);
            CurrentHealth = 0;
            transform.root.gameObject.SetActive(false);
        }
    }

    [PunRPC]
    private void SpawnRawMeatRPC()
    {
        InstantiateRoomObject("Prefabs/Pickups/Alien Meat", null);
    }

    [PunRPC]
    private void SpawnReviveBadgeRPC()
    {
        InstantiateRoomObject("Prefabs/Pickups/ReviveBadge", new object[] { (rootObject != null ? rootObject.GetComponent<PhotonView>().ViewID : photonView.ViewID) });
    }

    private void InstantiateRoomObject(string prefabPath, object[] parameters)
    {
        Vector3 spawnPosition = new Vector3(transform.position.x, transform.position.y + dropOffsetY, transform.position.z);
        PhotonNetwork.InstantiateRoomObject(prefabPath, spawnPosition, Quaternion.identity, 0, parameters);
    }

    public void Revive(Vector3 revivePosition)
    {
        photonView.RPC(nameof(ReviveRPC), RpcTarget.All, revivePosition);
    }

    [PunRPC]
    private void ReviveRPC(Vector3 revivePosition)
    {
        transform.root.gameObject.SetActive(true);
        ResetHealth();
        transform.position = revivePosition;
    }

    [PunRPC]
    private void AddHealth(int amount)
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
}
