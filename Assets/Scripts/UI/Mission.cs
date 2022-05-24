using System.Collections;  
using System.Collections.Generic;  
using UnityEngine;  
public class Mission: MonoBehaviour {  
    public GameObject Panel;  
    public void PanelOpener() {  
        if (Panel != null) {  
            bool isActive = Panel.activeSelf;  
            Panel.SetActive(!isActive);  
        }  
    }  

       public void PanelCloser() {  
       Panel.SetActive(false);
        }  
} 