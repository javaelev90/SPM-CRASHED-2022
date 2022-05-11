using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ShowUI : MonoBehaviour
{
    public GameObject uiObject;
    [SerializeField] private bool canSoldierPickup;
    [SerializeField] private bool canEngineerPickup;

    // Start is called before the first frame update
    void Start()
    {
        uiObject.SetActive(false);
    }

    void OnTriggerEnter(Collider player)
    {
        if (player.CompareTag("Player"))
        {
            bool isLocalPlayer = player.gameObject.GetComponent<PhotonView>().IsMine;
            if (isLocalPlayer && (canSoldierPickup && player.gameObject.GetComponent<SoldierCharacter>()) || (canEngineerPickup && player.gameObject.GetComponent<Engineer>()))
            {
                uiObject.SetActive(true);
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
            }
        }
    }
}