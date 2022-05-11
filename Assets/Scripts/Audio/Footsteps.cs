using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Footsteps : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] AudioClip[] audioClip;
    public PhysicsBody controller;
    // Start is called before the first frame update
    private void Awake()
    {
      //  audioSource = GetComponent<AudioSource>();
    }

    private void Step()
    {
        if (controller.Grounded)
        {
            AudioClip clip = GetAudioClip();
            audioSource.PlayOneShot(clip);
        }
    }

    private AudioClip GetAudioClip()
    {
        int index = Random.Range(0, audioClip.Length - 1);
        return audioClip[index];
    }
}
