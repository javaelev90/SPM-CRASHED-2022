using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using EventCallbacksSystem;

public class ShowUI : MonoBehaviour
{
  
    [SerializeField] private bool canSoldierPickup;
    [SerializeField] private bool canEngineerPickup;

    [SerializeField] DialoguePickups[] dialogs;

     private static bool done = true;
    

    // Start is called before the first frame update
 
    void OnTriggerEnter(Collider player){
        if(( done && canSoldierPickup && player.gameObject.GetComponent<SoldierCharacter>()) || (canEngineerPickup && player.gameObject.GetComponent<Engineer>()))
        {  
           
            done = false; 
            
           foreach(DialoguePickups dialogue in dialogs)
           {
                dialogue.beginDialogue();
           }
        }

    }
}