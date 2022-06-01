using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowReviveUI : MonoBehaviour
{

    public GameObject uiObject;

    InventorySystem inventory;


    // Start is called before the first frame update
    void Start()
    {
        uiObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Player") && inventory.Amount<ReviveBadge>() > 0){
            uiObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other) {
        if(other.CompareTag("Player") && inventory.Amount<ReviveBadge>() == 0){
            uiObject.SetActive(false);
        }
    }
}
