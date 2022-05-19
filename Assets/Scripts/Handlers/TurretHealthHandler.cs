using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using EventCallbacksSystem;

public class TurretHealthHandler : HealthHandler
{
    [SerializeField] private GameObject[] pickupPrefabs; //size gets set in inspector! drag prefabs in there!
    [SerializeField] private GameObject[] pickups;
    [SerializeField] private int amountToSpawn = 5;
    [SerializeField] private float offsetY = 1f;

    private void Start()
    {
        EventSystem.Instance.RegisterListener<TurretHealthUpgradeEvent>(UpgradeTurretHealth);
    }

    public void UpgradeTurretHealth(TurretHealthUpgradeEvent turretHealthUpgradeEvent)
    {
        MaxHealth += turretHealthUpgradeEvent.UpgradeAmount;
        CurrentHealth += turretHealthUpgradeEvent.UpgradeAmount;
    }

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
        photonView.RPC(nameof(SpawnItems), RpcTarget.MasterClient);
    }

    [PunRPC]
    private void SpawnItems()
    {
        SpawnItemDrops();
    }

    protected void SpawnItemDrops()
    {
        pickups = new GameObject[pickupPrefabs.Length]; //makes sure they match length

        for (int i = 0; i < amountToSpawn; i++)
        {
            for (int j = 0; j < pickupPrefabs.Length; j++)
            {
                Vector3 position = new Vector3(transform.position.x + Random.Range(-2, 2), transform.position.y + offsetY, transform.position.z + Random.Range(-3, 3));
                pickups[j] = PhotonNetwork.InstantiateRoomObject(GlobalSettings.PickupsPath + pickupPrefabs[j].name, position, Quaternion.identity);
            }
        }

    }
}
