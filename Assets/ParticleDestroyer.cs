using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleDestroyer : MonoBehaviour
{
    [SerializeField] private float destroyDelay = 1f;

    public float DestroyDelay
    {
        get
        {
            return destroyDelay;
        }
        set
        {
            destroyDelay = value;
        }
    }
}
