using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EnemyHealthHandler : HealthHandler
{
    [Header("If part of pooled object")]
    [SerializeField] private PooledObject rootObject;

    public override void TakeDamage(int amount)
    {
        photonView.RPC(nameof(TakeDamageRPC), RpcTarget.All, amount);
    }

    [PunRPC]
    private void TakeDamageRPC(int amount)
    {
        if (isAlive == true)
        {
            RemoveHealth(amount);
        }
    }

    public override void Die()
    {
        if (itemDropPrefab != null)
        {
            DropItem();
        }
        rootObject?.DeSpawn();
    }

    public override void DropItem()
    {
        photonView.RPC(nameof(SpawnRawMeatRPC), RpcTarget.MasterClient);
    }

    [PunRPC]
    private void SpawnRawMeatRPC()
    {
        InstantiateRoomObject(null);
    }
}
