using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using EventCallbacksSystem;

public class PlayerHealthHandler : HealthHandler
{
    private bool immortal = false;
    private void Start()
    {
        EventSystem.Instance.RegisterListener<ImmortalEvent>(SetImmortal);
    }

    public void SetImmortal(ImmortalEvent immortal)
    {
        this.immortal = immortal.Immortal;
    }

    public override void TakeDamage(int amount)
    {
        if (immortal)
        {
            amount = 0;
        }
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
        UpdateActiveState(false);
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

    private void UpdateActiveState(bool active)
    {
        photonView.RPC(nameof(UpdateActiveStateRPC), RpcTarget.All, active);
    }

    [PunRPC]
    private void UpdateActiveStateRPC(bool active)
    {
        transform.root.gameObject.SetActive(active);
    }
}
