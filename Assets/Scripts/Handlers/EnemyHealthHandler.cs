using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EnemyHealthHandler : HealthHandler
{
    [Header("If part of pooled object")]
    [SerializeField] private PooledObject rootObject;

    [SerializeField] private GameObject hitVFX;
    GameObject vfx;

    private bool hasBeenHurt = false;
    private Animator anim;
    

    public override void TakeDamage(int amount)
    {
        photonView.RPC(nameof(TakeDamageRPC), RpcTarget.All, amount);
        // Plays VFX where the bullet hits on enemy
        Destroy(Instantiate(hitVFX,transform.position, transform.rotation), 2f);
        /*if (!hasBeenHurt)
        {
            hasBeenHurt = true;
            anim = GetComponentInChildren<Animator>();
            if(anim != null)
            {
                //hurtAnimator.SetTrigger("Hurt");
                anim = GetComponentInChildren<Animator>();
                anim.SetTrigger("Hurt");
                //gameObject.GetComponentInChildren<EnemyHurtAnimation>().PlayEnemyHurtAnim();
            }
        }*/
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
