using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lava : MonoBehaviour
{
    private HealthHandler health;
    private bool takeDamage;
    [SerializeField] private int damageAmount;
    [SerializeField] private float timeBetweenDamage;

    private void OnTriggerEnter(Collider other)
    {
        
        health = other.gameObject.GetComponent<HealthHandler>();
        if (health != null)
        {
            Debug.Log(1);
            takeDamage = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(health == other.gameObject.GetComponent<HealthHandler>())
        {
            takeDamage = false;
        }
    }

    private IEnumerator LavaDamage()
    {
        health.TakeDamage(damageAmount);
        yield return new WaitForSeconds(timeBetweenDamage);
        if (takeDamage)
            LavaDamage();
    }
}
