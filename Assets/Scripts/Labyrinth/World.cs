using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    // 29 bred
    //31 l?ng


    [Header("World")]
    private int mapWidthInChunks = 16;
    private int mapLengthInChunks = 9;
    public int chunkSize = 20;
    //public float percentageBlocks = 0.35f;
    //public float noiseScale = 0.03f;
    public GameObject chunkPrefab;
    public GameObject labyrinthDesign;
    private LabyrinthReader labyrinthReader;
    public GameObject finalLabyrinth;
    private int[,] map;
    public float scale = 3f;
    public Vector3 position;
    

    private int chunkHeight = 1;

    Dictionary<Vector3Int, ChunkData> chunkDataDictionary = new Dictionary<Vector3Int, ChunkData>();
    Dictionary<Vector3Int, ChunkRenderer> chunkDictionary = new Dictionary<Vector3Int, ChunkRenderer>();

    private void Awake()
    {
        /*labyrinthReader = labyrinthDesign.GetComponent<LabyrinthReader>();
        map = labyrinthReader.GenerateCubePosition2DArray(chunkSize);
        mapWidthInChunks = Mathf.FloorToInt((float)(map.GetLength(0)) / chunkSize);
        mapLengthInChunks = Mathf.FloorToInt((float)map.GetLength(1) / chunkSize);*/
        finalLabyrinth = new GameObject();
        GenerateWorld();
    }

    [ContextMenu("Generate New World")]
    public void GenerateWorld()
    {
        
        finalLabyrinth.transform.localScale = new Vector3(1, 1, 1);
        finalLabyrinth.transform.position = new Vector3();

        labyrinthReader = labyrinthDesign.GetComponent<LabyrinthReader>();

        map = labyrinthReader.GenerateCubePosition2DArray(chunkSize);
        mapWidthInChunks = Mathf.FloorToInt((float)(map.GetLength(0)) / chunkSize);
        mapLengthInChunks = Mathf.FloorToInt((float)map.GetLength(1) / chunkSize);

        chunkDataDictionary.Clear();
        foreach (ChunkRenderer chunk in chunkDictionary.Values)
        {
            Destroy(chunk.gameObject);
        }
        chunkDictionary.Clear();

        for (int x = 0; x < mapWidthInChunks; x++)
        {
            for (int z = 0; z < mapLengthInChunks; z++)
            {

                ChunkData data = new ChunkData(chunkSize, chunkHeight, this, new Vector3Int(x * chunkSize, 0, z * chunkSize));
                GenerateVoxels(data);
                chunkDataDictionary.Add(data.worldPosition, data);
            }
        }

        foreach (ChunkData data in chunkDataDictionary.Values)
        {
            MeshData meshData = Chunk.GetChunkMeshData(data);
            GameObject chunkObject = Instantiate(chunkPrefab, data.worldPosition, Quaternion.identity, finalLabyrinth.transform);
            ChunkRenderer chunkRenderer = chunkObject.GetComponent<ChunkRenderer>();
            chunkDictionary.Add(data.worldPosition, chunkRenderer);
            chunkRenderer.InitializeChunk(data);
            chunkRenderer.UpdateChunk(meshData);

        }

        finalLabyrinth.transform.localScale *= scale;
        finalLabyrinth.transform.position += position;
       
    }

        private void GenerateVoxels(ChunkData data)
    {
        for (int x = 0; x < data.chunkSize; x++)
        {
            for (int z = 0; z < data.chunkSize; z++)
            {

                BlockType voxelType = BlockType.Air;
                
                if (map[data.worldPosition.x + x, data.worldPosition.z + z] > 0)
                {
                    voxelType = BlockType.Wall;
                }
                Chunk.SetBlock(data, new Vector3Int(x, 0, z), voxelType);


            }
        }
    }

    internal BlockType GetBlockFromChunkCoordinates(ChunkData chunkData, int x, int y, int z)
    {
        Vector3Int pos = Chunk.ChunkPositionFromBlockCoords(this, x, y, z);
        ChunkData containerChunk = null;

        chunkDataDictionary.TryGetValue(pos, out containerChunk);

        if (containerChunk == null)
            return BlockType.Nothing;
        Vector3Int blockInCHunkCoordinates = Chunk.GetBlockInChunkCoordinates(containerChunk, new Vector3Int(x, y, z));
        return Chunk.GetBlockFromChunkCoordinates(containerChunk, blockInCHunkCoordinates);
    }
}
