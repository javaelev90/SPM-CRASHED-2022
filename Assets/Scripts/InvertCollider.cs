using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InvertCollider : MonoBehaviour
{
    public MeshCollider meshCollider;

    private void Awake()
    {
        if (!meshCollider) meshCollider = GetComponent<MeshCollider>();

        var mesh = meshCollider.sharedMesh;

        // Reverse the triangles
        mesh.triangles = mesh.triangles.Reverse().ToArray();

        // also invert the normals
        mesh.normals = mesh.normals.Select(n => -n).ToArray();
    }
}