using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    public GameObject Player { get; set;}
    public GameObject OtherPlayer { get; set;}
    public GameObject Ship { get; set; }

    [SerializeField] private RectTransform shipMarker;
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

    // Update is called once per frame
    void Update()
    {
        if (Ship && Player)
        {
            Vector3 directionToShip = Ship.transform.position - Player.transform.position;
            shipMarkerPos.x = directionToShip.x;
            shipMarkerPos.y = directionToShip.z;
            shipMarker.anchoredPosition = shipMarkerPos * 1f;
        }
    }
}
