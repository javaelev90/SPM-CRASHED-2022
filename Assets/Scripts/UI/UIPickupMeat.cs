using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPickupMeat : MonoBehaviour
{

     [SerializeField] DialoguePickups dialogs;

     private static bool done = true;
    

    // Start is called before the first frame update

    void OnTriggerEnter(Collider player){
        if(done && player.gameObject.GetComponent<SoldierCharacter>())
        {  
        done = false; 
    
           
                dialogs.beginDialogue();
           
        
        }
    }

    void OnTriggerExit(){
        StopCoroutine(dialogs.type);
    }
}
