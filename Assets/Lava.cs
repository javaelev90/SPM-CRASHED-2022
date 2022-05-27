using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lava : MonoBehaviour
{
    /* void OnCollisionEnter(Collision collision)
     {
         Debug.Log(1);
         foreach (ContactPoint contact in collision.contacts)
         {
             Debug.DrawRay(contact.point, Vector3.up, Color.red);
         }
     }*/

    private void OnTriggerEnter(Collider other)
    {
        HealthHandler health = other.gameObject.GetComponent<HealthHandler>();
        if (health != null)
        {
            health.IsInLava(true);
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        HealthHandler health = other.gameObject.GetComponent<HealthHandler>();
        if (health != null)
        {
            health.IsInLava(false);
        }
    }


}
