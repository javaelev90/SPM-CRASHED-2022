using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gizmo_DrawMesh : MonoBehaviour
{
    [SerializeField] private Mesh theMesh;

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireMesh(theMesh, transform.position);
    }
}
