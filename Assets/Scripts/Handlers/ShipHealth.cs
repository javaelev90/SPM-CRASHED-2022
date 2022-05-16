using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using EventCallbacksSystem;

public class ShipHealth : HealthHandler
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
        if(amount < 95){
            source.PlayOneShot(hit);
        }
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
        EventSystem.Instance.FireEvent(new GameOverEvent("Ship has been destroyed"));
    }
}
