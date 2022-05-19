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
    [SerializeField] private float scale = 1f;
    private float radius;

    Vector2 shipMarkerPos;
    Vector2 otherPlayerPos;
    float angle;
    public static Minimap Instance { get; private set; }

    private void OnEnable()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        radius = minimapCircle.sizeDelta.x / 2f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Player)
        {
            if (Ship)
            {
                PositionOnMinimap(Ship.transform, shipMarker, shipMarkerPos);

                if (!IsInsideUnitCircle(shipMarker))
                {
                    shipMarker.GetComponent<RawImage>().enabled = false;
                    outerMarkerShip.gameObject.SetActive(true);

                    Vector2 direction = shipMarker.anchoredPosition - Vector2.up;
                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    outerMarkerShipParent.transform.rotation = Quaternion.Euler(0f, 0f, angle);
                }
                else
                {
                    outerMarkerShip.gameObject.SetActive(false);
                    shipMarker.GetComponent<RawImage>().enabled = true;
                }
            }


            if (OtherPlayer)
            {
                PositionOnMinimap(OtherPlayer.transform, otherPlayerMarker, otherPlayerPos);
            }
        }
        
       // Debug.Log("Angle " + angle);
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
