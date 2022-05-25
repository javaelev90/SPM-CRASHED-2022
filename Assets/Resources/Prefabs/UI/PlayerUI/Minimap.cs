using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minimap : MonoBehaviour
{
    public GameObject Player { get; set; }
    public GameObject OtherPlayer { get; set; }
    public GameObject Ship { get; set; }

    [SerializeField] private RectTransform playerMarker;
    [SerializeField] private RectTransform shipMarker;
    [SerializeField] private RectTransform otherPlayerMarker;
    [SerializeField] private RectTransform minimapCircle;
    [SerializeField] private RectTransform outerMarkerShip;
    [SerializeField] private RectTransform outerMarkerOtherPlayer;
    [SerializeField] private Transform outerMarkerShipParent;
    [SerializeField] private Transform outerMarkerOtherPlayerParent;
    [SerializeField] private float scale = 1f;

    private float radius;
    private Vector2 shipMarkerPos;
    private Vector2 otherPlayerPos;
    public static Minimap Instance { get; private set; }

    private void OnEnable()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        radius = minimapCircle.sizeDelta.x / 2f;
        outerMarkerShipParent.gameObject.SetActive(false);
        outerMarkerOtherPlayerParent.gameObject.SetActive(false);
    }

    private void Start()
    {
        Player = GameManager.player;
        StartCoroutine(FindOtherPlayer());
    }

    IEnumerator FindOtherPlayer()
    {
        while (OtherPlayer == null)
        {
            OtherPlayer = GameManager.otherPlayer;
            if (OtherPlayer != null)
            {
                break;
            }
            yield return new WaitForSeconds(0.2f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Player)
        {
            if (Ship)
            {
                PositionOnMinimap(Ship.transform, shipMarker, shipMarkerPos);
                ActivateOuterMarker(shipMarker, outerMarkerShipParent);
            }

            if (OtherPlayer)
            {
                PositionOnMinimap(OtherPlayer.transform, otherPlayerMarker, otherPlayerPos);
                ActivateOuterMarker(otherPlayerMarker, outerMarkerOtherPlayerParent);
            }
        }
    }

    private void ActivateOuterMarker(RectTransform objectMarker, Transform parent)
    {
        if (!IsInsideUnitCircle(objectMarker))
        {
            objectMarker.GetComponent<RawImage>().enabled = false;
            parent.gameObject.SetActive(true);
            Vector2 direction = objectMarker.anchoredPosition - playerMarker.anchoredPosition;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            parent.transform.rotation = Quaternion.Euler(0f, 0f, angle - 90);
        }
        else
        {
            parent.gameObject.SetActive(false);
            objectMarker.GetComponent<RawImage>().enabled = true;
        }
    }

    private bool IsInsideUnitCircle(RectTransform rect)
    {
        return Mathf.Pow(rect.anchoredPosition.x / radius, 2f) + Mathf.Pow(rect.anchoredPosition.y / radius, 2f) < 1f;
    }

    private void PositionOnMinimap(Transform objectToShow, RectTransform objectToShowMarker, Vector2 positionHolder)
    {
        Vector3 shipPosition = Player.transform.InverseTransformPoint(objectToShow.position);
        positionHolder.x = shipPosition.x;
        positionHolder.y = shipPosition.z;
        objectToShowMarker.anchoredPosition = positionHolder * scale;
    }
}
