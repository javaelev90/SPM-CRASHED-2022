using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Pun;

public class ObjectCulling : MonoBehaviour
{
    [Range(5, 60)]
    [SerializeField] private int minCullingSquareSide;
    [Range(5, 60)]
    [SerializeField] private int playerCullingSquareSide;
    [Range(5, 60)]
    [SerializeField] private int minQuadTreeCapacity;
    public QuadTree<PooledObject> quadTree { get; private set; }

    private List<Point<PooledObject>> activeObjects = new List<Point<PooledObject>>();
    private List<PhotonObjectPool> pools;
    private Quad<PooledObject> mapBoundary;
    private float worldWidth;
    private float worldHeigth;
    private GameObject player;
    private Quad<PooledObject> playerQuad;
    private Vector3 previousPosition;
    private float updateDistance = 1f;
    private float updateTimer = 0f;
    private float updateDelay = 0.2f;

    private void Start()
    {
        worldWidth = Terrain.activeTerrain.terrainData.size.x;
        worldHeigth = Terrain.activeTerrain.terrainData.size.z;
        mapBoundary = new Quad<PooledObject>(0, 0, worldWidth, worldHeigth);
    }

    public void Initialize(GameObject player)
    {
        this.player = player;
        previousPosition = player.transform.position;
        pools = FindObjectsOfType<PhotonObjectPool>().ToList();
        UpdatePlayerQuad();
        UpdateQuadTree();
    }

    public void UpdateQuadTree()
    {
        quadTree = new QuadTree<PooledObject>(mapBoundary, minQuadTreeCapacity, minCullingSquareSide, minCullingSquareSide);
        foreach (PhotonObjectPool pool in pools)
        {
            if (pool.activeObjects.Count > 0)
            {
                AddActiveObjectsToQuadTree(pool.activeObjects);
            }
        }
    }

    private void AddActiveObjectsToQuadTree(Dictionary<int,PooledObject> pool)
    {
        int[] keys = pool.Keys.ToArray();
        for (int index = 0; index < keys.Length; index++)
        {
            if (pool.TryGetValue(keys[index], out PooledObject pooledObject))
            {
                quadTree.Insert(new Point<PooledObject>(
                    pooledObject.transform.position.x,
                    pooledObject.transform.position.z,
                    pooledObject)
                );
                pooledObject.photonView.Group = 0;
            }
        }
        Debug.Log($"Number of active objects {keys.Length}");
    }

    private void UpdatePlayerQuad()
    {
        playerQuad = new Quad<PooledObject>(
            player.transform.position.x,
            player.transform.position.z,
            playerCullingSquareSide,
            playerCullingSquareSide
        );
    }

    private void SetInterestGroup(byte group, List<Point<PooledObject>> previousActiveObjects)
    {
        previousActiveObjects.ForEach(activeObject => activeObject.data.photonView.Group = group);
    }
    //WIP
    private void UpdateNOTDONE()
    {
        updateTimer += Time.deltaTime;
        if (updateTimer > updateDelay)
        {
            if (Vector3.Distance(player.transform.position, previousPosition) > updateDistance)
            {
                UpdatePlayerQuad();
                UpdateQuadTree();

                SetInterestGroup(0, activeObjects);
                activeObjects.Clear();

                activeObjects = quadTree.Query(playerQuad, activeObjects);
                SetInterestGroup(1, activeObjects);
                previousPosition = player.transform.position;
            }

            updateTimer = 0f;
        }
    }
}
