using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowUIShip : MonoBehaviour
{
 //public CapsuleCollider caps;

    

    public GameObject uiObject;

    Ship ship;

    // Start is called before the first frame update
    void Start()
    {
        uiObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

      private void OnTriggerEnter(Collider collider) {
       
         
        if(collider.CompareTag("Player") && ship.hasObtained && collider.gameObject.Equals(GameManager.player)){
              uiObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other) {
        if(other.CompareTag("Player") && ship.UppgradeShip()){
            uiObject.SetActive(false);
        }
    }
}