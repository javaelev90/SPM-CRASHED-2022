using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TurretHealthHandler : HealthHandler
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
            RemoveHealth(amount);
        }
    }

    public override void Die()
    {
        gameObject.transform.GetChild(1).gameObject.SetActive(false);
        gameObject.transform.GetChild(3).gameObject.SetActive(true);
    }

    public void Revived()
    {
        gameObject.transform.GetChild(1).gameObject.SetActive(true);
        gameObject.transform.GetChild(3).gameObject.SetActive(false);
        UpdateHealthBar();
    }

    public void AddTurretHealth(int amount)
    {
        photonView.RPC(nameof(AddTurretHealthRPC), RpcTarget.All, amount);
    }

    [PunRPC]
    private void AddTurretHealthRPC(int amount)
    {
        if (CurrentHealth + amount > MaxHealth)
        {
            CurrentHealth = MaxHealth;

        }
        else
        {
            CurrentHealth += amount;
            UpdateHealthBar();
        }
    }

    public void SalvageDrop()
    {
        if (itemDropPrefab != null)
        {
            DropItem();
        }
    }

    public override void DropItem()
    {
        photonView.RPC(nameof(SpawnGreenGooRPC), RpcTarget.MasterClient);
    }

    [PunRPC]
    private void SpawnGreenGooRPC()
    {
        InstantiateRoomObject(null);
    }
}
