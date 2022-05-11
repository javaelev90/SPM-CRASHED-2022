using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerHealthHandler : HealthHandler
{
    public override void TakeDamage(int amount)
    {
        photonView.RPC(nameof(TakeDamageRPC), RpcTarget.All, amount);
    }

    [PunRPC]
    private void TakeDamageRPC(int amount)
    {
        if (isAlive == true)
        {
            if (photonView.IsMine)
            {
                RemoveHealth(amount);
            }
        }
    }

    public override void Die()
    {
        if (itemDropPrefab is object)
        {
            DropItem();
        }
        CurrentHealth = 0;
        transform.root.gameObject.SetActive(false);
    }

    public override void DropItem()
    {
        photonView.RPC(nameof(SpawnReviveBadgeRPC), RpcTarget.MasterClient);
    }

    [PunRPC]
    private void SpawnReviveBadgeRPC()
    {
        InstantiateRoomObject(new object[] { photonView.ViewID });
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

}
