using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject otherPlayer;
    public GameObject Ship { get; set; }

    [SerializeField] private GameObject shipMarker;
    [SerializeField] private GameObject otherPlayerMarker;

    Vector2 shipMarkerPos;
    Vector2 otherPlayerPos;

    public static Minimap Instance { get; private set; }
    private void OnEnable()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        if (player.GetComponent<SoldierCharacter>())
            otherPlayer = FindObjectOfType<Engineer>().gameObject;
        else if (player.GetComponent<Engineer>())
            otherPlayer.GetComponent<SoldierCharacter>();

    }

    // Update is called once per frame
    void Update()
    {
        if (Ship)
        {
            Vector3 directionToShip = Ship.transform.position - player.transform.position;
            shipMarkerPos.x = directionToShip.x;
            shipMarkerPos.y = directionToShip.z;
            shipMarker.transform.position = shipMarkerPos * 1f;
        }
    }
}
