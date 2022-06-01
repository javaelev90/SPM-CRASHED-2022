using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Pun;

public class ObjectCulling : MonoBehaviourPunCallbacks
{
    [Header("Player culling setting")]
    [Range(5, 200)]
    [SerializeField] private int cullingBoundarySideLength = 100;
    [Range(0.1f, 2f)]
    [SerializeField] private float cullingUpdateDelay = 0.1f;
    [Tooltip("The distance the player has to move before culled objects are updated")]
    [Range(0.1f, 5f)]
    [SerializeField] private float minMovementDistance = 1f;

    [Header("Quad Tree settings")]
    [Range(5, 100)]
    [SerializeField] private int minQuadTreeCapacity = 5;
    [Range(5, 100)]
    [SerializeField] private int minCullingSquareSide = 5;

    private QuadTree<PooledObject> quadTree;
    private HashSet<Point<PooledObject>> activeObjects = new HashSet<Point<PooledObject>>();
    private HashSet<Point<PooledObject>> inActiveObjects = new HashSet<Point<PooledObject>>();

    private List<PhotonObjectPool> pools;
    private Quad mapBoundary;
    private float worldWidth;
    private float worldHeigth;

    private PlayerInfo player;
    private PlayerInfo otherPlayer;

    private float updateTimer = 0f;
    private bool foundOtherPlayer = false;
    private bool isInitialized = false;

    private int[] activeObjectKeys;
    private Transform childTransform;

    private void Awake()
    {
        worldWidth = Terrain.activeTerrain.terrainData.size.x;
        worldHeigth = Terrain.activeTerrain.terrainData.size.z;
        mapBoundary = new Quad((worldWidth / 2f), (worldHeigth / 2f), worldWidth, worldHeigth);

    }

    public void Initialize(GameObject playerObject, Character character)
    {
        player = new PlayerInfo();
        otherPlayer = new PlayerInfo();
        player.PlayerObject = playerObject;
        player.PreviousPosition = playerObject.transform.position;
        otherPlayer.PreviousPosition = playerObject.transform.position;

        pools = FindObjectsOfType<PhotonObjectPool>().ToList();
        quadTree = new QuadTree<PooledObject>(mapBoundary, minQuadTreeCapacity, minCullingSquareSide, minCullingSquareSide);

        isInitialized = true;
    }

    public void UpdateQuadTree()
    {
        quadTree = new QuadTree<PooledObject>(mapBoundary, minQuadTreeCapacity, minCullingSquareSide, minCullingSquareSide);
        foreach (PhotonObjectPool pool in pools)
        {
            if (pool.activeObjects.Count > 0)
                AddActiveObjectsToQuadTree(pool.activeObjects);
        }
    }

    private void AddActiveObjectsToQuadTree(Dictionary<int,PooledObject> pool)
    {
        activeObjectKeys = pool.Keys.ToArray();

        for (int index = 0; index < activeObjectKeys.Length; index++)
        {
            if (pool.TryGetValue(activeObjectKeys[index], out PooledObject pooledObject))
            {
                // Pooled object has been destroyed
                if (pooledObject == null)
                {
                    pool.Remove(activeObjectKeys[index]);
                    continue;
                }
                    
                if (pooledObject.shouldBeCulled == false)
                    continue;
                
                childTransform = FindChildWithTag(pooledObject.gameObject, "EnemyMainBody");
                quadTree.Insert(
                    new Point<PooledObject>(
                    childTransform.position.x,
                    childTransform.position.z,
                    pooledObject)
                );
            }
        }
    }

    private Transform FindChildWithTag(GameObject parent, string tag)
    {
        foreach(Transform child in parent.transform)
        {
            if (child.CompareTag(tag)) 
                return child;
        }
        return parent.transform;
    }

    private Quad UpdatePlayerQuad(Transform playerTransform)
    {
        return new Quad(
            playerTransform.position.x,
            playerTransform.position.z,
            cullingBoundarySideLength,
            cullingBoundarySideLength
        );
    }

    private void SetActiveState(bool active, HashSet<Point<PooledObject>> pointsToUpdate)
    {
        foreach (Point<PooledObject> point in pointsToUpdate)
        {
            // Object has been destroyed
            if (point.data == null)
                continue;

            point.data.UpdateActiveState(active);
        }
    }

    private void FixedUpdate()
    {
        AssignOtherPlayer();
        CullEnemyObjects();
    }

    private void AssignOtherPlayer()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (foundOtherPlayer == false)
            {
                if (GameManager.otherPlayer != null)
                {
                    otherPlayer.PlayerObject = GameManager.otherPlayer;
                    foundOtherPlayer = true;
                }
            }
            else
            {
                if (GameManager.otherPlayer == null)
                {
                    otherPlayer.PlayerObject = null;
                    foundOtherPlayer = false;
                }
            }
        }
    }

    private void CullEnemyObjects()
    {
        if (PhotonNetwork.IsMasterClient && isInitialized == true)
        {
            updateTimer += Time.fixedDeltaTime;
            if (updateTimer > cullingUpdateDelay)
            {
                updateTimer = 0f;

                if ((player.PlayerObject.transform.position - player.PreviousPosition).sqrMagnitude > minMovementDistance)
                {
                    player.PositionChanged = true;
                }
                if (foundOtherPlayer && (otherPlayer.PlayerObject.transform.position - otherPlayer.PreviousPosition).sqrMagnitude > minMovementDistance)
                {
                    otherPlayer.PositionChanged = true;
                }

                if (player.PositionChanged || otherPlayer.PositionChanged)
                {
                    UpdateQuadTree();
                    UpdateCulling(ref player);
                    UpdateCulling(ref otherPlayer);

                    inActiveObjects.ExceptWith(activeObjects);
                    SetActiveState(false, inActiveObjects);
                    SetActiveState(true, activeObjects);

                    //Save previous active objects
                    inActiveObjects.Clear();
                    inActiveObjects.UnionWith(activeObjects);
                    activeObjects.Clear();
                }
            }
        }
    }

    private void UpdateCulling(ref PlayerInfo playerInfo)
    {
        if (playerInfo.PlayerObject != null)
        {
            playerInfo.Bounds = UpdatePlayerQuad(playerInfo.PlayerObject.transform);
            activeObjects = quadTree.Query(playerInfo.Bounds, activeObjects);
            playerInfo.PreviousPosition = playerInfo.PlayerObject.transform.position;
            playerInfo.PositionChanged = false;
        }

    }

    private void OnDrawGizmos()
    {
        if (quadTree != null)
        {
            quadTree.OnDrawGizmos(30);
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(
                new Vector3(player.PlayerObject.transform.position.x, player.PlayerObject.transform.position.y, player.PlayerObject.transform.position.z),
                new Vector3(cullingBoundarySideLength, 0f, cullingBoundarySideLength)
            );

            if (foundOtherPlayer)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireCube(
                    new Vector3(otherPlayer.PlayerObject.transform.position.x, otherPlayer.PlayerObject.transform.position.y, otherPlayer.PlayerObject.transform.position.z),
                    new Vector3(cullingBoundarySideLength, 0f, cullingBoundarySideLength)
                );
            }
        }
    }

    private class PlayerInfo
    {
        public GameObject PlayerObject { get; set; }
        public Quad Bounds { get; set; }
        public Vector3 PreviousPosition { get; set; }
        public bool PositionChanged { get; set; }
    }
}