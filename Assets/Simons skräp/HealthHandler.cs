using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthHandler : MonoBehaviour
{
    [SerializeField] private HealthBarHandler healthBarHandler;
    public int MaxHealth { get; internal set; }
    public int CurrentHealth { get; internal set; }

    public void TakeDamage(int amount)
    {
        CurrentHealth -= amount;
        healthBarHandler.SetHealthBarValue((float) CurrentHealth / MaxHealth);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
