using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using EventCallbacksSystem;

public class ShowUI : MonoBehaviour
{


    [SerializeField] DialoguePickups[] dialogs;

     private static bool done = true;
    

    // Start is called before the first frame update

    void OnTriggerEnter(Collider player){
        if(done && player.CompareTag("Player"))
        {  
        done = false; 
           foreach(DialoguePickups dialogue in dialogs)
           {
                dialogue.beginDialogue();
                  
           }
        
        }
    }
}