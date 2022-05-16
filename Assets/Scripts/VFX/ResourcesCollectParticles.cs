using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesCollectParticles : MonoBehaviour
{

    public GameObject clickedVFX;
    public GameObject origin;



   

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void SpawnClickedVFX()
    {
        if (clickedVFX != null)
        {
            var vfx = Instantiate(clickedVFX, origin.transform.position, Quaternion.identity) as GameObject;
            vfx.transform.SetParent(origin.transform);
            var ps = vfx.GetComponent<ParticleSystem>();
            Destroy(vfx, ps.main.duration + ps.main.startLifetime.constantMax);
        }
    }

}
