using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthHandler : MonoBehaviour
{
    [SerializeField] private HealthBarHandler healthBarHandler;
    public int MaxHealth { get; internal set; }
    public int CurrentHealth { get; internal set; }

    [SerializeField] private bool isEnemy;

    public bool IsAlive { get; internal set; }

    public void TakeDamage(int amount)
    {
        if (IsAlive)
        {
            CurrentHealth -= amount;
            healthBarHandler.SetHealthBarValue((float)CurrentHealth / MaxHealth);

            if (CurrentHealth <= 0)
            {
                IsAlive = false;
                if (isEnemy)
                {
                    GetComponent<EnemyCharacter>().Die();
                }
                else
                {
                    GetComponent<PlayerCharacter>().Die();
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        IsAlive = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
