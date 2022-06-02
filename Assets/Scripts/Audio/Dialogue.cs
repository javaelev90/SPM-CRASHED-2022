using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using EventCallbacksSystem;

public class Dialogue : MonoBehaviour
{

    private int index;
    public float textSpeed;
    public string[] numberOfLines;
    public TextMeshProUGUI textComponent;
    public Dialogue players;
    public bool isFirst;
    private bool isDone;
    private bool isTyping;

    public bool startOfGame = true;
    // Start is called before the first frame update
    void Start()
    {

        
      // EventSystem.Instance.RegisterListener<EventEvent>();
        if (isFirst && startOfGame)
        {
            textComponent.text = string.Empty;
            startOfGame = false;
            //beginDialogue();
            StartCoroutine(StartOfGame());
        }
        else if(isFirst && !startOfGame){
            textComponent.text = string.Empty;
            beginDialogue();
        }
    }

    // Update is called once per frame

    private IEnumerator StartOfGame(){
  

            yield return new WaitForSeconds(10);
            beginDialogue();
        
    }
    
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
    void Next()
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