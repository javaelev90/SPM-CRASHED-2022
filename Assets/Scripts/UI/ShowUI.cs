using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using EventCallbacksSystem;

public class ShowUI : MonoBehaviour
{
    public GameObject uiObject;
    [SerializeField] private bool canSoldierPickup;
    [SerializeField] private bool canEngineerPickup;

    [SerializeField] DialoguePickups[] dialogs;
    

    // Start is called before the first frame update
    void Start()
    {
        uiObject.SetActive(false);
    }

    void OnTriggerEnter(Collider player){
        if((canSoldierPickup && player.gameObject.GetComponent<SoldierCharacter>()) || (canEngineerPickup && player.gameObject.GetComponent<Engineer>()))
        {
           uiObject.SetActive(true);
           foreach(DialoguePickups dialogue in dialogs)
           {
                dialogue.beginDialogue();
           }
        }
    }

    private void OnTriggerExit(Collider player) {
        if(uiObject.activeSelf)
        {
           uiObject.SetActive(false);
        }
    }
}