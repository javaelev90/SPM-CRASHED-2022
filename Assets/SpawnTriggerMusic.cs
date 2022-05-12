using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SpawnTriggerMusic : MonoBehaviour
{
   
    public AudioMixerSnapshot background;
    public AudioMixerSnapshot combat;
    private bool flipState = false; 

    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Player")){
            combat.TransitionTo(0.5f);
        }
    }

    private void OnTriggerExit(Collider other) {
        if(other.CompareTag("Player")){
            background.TransitionTo(0.5f);
        }
    }
}