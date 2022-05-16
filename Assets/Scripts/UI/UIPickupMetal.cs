using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using EventCallbacksSystem;


public class UIPickupMetal : MonoBehaviour
{

   [SerializeField] DialoguePickups dialogs;

     private static bool done = true;
    

    // Start is called before the first frame update

    void OnTriggerEnter(Collider player){
        if(done && player.gameObject.GetComponent<Engineer>())
        {  
        done = false; 
    
           
                dialogs.beginDialogue();
           
        
        }
    }
}
