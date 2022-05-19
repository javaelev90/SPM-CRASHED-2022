using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class CreateDuplicate : MonoBehaviour
{
    public MeshFilter Filter;
    public string path;
    public string nameOfNewMesh;

    // Start is called before the first frame update
    /*void Start()
    {
        Mesh incertedMesh = mesh;
       
        // Reverse the triangles
        incertedMesh.triangles = incertedMesh.triangles.Reverse().ToArray();

        // also invert the normals
        incertedMesh.normals = incertedMesh.normals.Select(n => -n).ToArray();

        AssetDatabase.CreateAsset( incertedMesh, path);
        AssetDatabase.SaveAssets();
    }
    */
    // Update is called once per frame
    void Update()
    {
        
    }
#if (UNITY_EDITOR)
    [ContextMenu("Generate Invert")]
    public void CreateInvetedDuplicate()
    {
        //GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Mesh incertedMesh = Instantiate(Filter.mesh);

        
        // Reverse the triangles
        incertedMesh.triangles = incertedMesh.triangles.Reverse().ToArray();

        // also invert the normals
        incertedMesh.normals = incertedMesh.normals.Select(n => -n).ToArray();

        AssetDatabase.CreateAsset(incertedMesh, path + "/" + nameOfNewMesh + ".asset");
        AssetDatabase.SaveAssets();
    }
#endif
}
