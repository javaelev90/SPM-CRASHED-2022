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
    [SerializeField] private float scale = 1f;
    private float radius;
    private GameObject outerMarkerShipParent;

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

        radius = minimapCircle.sizeDelta.x / 2f;
        outerMarkerShipParent = outerMarkerShip.transform.parent.gameObject;
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
                    float angle = Vector2.Angle(playerMarker.anchoredPosition, shipMarker.anchoredPosition);
                    Quaternion rotation = Quaternion.Euler(0f, 0f, angle);
                    outerMarkerShipParent.transform.rotation = Quaternion.Slerp(outerMarkerShipParent.transform.rotation, rotation, 0.3f);
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
