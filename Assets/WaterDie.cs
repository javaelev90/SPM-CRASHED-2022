using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterDie : MonoBehaviour
{

    PlayerHealthHandler handler;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.GetComponent<SoldierCharacter>() || other.gameObject.GetComponent<Engineer>()){
            handler.Die();
        }
    }
}
