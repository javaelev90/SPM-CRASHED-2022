using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointSystem : MonoBehaviour
{
    [SerializeField] private int numberOfPoints;
    [SerializeField] public float spreadRadius;
    [SerializeField] private float delay;
    [SerializeField] private bool manuallyPlacedPoints;
    
    private float timeCounter;
    private Vector3 position = Vector3.zero;
    private Vector3 nextPosition;
    public Vector3 NextPosition
    { get { return nextPosition; } }
    private int nextIndexPosition = 0;
    List<Vector3> wayPoints = new List<Vector3>();

    private void Awake()
    {
        if (manuallyPlacedPoints == false)
        {
            CreateRandomWayPoints();
        }
    }

    public Vector3 GetNewPosition
    {
        get
        {
            if (manuallyPlacedPoints == false)
            {
                return NewRandomPosition;
            }
            else
            {
                return NextPositionWayPoint;
            }
        }
    }

    public Vector3 NewRandomPosition
    {
        get
        {
            int random = Random.Range(0, transform.childCount);
            nextPosition = transform.GetChild(random).position;
            return nextPosition;
        }
    }

    public Vector3 NextPositionWayPoint
    {
        get
        {   
            if(wayPoints.Count > 0)
            {
                nextPosition = wayPoints[nextIndexPosition];
                nextIndexPosition++;
                if (nextIndexPosition >= wayPoints.Count) nextIndexPosition = 0;
            }
            else
            {
                nextPosition = transform.position;
            }
            return nextPosition;
        }
    }

    public void CreateRandomWayPoints()
    {
        for (int i = 0; i < numberOfPoints; i++)
        {
            GameObject point = new GameObject();
            point.name = string.Format("Point ({0})", i + 1);
            point.transform.SetParent(transform);
        }
        PositionWayPoints(AssignPosition);

        nextPosition = transform.GetChild(0).position;
    }

    public void PositionWayPoints(System.Action<Vector3, Transform> positionAssigning)
    {
        foreach (Transform t in transform)
        {
            position.x = Random.Range(-spreadRadius, spreadRadius);
            position.z = Random.Range(-spreadRadius, spreadRadius);
            position.y = 1f;
            //t.position = position;
            positionAssigning.Invoke(position, t);
        }
    }

    private void AssignPosition(Vector3 newPosition, Transform childTransform)
    {
        childTransform.position = newPosition;
    }

    public void AssignLocalPosition(Vector3 newPosition, Transform childTransform)
    {
        childTransform.localPosition = newPosition;
    }

    public void AssignWayPoints(List<Vector3> wayPoints)
    {
        this.wayPoints = wayPoints;
    }
}