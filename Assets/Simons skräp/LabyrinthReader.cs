using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabyrinthReader : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //CombineMeshes();
        GenerateCubePosition2DArray();
    }

    public float[,] GenerateCubePosition2DArray()
    {
        Transform[] cubePositions = GetComponentsInChildren<Transform>();

        Debug.Log(cubePositions.Length);

        float[,] positions = new float[cubePositions.Length, 2];

        for(int i = 0; i < cubePositions.Length; i++)
        {
            positions[i, 0] = cubePositions[i].position.x;
            positions[i, 1] = cubePositions[i].position.z;
        }

        for (int i = 0; i < positions.GetLength(0); i++)
        {
            Debug.Log(positions[i, 0] + ", " + positions[i, 1]);
        }

        Debug.Log(positions.GetLength(0));

        return positions;
    }

    private void CombineMeshes()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        for(int i = 0; i < meshFilters.Length; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);
        }

        GetComponent<MeshFilter>().mesh = new Mesh();
        GetComponent<MeshFilter>().mesh.CombineMeshes(combine, true);
        transform.SetPositionAndRotation(new Vector3(0, 0, 0), Quaternion.identity);
        gameObject.SetActive(true);
    }
}
