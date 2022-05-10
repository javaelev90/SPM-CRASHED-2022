using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class HealthHandler : MonoBehaviourPunCallbacks
{
    [Header("Required components")]
    [SerializeField] private PooledObject rootObject;
    [SerializeField] private HealthBarHandler healthBarHandler;

    [Header("Health")] // Keep these public to enable pooled object recycle functionality
    public int MaxHealth;
    public int CurrentHealth;
    public bool isAlive = true;
    [SerializeField] private bool isEnemy;

    public void TakeDamage(int amount)
    {
        if (isEnemy)
            photonView.RPC(nameof(TakeDamageRPC), RpcTarget.All, amount);
        else
            TakeDamageRPC(amount);
    }

    void Start()
    {
        ResetHealth();
    }

    private void ResetHealth()
    {
        CurrentHealth = MaxHealth;
        isAlive = true;
    }

    [PunRPC]
    private void TakeDamageRPC(int amount)
    {
        if (isAlive)
        {
            CurrentHealth -= amount;
            healthBarHandler.SetHealthBarValue((float)CurrentHealth / MaxHealth);

            if (CurrentHealth <= 0)
            {
                isAlive = false;
                Die();
            }
        }
    }

    public void Die()
    {
        if (isEnemy)
        {
            rootObject.DeSpawn();
        }
        else
        {
            if (PhotonNetwork.IsMasterClient)
            {
                //photonView.RPC(nameof(SpawnReviveBadgeRPC), RpcTarget.All);
            }
            if (photonView.IsMine)
            {
                CurrentHealth = 0;
                transform.root.gameObject.SetActive(false);
            }
        }
    }

    [PunRPC]
    private void SpawnReviveBadgeRPC()
    {
        //PhotonNetwork.InstantiateRoomObject("Prefabs/Pickups/ReviveBadge", transform.position, Quaternion.identity, 0, new object[] { (rootObject != null ? rootObject.GetComponent<PhotonView>().ViewID : photonView.ViewID) });
    }

    public void Revive(Vector3 revivePosition)
    {
        photonView.RPC(nameof(ReviveRPC), RpcTarget.All, revivePosition);
    }

    [PunRPC]
    private void ReviveRPC(Vector3 revivePosition)
    {
        Debug.Log("Trying to revive");
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
