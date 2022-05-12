using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using EventCallbacksSystem;
public class DialoguePickups : MonoBehaviour
{
  private int index;
    public float textSpeed;
    public string[] numberOfLines;
    public TextMeshProUGUI textComponent;
    public DialoguePickups players;
    public bool isFirst;
    private bool isDone = false;
    private bool isTyping;

    private static bool doneType = false;
  

   /* private void OnTriggerEnter(Collider other) {
        if (isFirst && other.CompareTag("Player") && doneType)
        {   
            doneType = true;
            Debug.Log("avslut");
            textComponent.text = string.Empty;
            beginDialogue();
            
        }
         Debug.Log("avslut");
    }*/
   public void beginDialogue()
    {
       
        index = 0;
        StartCoroutine(Type());
        
    }

    IEnumerator Type()
    {
        isTyping = true;
        foreach (char c in numberOfLines[index].ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
            if (isDone)
            {
                textComponent.text = numberOfLines[index];
                break;
            }
        }
        isTyping = false;
        isDone = false;
        if (index == 0 && isFirst)
        {
            players.beginDialogue();
        }
        else
        {
            players.Next();
        }
    }
    public void Next()
    {
        if (index < numberOfLines.Length - 1)
        {
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(Type());
        }
        else
        {
            gameObject.SetActive(false);
            textComponent.enabled = false;
        }
    }
}