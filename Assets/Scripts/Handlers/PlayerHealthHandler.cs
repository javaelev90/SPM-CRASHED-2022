using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using EventCallbacksSystem;

public class PlayerHealthHandler : HealthHandler
{
    private bool immortal = false;
    [SerializeField] private GameObject deathParticles;
    [SerializeField] private Camera deathCamera;
    [SerializeField] private GameObject playerUI;

    private void Start()
    {
        EventSystem.Instance.RegisterListener<ImmortalEvent>(SetImmortal);
        EventSystem.Instance.RegisterListener<HealthUpgradeEvent>(UpgradeHealth);
    }

    public void SetImmortal(ImmortalEvent immortalEvent)
    {
        immortal = !immortal;
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
        UpdateDeathCamera(null, true);
        playerUI.transform.SetParent(deathCamera.transform);
        Destroy(Instantiate(deathParticles, transform.position, transform.rotation), 10f);
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
        UpdateDeathCamera(transform, false);
        playerUI.transform.SetParent(transform);
        transform.root.gameObject.SetActive(true);
        transform.position = revivePosition;
        ResetHealth();
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

    public void UpgradeHealth(HealthUpgradeEvent healthUpgradeEvent)
    {
        MaxHealth += healthUpgradeEvent.UpgradeAmount;
        CurrentHealth += healthUpgradeEvent.UpgradeAmount;
    }

    private void UpdateDeathCamera(Transform parent, bool active)
    {
        deathCamera.gameObject.SetActive(active);
        deathCamera.enabled = active;
        deathCamera.transform.SetParent(parent);
    }
}
