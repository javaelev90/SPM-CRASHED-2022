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
    private bool isDone;
    private bool isTyping;

    public Coroutine type;

    ShowUiDialogue dialog;

  
   public void beginDialogue()
    {
        index = 0;
        type = StartCoroutine(Type());
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

    public void OnTriggerExit(){
        if(gameObject.CompareTag("Player")){
            StopCoroutine(Type());
            textComponent.CancelInvoke();
            textComponent.ClearMesh();
            gameObject.SetActive(false);
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
          
        }
    }
}