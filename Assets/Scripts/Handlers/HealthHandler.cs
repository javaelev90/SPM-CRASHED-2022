using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class HealthHandler : MonoBehaviourPunCallbacks
{

    public int MaxHealth { get; internal set; }
    public int CurrentHealth { get; internal set; }

    [SerializeField] private bool isEnemy;

    public bool IsAlive { get; internal set; }

    public void TakeDamage(int amount)
    {
        if (isEnemy)
            photonView.RPC(nameof(TakeDamageRPC), RpcTarget.All, amount);
        else
            TakeDamageRPC(amount);
    }

    void Start()
    {
        IsAlive = true;
    }

    void Update()
    {

    }

    [PunRPC]
    private void TakeDamageRPC(int amount)
    {
        if (IsAlive)
        {
            CurrentHealth -= amount;
            //healthBarHandler.SetHealthBarValue((float)CurrentHealth / MaxHealth);

            if (CurrentHealth <= 0)
            {
                IsAlive = false;
                if (isEnemy)
                {
                    //GetComponent<EnemyCharacter>().Die();
                }
                else
                {
                    //GetComponent<PlayerCharacter>().Die();
                }
            }
        }
    }
}
