using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowUiTeleport : MonoBehaviour
{
    public GameObject uiObject;

   // public DialoguePickups dialog;
    

    // Start is called before the first frame update
    void Start()
    {
        uiObject.SetActive(false);
    }

    void OnTriggerEnter(Collider player){
        
        if(player.gameObject.Equals(GameManager.player))
        {
            
            uiObject.SetActive(true);
            Destroy(uiObject, 5);
        }
    }

}
