using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class LabyrinthMusic : MonoBehaviour
{
   
    public AudioMixerSnapshot background;
    public AudioMixerSnapshot labyrinth;
    private bool flipState = false; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Player")){
            labyrinth.TransitionTo(0.5f);
        }
    }

    private void OnTriggerExit(Collider other) {
        if(other.CompareTag("Player")){
            background.TransitionTo(0.5f);
        }
    }
}