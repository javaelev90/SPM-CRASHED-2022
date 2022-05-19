using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Pun;

public class ObjectCulling : MonoBehaviourPunCallbacks
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
    private GameObject otherPlayer;
    private Quad<PooledObject> playerQuad;
    private Quad<PooledObject> otherPlayerQuad;
    private Vector3 playerPrevPosition;
    private Vector3 otherPlayerPrevPosition;

    private float updateDistance = 1f;
    private float updateTimer = 0f;
    private float updateDelay = 0.2f;

    private bool startedCulling = false;

    private void Start()
    {
        worldWidth = Terrain.activeTerrain.terrainData.size.x;
        worldHeigth = Terrain.activeTerrain.terrainData.size.z;
        mapBoundary = new Quad<PooledObject>(0, 0, worldWidth, worldHeigth);
    }

    public void Initialize(GameObject player, Character character)
    {
        this.player = player;
        playerPrevPosition = player.transform.position;
        otherPlayerPrevPosition = player.transform.position;
        pools = FindObjectsOfType<PhotonObjectPool>().ToList();
        //playerQuad = UpdatePlayerQuad(player.transform);
        //otherPlayerQuad = UpdatePlayerQuad(otherPlayer.transform);
        UpdateQuadTree();
        //StartCoroutine(FindOtherPlayer(character));
    }

    IEnumerator FindOtherPlayer(Character character)
    {
        while (otherPlayer == null)
        {
            if (character == Character.SOLDIER)
            {
                otherPlayer = FindObjectOfType<Engineer>()?.gameObject;
            }
            else
            {
                otherPlayer = FindObjectOfType<SoldierCharacter>()?.gameObject;
            }

            yield return new WaitForSeconds(0.1f);
        }
        startedCulling = true;
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
        //quadTree.DebugPrint();
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
                pooledObject.photonView.Group = 1;
            }
        }
        Debug.Log($"Number of active objects {keys.Length}");
    }

    private Quad<PooledObject> UpdatePlayerQuad(Transform playerTransform)
    {
        return new Quad<PooledObject>(
            playerTransform.position.x,
            playerTransform.position.z,
            playerCullingSquareSide,
            playerCullingSquareSide
        );
    }

    private void SetInterestGroup(byte group, List<Point<PooledObject>> previousActiveObjects)
    {
        previousActiveObjects.ForEach(activeObject => activeObject.data.photonView.Group = group);
    }

    private void SetActiveState(bool active, List<Point<PooledObject>> previousActiveObjects)
    {
        previousActiveObjects.ForEach(activeObject => activeObject.data.UpdateActiveState(active));
    }
    //WIP
    private void UpdateNOTDONE()
    {
        if (PhotonNetwork.IsMasterClient && startedCulling)
        {
            updateTimer += Time.deltaTime;
            if (updateTimer > updateDelay)
            {
                updateTimer = 0f;

                if (Vector3.Distance(player.transform.position, playerPrevPosition) > updateDistance)
                {
                    playerQuad = UpdatePlayerQuad(player.transform);
                    UpdateQuadTree();

                    SetActiveState(false, activeObjects);
                    activeObjects.Clear();

                    activeObjects = quadTree.Query(playerQuad, activeObjects);
                    SetActiveState(true, activeObjects);
                    activeObjects.Clear();
                    playerPrevPosition = player.transform.position;
                }
                if (Vector3.Distance(otherPlayer.transform.position, otherPlayerPrevPosition) > updateDistance)
                {
                    otherPlayerQuad = UpdatePlayerQuad(otherPlayer.transform);
                    UpdateQuadTree();

                    SetActiveState(false, activeObjects);
                    activeObjects.Clear();

                    activeObjects = quadTree.Query(otherPlayerQuad, activeObjects);
                    SetActiveState(true, activeObjects);
                    activeObjects.Clear();
                    otherPlayerPrevPosition = otherPlayer.transform.position;
                }
            }
        }
    }
}
