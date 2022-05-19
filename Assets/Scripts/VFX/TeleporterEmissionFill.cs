using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterEmissionFill : MonoBehaviour
{

    public Mesh mesh;
    public float objectHeight;
    public Material mat;
    public int matIndex;

    // Start is called before the first frame update
    void Start()
    {
        mesh = gameObject.GetComponent<MeshFilter>().mesh;
        mat = gameObject.GetComponent<MeshRenderer>().materials[matIndex];
        
    }

    // Update is called once per frame
    void Update()
    {
        objectHeight = mesh.bounds.size.y;

        mat.SetFloat("_ObjectHeight", objectHeight);
    }
}
