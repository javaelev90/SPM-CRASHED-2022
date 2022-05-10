using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    // 29 bred
    //31 lång


    [Header("World")]
    public int mapWidthInChunks = 16;
    public int mapLengthInChunks = 9;
    public int chunkSize = 20;
    public float percentageBlocks = 0.35f;
    public float noiseScale = 0.03f;
    public GameObject chunkPrefab;
    public Transform Labyrinth;
    public LabyrinthReader labyrinthReader;
    private float[,] map;

    private int chunkHeight = 1;

    Dictionary<Vector3Int, ChunkData> chunkDataDictionary = new Dictionary<Vector3Int, ChunkData>();
    Dictionary<Vector3Int, ChunkRenderer> chunkDictionary = new Dictionary<Vector3Int, ChunkRenderer>();

    private void Start()
    {
        map = labyrinthReader.GenerateCubePosition2DArray();
        GenerateWorld();
    }

    [ContextMenu("Generate New World")]
    public void GenerateWorld()
    {
        // Log execution time.
        var stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();

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
            GameObject chunkObject = Instantiate(chunkPrefab, data.worldPosition, Quaternion.identity, Labyrinth);
            ChunkRenderer chunkRenderer = chunkObject.GetComponent<ChunkRenderer>();
            chunkDictionary.Add(data.worldPosition, chunkRenderer);
            chunkRenderer.InitializeChunk(data);
            chunkRenderer.UpdateChunk(meshData);

        }
        // Logging 
        stopwatch.Stop();
        Debug.LogFormat("[WorldGenerator::BuildWorld] Execution time: {0}ms", stopwatch.ElapsedMilliseconds);
    }

        private void GenerateVoxels(ChunkData data)
    {
        for (int x = 0; x < data.chunkSize; x++)
        {
            for (int z = 0; z < data.chunkSize; z++)
            {

                BlockType voxelType = BlockType.Air;
                
                if (map[x,z] > 0)
                {
                    Debug.Log(1);
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
