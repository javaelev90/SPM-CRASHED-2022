using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ObjectCulling : MonoBehaviour
{
    [Range(5, 60)]
    [SerializeField]private int minCullingSquareSide;
    [Range(5, 60)]
    [SerializeField] private int minQuadTreeCapacity;
    public QuadTree<GameObject> quadTree { get; private set; }
    private List<PhotonObjectPool> pools;
    private Quad<GameObject> boundary;
    private float worldWidth;
    private float worldHeigth;
    private GameObject player;
    //update quadtree
    //change group

    private void Start()
    {
        worldWidth = Terrain.activeTerrain.terrainData.size.x;
        worldHeigth = Terrain.activeTerrain.terrainData.size.z;
        boundary = new Quad<GameObject>(0, 0, worldHeigth, worldHeigth);
    }

    public void Initialize(GameObject player)
    {
        this.player = player;
        pools = FindObjectsOfType<PhotonObjectPool>().ToList();
        UpdateQuadTree();
    }

    public void UpdateQuadTree()
    {
        quadTree = new QuadTree<GameObject>(boundary, minQuadTreeCapacity, minCullingSquareSide, minCullingSquareSide);
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
                quadTree.Insert(new Point<GameObject>(
                    pooledObject.transform.position.x,
                    pooledObject.transform.position.z,
                    pooledObject.gameObject)
                );
            }
        }
        Debug.Log($"Number of active objects {keys.Length}");
    }
}
