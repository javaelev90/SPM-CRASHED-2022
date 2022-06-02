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

        //if (amount < 95)
        //{
            //Debug.Log("debug");
            //source.PlayOneShot(hit);
        //}


        if (CurrentHealth <= 950 || CurrentHealth <= 500 || CurrentHealth <= 100)
        {
            Debug.Log("Playing warning about ship health dropping");
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
                photonView.RPC(nameof(SetHealthRPC), RpcTarget.Others, CurrentHealth);
            }
        }
    }
    public override void Die()
    {
        //EventSystem.Instance.FireEvent(new GameOverEvent("Ship has been destroyed"));
    }

    public override void IsInLava(bool inLava) { }
}
