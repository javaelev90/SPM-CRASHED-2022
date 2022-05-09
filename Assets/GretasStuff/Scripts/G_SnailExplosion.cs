using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G_SnailExplosion : MonoBehaviour
{
    public GameObject snail;
    public GameObject explosionEffect;
    public GameObject SnailGooDecalPrefab;

    public float timeTillExplode = 10f;

    private bool hasExploded = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(timeTillExplode > 0)
        {
        timeTillExplode -= Time.deltaTime;
        }
        else if(!hasExploded)
        {
            Explode();
            hasExploded = true;
        }
    }

    void Explode()
    {
        var vfx = Instantiate(explosionEffect, snail.transform.position, Quaternion.identity) as GameObject;
        vfx.transform.SetParent(this.transform); //   vfx.transform.SetParent(snail.transform);
        var ps = vfx.GetComponent<ParticleSystem>();
        Destroy(vfx, 10f);// ps.main.duration + ps.main.startLifetime.constantMax);
        snail.gameObject.GetComponent<MeshRenderer>().enabled = false;
        Destroy(this.gameObject, 20f);// ps.main.duration + ps.main.startLifetime.constantMax);

        //Instantiate(explosionEffect, this.transform.position, this.transform.rotation);
        SpawnDecal();
        //Destroy(this.gameObject);
    }

    void SpawnDecal()
    {
        Vector3 from = this.transform.position;
        Vector3 to = new Vector3(this.transform.position.x, this.transform.position.y - (this.transform.localScale.y / 2.0f) + 0.1f, this.transform.position.z);
        Vector3 direction = to - from;

        RaycastHit hit;
        if (Physics.Raycast(from, direction, out hit) == true)
        {
            GameObject decal = Instantiate(SnailGooDecalPrefab);
            decal.transform.position = hit.point;
            //decal.transform.position = this.transform.position;
        }
    }
}
