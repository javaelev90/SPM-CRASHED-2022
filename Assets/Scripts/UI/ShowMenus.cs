using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class ShowMenus : MonoBehaviour
{
    public ShowPauseMenu pause;
    // Start is called before the first frame update
    void Start()
    {
       // pause = FindObjectOfType<ShowPauseMenu>();
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void Show(InputAction.CallbackContext ctx){
         if(ctx.performed){
             Debug.Log("bajs");
            pause.gameObject.SetActive(!pause.gameObject.activeSelf);
        }
        Debug.Log("korv");
    }
}
