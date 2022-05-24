using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPickupShip : MonoBehaviour
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

     void OnTriggerExit(Collider player)
    {
        if (player.CompareTag("Player"))
        {
          StopCoroutine(dialogs.type);
          dialogs.gameObject.SetActive(false);
        }
    }
}
