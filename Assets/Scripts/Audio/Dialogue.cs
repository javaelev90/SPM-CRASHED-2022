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
    // Start is called before the first frame update
    void Start()
    {
      // EventSystem.Instance.RegisterListener<EventEvent>();
        if (isFirst)
        {
            textComponent.text = string.Empty;
            beginDialogue();
        }
    }

    // Update is called once per frame
    
  void Update()
    {
        
           // if(isTyping){  
             //   isDone = true;
            //}
        
    }
    void beginDialogue()
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