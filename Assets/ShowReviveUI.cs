using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowReviveUI : MonoBehaviour
{

    public GameObject uiObject;

     [SerializeField] private InventorySystem inventory;


    // Start is called before the first frame update
    void Start()
    {
        uiObject.SetActive(false);
        inventory = GetComponent<InventorySystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (inventory != null)
        {
            if (inventory.Amount<ReviveBadge>() == 0)
            {
                uiObject.SetActive(false);
            }
        }
    }
    
    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Player")  && other.gameObject.Equals(GameManager.player)){
            inventory = other.gameObject.GetComponent<InventorySystem>();
            if (inventory.Amount<ReviveBadge>() > 0)
            {
                uiObject.SetActive(true);
                Debug.Log("jag är död");
            }
            
        }
    }
}

