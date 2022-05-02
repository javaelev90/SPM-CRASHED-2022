using System.Collections;
using UnityEngine;

public class WayPointSystem : MonoBehaviour
{
    [SerializeField] private int numberOfPoints;
    [SerializeField] private float spreadRadius;
    [SerializeField] private float delay;

    private float timeCounter;
    private Vector3 position = Vector3.zero;
    private Transform randomPosition;
    public Transform RandomPosition { get { return randomPosition; } }

    private void Awake()
    {
        for (int i = 0; i < numberOfPoints; i++)
        {
            GameObject point = new GameObject();
            point.name = string.Format("Point ({0})", i + 1);
            point.transform.SetParent(transform);
        }

        foreach (Transform t in transform)
        {
            position.x = Random.Range(-spreadRadius, spreadRadius) + 1;
            position.z = Random.Range(-spreadRadius, spreadRadius) + 1;
            position.y = 0f;
            t.position = position;
        }

        randomPosition = transform.GetChild(0);
    }

    private void Start()
    {
        timeCounter = delay;
    }

    private void Update()
    {
        //timeCounter -= Time.deltaTime;
        //if(timeCounter <= 0f)
        //{
        //    int random = Random.Range(0, transform.childCount);
        //    randomPosition = transform.GetChild(random);
        //    timeCounter = delay;
        //}
    }

    public Transform NewRandomPosition
    {
        get
        {
            int random = Random.Range(0, transform.childCount);
            randomPosition = transform.GetChild(random);
            return randomPosition;
        }
    }

}