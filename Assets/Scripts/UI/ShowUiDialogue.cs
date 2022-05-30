using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using EventCallbacksSystem;

public class ShowUiDialogue : MonoBehaviour
{
    public GameObject uiObject;

    [SerializeField] DialogueTrigger[]  dialogs;
    [SerializeField] private bool canSoldierPickup;
    [SerializeField] private bool canEngineerPickup;

   // public DialoguePickups dialog;
    

    // Start is called before the first frame update
    void Start()
    {
        uiObject.SetActive(false);
    }

    void OnTriggerEnter(Collider player){
        if(((canSoldierPickup && player.gameObject.GetComponent<SoldierCharacter>())) || ((canEngineerPickup && player.gameObject.GetComponent<Engineer>())))
        {
            Debug.Log("kolliderar");
            
            uiObject.SetActive(true);
            foreach (DialogueTrigger dialogue in dialogs)
            {
                
                dialogue.beginDialogue();
                Debug.Log("skriver ut");
            }
        }
    }

    private void OnTriggerExit(Collider player) {
        if(uiObject.activeSelf)
        {
           uiObject.SetActive(false);
           //StopCoroutine(dialogs.type);
           foreach (var d in dialogs)
                {
                    d.gameObject.SetActive(false);
                }
        }
    }
}