using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPickupShip : MonoBehaviour
{

    [SerializeField] Dialogue[] dialogs;

    private static bool done = true;


    // Start is called before the first frame update

    void OnTriggerEnter(Collider player)
    {
        if (done && player.CompareTag("Player"))
        {
            done = false;
            foreach (Dialogue dialogue in dialogs)
            {
                if (dialogue != null && dialogue.gameObject.activeInHierarchy)
                {
                    dialogue.beginDialogue();
                }
            }
        }
    }

    void OnTriggerExit(Collider player)
    {
        if (player != null)
        {
            if (player.CompareTag("Player"))
            {
                //StopCoroutine(dialogs.type);
                foreach (var d in dialogs)
                {
                    if(d != null && d.gameObject.activeInHierarchy)
                    d.gameObject.SetActive(false);
                }
            }
        }
    }
}
