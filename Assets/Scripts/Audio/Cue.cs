using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cue : MonoBehaviour
{
    // Start is called before the first frame update

    AudioSource source;
    	private double time = 20f;

    
    void Start()
    {
         source = GetComponent<AudioSource>();

        source.PlayDelayed(18f);
    }

    // Update is called once per frame
   
}
