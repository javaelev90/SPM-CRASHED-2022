using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowUI : MonoBehaviour
{
    public GameObject uiObject;

    // Start is called before the first frame update
    void Start()
    {
        uiObject.SetActive(false);
    }

    void OnTriggerEnter(Collider player){
        if(player.gameObject.tag == "Player")
        {
           uiObject.SetActive(true);
           Debug.Log("skriver ut ");
            StartCoroutine("WaitForSec");
        }
        
    }
    
    IEnumerator WaitForSec(){
        yield return new WaitForSeconds(5);
        Destroy(uiObject);
    }
}
