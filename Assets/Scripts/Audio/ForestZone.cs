using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestZone : MonoBehaviour
{

    public AudioSource source;
    public bool played = false; 
    public AudioClip triggerSound;
    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void OnTriggerEnter()
    {
        if (!played)
        {
            source.PlayOneShot(triggerSound);
            played = true;
            //Debug.Log("Spelar");
        }
    }

}
