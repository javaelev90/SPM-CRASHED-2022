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
    [SerializeField] private float cullingUpdateDelay = 0.3f;
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
    private GameObject player;
    public GameObject otherPlayer;
    private Quad playerQuad;
    private Quad otherPlayerQuad;
    private Vector3 playerPrevPosition;
    private Vector3 otherPlayerPrevPosition;

    private float updateTimer = 0f;
    private bool positionChanged = false;
    private bool foundOtherPlayer = false;

    private void Start()
    {
        worldWidth = Terrain.activeTerrain.terrainData.size.x;
        worldHeigth = Terrain.activeTerrain.terrainData.size.z;
        mapBoundary = new Quad((worldWidth / 2f), (worldHeigth / 2f), worldWidth, worldHeigth);
    }

    public void Initialize(GameObject player, Character character)
    {
        this.player = player;
        playerPrevPosition = player.transform.position;
        otherPlayerPrevPosition = player.transform.position;

        pools = FindObjectsOfType<PhotonObjectPool>().ToList();
        quadTree = new QuadTree<PooledObject>(mapBoundary, minQuadTreeCapacity, minCullingSquareSide, minCullingSquareSide);

        //startedCulling = true;
        StartCoroutine(FindOtherPlayer(character));
    }

    IEnumerator FindOtherPlayer(Character character)
    {
        while (otherPlayer == null)
        {
            if (character == Character.SOLDIER)
                otherPlayer = FindObjectOfType<Engineer>()?.gameObject;
            else
                otherPlayer = FindObjectOfType<SoldierCharacter>()?.gameObject;

            yield return new WaitForSeconds(0.2f);
        }
        GameManager.otherPlayer = otherPlayer;
        foundOtherPlayer = true;
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
        int[] keys = pool.Keys.ToArray();
        Point<PooledObject> point;
        Transform childTransform;

        for (int index = 0; index < keys.Length; index++)
        {
            if (pool.TryGetValue(keys[index], out PooledObject pooledObject))
            {
                if (pooledObject.shouldBeCulled == false)
                    continue;
                
                childTransform = FindChildWithTag(pooledObject.gameObject, "EnemyMainBody");
                point = new Point<PooledObject>(
                    childTransform.position.x,
                    childTransform.position.z,
                    pooledObject);

                quadTree.Insert(point);
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

    private void SetInterestGroup(byte group, List<Point<PooledObject>> previousActiveObjects)
    {
        previousActiveObjects.ForEach(activeObject => activeObject.data.photonView.Group = group);
    }

    private void SetActiveState(bool active, HashSet<Point<PooledObject>> pointsToUpdate)
    {
        foreach (Point<PooledObject> point in pointsToUpdate)
        {
            point.data.UpdateActiveState(active);
        }
    }

    //WIP
    private void Update()
    {
        CullEnemyObjects();
    }

    private void CullEnemyObjects()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            updateTimer += Time.deltaTime;
            if (updateTimer > cullingUpdateDelay)
            {
                updateTimer = 0f;

                if ((player.transform.position - playerPrevPosition).sqrMagnitude > minMovementDistance)
                {
                    UpdateCulling(ref player, ref playerQuad, ref playerPrevPosition);
                }
                if (foundOtherPlayer && (otherPlayer.transform.position - otherPlayerPrevPosition).sqrMagnitude > minMovementDistance)
                {
                    UpdateCulling(ref otherPlayer, ref otherPlayerQuad, ref otherPlayerPrevPosition);
                }
                if (positionChanged)
                {
                    inActiveObjects.ExceptWith(activeObjects);
                    SetActiveState(false, inActiveObjects);
                    SetActiveState(true, activeObjects);

                    //Save previous active objects
                    inActiveObjects.Clear();
                    inActiveObjects.UnionWith(activeObjects);
                    activeObjects.Clear();
                    positionChanged = false;
                }
            }
        }
    }

    private void UpdateCulling(ref GameObject playerObject, ref Quad playerQuad, ref Vector3 previousPosition)
    {
        UpdateQuadTree();
        playerQuad = UpdatePlayerQuad(playerObject.transform);
        activeObjects = quadTree.Query(playerQuad, activeObjects);
        previousPosition = playerObject.transform.position;
        positionChanged = true;
    }

    private void OnDrawGizmos()
    {
        quadTree.OnDrawGizmos();
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z), new Vector3(cullingBoundarySideLength, 0f, cullingBoundarySideLength));
    }
}
