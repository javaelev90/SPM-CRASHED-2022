using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecalSnailGoo : MonoBehaviour
{

    public float lifetime = 2.0f;

    private float mark;
    private Vector3 origSize;

    // Start is called before the first frame update
    void Start()
    {
        mark = Time.time;
        origSize = this.transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        float elapsedTime = Time.time - mark;
        if (elapsedTime != 0)
        {
            float percentTimeLeft = (lifetime - elapsedTime) / lifetime;

            this.transform.localScale = new Vector3(origSize.x * percentTimeLeft, origSize.y * percentTimeLeft, origSize.z * percentTimeLeft);
            if (elapsedTime > lifetime)
            {
                Destroy(this.gameObject);
            }
        }
    }
}
