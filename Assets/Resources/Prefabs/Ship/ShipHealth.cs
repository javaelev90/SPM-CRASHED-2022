using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using EventCallbacksSystem;

public class ShipHealth : HealthHandler
{
    AudioSource source;
    public AudioClip clip;

    public override void TakeDamage(int amount)
    {
        photonView.RPC(nameof(TakeDamageRPC), RpcTarget.All, amount);
        if(amount < 95){
            source.PlayOneShot(clip);
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
