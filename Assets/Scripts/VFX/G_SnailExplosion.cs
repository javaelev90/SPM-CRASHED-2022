using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G_SnailExplosion : MonoBehaviour
{
    public GameObject snail;
    public GameObject explosionEffect;
    public GameObject readyToExplodeEffect;
    public GameObject SnailGooDecalPrefab;

    public float timeTillExplode = 10f;

   public AudioSource source;
    public AudioClip explode;

    public void Explode()
    {
        var vfx = Instantiate(explosionEffect, snail.transform.position, Quaternion.identity) as GameObject; 
        source.PlayOneShot(explode);
        //vfx.transform.SetParent(this.transform); //   vfx.transform.SetParent(snail.transform);
        //var ps = vfx.GetComponent<ParticleSystem>();
        Destroy(vfx.gameObject, 10f);// ps.main.duration + ps.main.startLifetime.constantMax);
        Destroy(this.gameObject, 20f);// ps.main.duration + ps.main.startLifetime.constantMax);
       
        //Instantiate(explosionEffect, this.transform.position, this.transform.rotation);
        SpawnDecal();
        //Destroy(this.gameObject);
    }

     private void Start()
    {
        source = GetComponent<AudioSource>();
    }

   
    public void ReadyToExplode()
    {
        var vfx = Instantiate(readyToExplodeEffect, snail.transform.position, Quaternion.identity) as GameObject;
        Debug.Log(vfx.name);
        Destroy(vfx.gameObject, 3f);
    }

    void SpawnDecal()
    {
        Vector3 from = this.transform.position;
        Vector3 to = new Vector3(this.transform.position.x, this.transform.position.y - (this.transform.localScale.y / 2.0f) + 0.1f, this.transform.position.z);

        RaycastHit hit;
        if (Physics.Raycast(from, Vector3.down * 3f, out hit) == true)
        {
            GameObject decal = Instantiate(SnailGooDecalPrefab);
            decal.transform.position = hit.point;
        }
    }
}
