using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LabyrinthReader : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        CombineMeshes();
        //GenerateCubePosition2DArray();
    }

    public int[,] GenerateCubePosition2DArray(int chunkSize)
    {
        Transform[] cubePositions = GetComponentsInChildren<Transform>();

        Debug.Log(cubePositions.Length);

        int[,] positions;

        float maxX = float.MinValue;
        float maxY = float.MinValue;
        float minX = float.MaxValue;
        float minY = float.MaxValue;
        float scale = 3;
        bool haveScale = false;

        foreach(Transform cube in cubePositions)
        {
            if (cube.position.x < minX)
                minX = cube.position.x;
            if (cube.position.x > maxX)
                maxX = cube.position.x;
            if (cube.position.z < minY)
                minY = cube.position.z;
            if (cube.position.z > maxY)
                maxY = cube.position.z;
            if (!haveScale)
            {
                haveScale = !haveScale;
                //scale = cube.localScale.x;
            }
        }
        Debug.Log(scale);
        int x = Mathf.FloorToInt((maxX - minX) / scale);
        int y = Mathf.FloorToInt((maxY - minY) / scale);
        x += chunkSize;
        y += chunkSize;

        positions = new int[x, y];

        foreach (Transform cube in cubePositions) 
        {
            //Debug.Log(positions.GetLength(0) + ", " + Mathf.FloorToInt((cube.position.x - minX) / scale) + ", " + positions.GetLength(1) + ", " + Mathf.FloorToInt((cube.position.z - minY) / scale));
            positions[Mathf.FloorToInt((cube.position.x - minX) / scale), Mathf.FloorToInt((cube.position.z - minY) / scale)] = 1;
        }
        //foreach (int i in positions)
            //Debug.Log(i);

        return positions;
    }

    private void CombineMeshes()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length-1];

        for(int i = 1; i < meshFilters.Length; i++)
        {
            combine[i-1].mesh = meshFilters[i].sharedMesh;
            combine[i-1].transform = meshFilters[i].transform.localToWorldMatrix;
        }

        GetComponent<MeshFilter>().mesh = new Mesh();
        GetComponent<MeshFilter>().mesh.CombineMeshes(combine, true);
        GetComponent<MeshCollider>().sharedMesh = GetComponent<MeshFilter>().mesh;
        transform.DetachChildren();
        transform.SetPositionAndRotation(new Vector3(0, 0, 0), Quaternion.identity);
        foreach(MeshFilter mf in meshFilters)
        {
            mf.transform.parent = gameObject.transform;
        }
        gameObject.SetActive(true);
    }
}
