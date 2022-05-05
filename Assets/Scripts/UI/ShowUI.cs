using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    void OnTriggerEnter(Collider player){
        if((canSoldierPickup && player.gameObject.GetComponent<SoldierCharacter>()) || (canEngineerPickup && player.gameObject.GetComponent<SoldierCharacter>()))
        {
           uiObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider player) {
        if(uiObject.activeSelf)
        {
           uiObject.SetActive(false);
        }
    }
}