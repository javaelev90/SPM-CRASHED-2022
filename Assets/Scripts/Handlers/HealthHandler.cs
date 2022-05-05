using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class HealthHandler : MonoBehaviourPunCallbacks
{
    [Header("Required components")]
    [SerializeField] private GameObject rootObject;
    [SerializeField] private HealthBarHandler healthBarHandler;

    [Header("Health")]
    [SerializeField] private int MaxHealth;
    private int CurrentHealth;
    [SerializeField] private bool isEnemy;
    public bool IsAlive { get; internal set; }

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
        IsAlive = true;
    }

    [PunRPC]
    private void TakeDamageRPC(int amount)
    {
        if (IsAlive)
        {
            Debug.Log("Taken damage " + gameObject.GetInstanceID());
            CurrentHealth -= amount;
            healthBarHandler.SetHealthBarValue((float)CurrentHealth / MaxHealth);

            if (CurrentHealth <= 0)
            {
                IsAlive = false;
                Die();
            }
        }
    }

    public void Die()
    {
        if (isEnemy)
        {
            //rootObject.DeSpawn();
            /*GetComponent<EnemyCharacter>().Die()*/;
        }
        else
        {
            if (PhotonNetwork.IsMasterClient)
            {
                photonView.RPC(nameof(SpawnReviveBadgeRPC), RpcTarget.All);
            }
            CurrentHealth = 0;
            transform.root.gameObject.SetActive(false);
        }
    }

    [PunRPC]
    private void SpawnReviveBadgeRPC()
    {
        PhotonNetwork.InstantiateRoomObject("Prefabs/Pickups/ReviveBadge", transform.position, Quaternion.identity, 0, new object[] { rootObject.GetComponent<PhotonView>().ViewID });
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
