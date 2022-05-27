using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingDissolveShader : MonoBehaviour
{

    /*public Material dissolveMat;
    public float health;
    public float maxHealth;*/
    public Animator animator;


    void Start()
    {
        if (gameObject.CompareTag("GlowTest"))
        animator.SetTrigger("Hurt");
    }

    void Update()
    {
        /*
            health -= 0.1f;
            dissolveMat.SetFloat("EmissionFill", health / maxHealth);*/
    }
   
 
}

