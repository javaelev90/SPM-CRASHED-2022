using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using EventCallbacksSystem;
using Photon.Pun;

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
        
        if(player.gameObject.Equals(GameManager.player))
        {
            if (uiObject != null)
            {
                uiObject.SetActive(true);
            }
            foreach (DialogueTrigger dialogue in dialogs)
            {
                if (dialogue.gameObject.activeInHierarchy)
                {
                    dialogue.beginDialogue();
                }
            }
        }
    }
    

    private void OnTriggerExit(Collider player) {
        if (uiObject != null)
        {
            if (uiObject.activeSelf)
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
}