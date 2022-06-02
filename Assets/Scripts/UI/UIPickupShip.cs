using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPickupShip : MonoBehaviour
{
  
   [SerializeField] Dialogue[]  dialogs;

     private static bool done = true;
    

    // Start is called before the first frame update

    void OnTriggerEnter(Collider player){
      Debug.Log("hejsan");
      if(done &&  player.CompareTag("Player"))
      {  
          done = false; 
          foreach (Dialogue dialogue in dialogs)
          {
            dialogue.beginDialogue();
            Debug.Log("skriver ut");
          }
      }
    }

     void OnTriggerExit(Collider player)
    {
        if (player.CompareTag("Player"))
        {
          //StopCoroutine(dialogs.type);
           foreach (var d in dialogs)
                {
                    d.gameObject.SetActive(false);
                }
        }
    }
}
