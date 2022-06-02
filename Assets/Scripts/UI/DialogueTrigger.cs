using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using EventCallbacksSystem;

public class DialogueTrigger : MonoBehaviour
{

    private int index;
    public float textSpeed;
    public string[] numberOfLines;
    public TextMeshProUGUI textComponent;
    public DialogueTrigger players;
    public bool isFirst;
    private bool isDone ;
    private bool isTyping;

    private bool startOfGame = true;
    // Start is called before the first frame update
  
       void OnTriggerEnter(Collider player)
    {
        Debug.Log("hej");
        if (player.CompareTag("Player") && isFirst)
        {
            textComponent.text = string.Empty;
            beginDialogue();
        }
    }

    void OnTriggerExit(Collider player)
    {
        if (player.CompareTag("Player"))
        {
           textComponent.enabled = false;
        }
    }

   public void beginDialogue()
    {
        index = 0;
        StartCoroutine(Types());
        
    }

    IEnumerator Types()
    {
        for (int index = 0; index < numberOfLines.Length - 1; index++) {
            textComponent.text = string.Empty;

            char[] line = numberOfLines[index].ToCharArray();
            Debug.Log(numberOfLines[index]);
            for (int c = 0; c < line.Length; c++) {
                textComponent.text += line[c];
                yield return new WaitForSeconds(textSpeed);
            }
                yield return new WaitForSeconds(1);

        }
                yield return new WaitForSeconds(1);

        gameObject.SetActive(false);
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
                Debug.Log("SAAAAAAAAAAAAAAAAVE ME");
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
        }
    }
}