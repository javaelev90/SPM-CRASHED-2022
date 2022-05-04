using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class XPManager : MonoBehaviour
{

    public TextMeshProUGUI currentText, targetText;
    public int currentXP, targetXP;

    public static XPManager instace;
    // Start is called before the first frame update
   private void Awake() {
       if(instace == null)
       instace = this;
       else{
           Destroy(gameObject);
       }
   }

   public void add(int xp){
       currentXP += xp;
       currentText.text = currentXP.ToString();

   }
}
