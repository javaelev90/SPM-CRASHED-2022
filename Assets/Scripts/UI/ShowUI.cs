using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using EventCallbacksSystem;
using Photon.Pun;


public class ShowUI : MonoBehaviour
{
    [SerializeField] DialoguePickups[] dialogs;

    private static bool done = true;
    public GameObject uiObject;
    [SerializeField] private bool canSoldierPickup;
    [SerializeField] private bool canEngineerPickup;

    // Start is called before the first frame update

    void OnTriggerEnter(Collider player)
    {
        if (player.CompareTag("Player"))
        {
            bool isLocalPlayer = player.gameObject.GetComponent<PhotonView>().IsMine;
            if (isLocalPlayer && ((canSoldierPickup && player.gameObject.GetComponent<SoldierCharacter>()) || (canEngineerPickup && player.gameObject.GetComponent<Engineer>())))
            {
                uiObject.SetActive(true);
            }
        }

        if (done && player.CompareTag("Player"))
        {
            done = false;
            foreach (DialoguePickups dialogue in dialogs)
            {
                if (dialogue.gameObject.activeInHierarchy)
                {
                    dialogue.beginDialogue();
                }
            }
        }
    }
    void OnTriggerExit(Collider player)
    {
        if (player.CompareTag("Player"))
        {
            bool isLocalPlayer = player.gameObject.GetComponent<PhotonView>().IsMine;
            if (isLocalPlayer && uiObject.activeSelf)
            {
                uiObject.SetActive(false);

                foreach (var d in dialogs)
                {
                    if (d.type != null)
                        StopCoroutine(d.type);
                    d.gameObject.SetActive(false);
                }
            }
        }
    }

}
