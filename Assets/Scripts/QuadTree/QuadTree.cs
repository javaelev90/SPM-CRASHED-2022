using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class QuadTree<T>
{
    private Quad boundary;
    private int quadCapacity;
    private List<Point<T>> points;
    private bool divided;
    private int minWidth;
    private int minHeight;

    private QuadTree<T> northWest;
    private QuadTree<T> northEast;
    private QuadTree<T> southWest;
    private QuadTree<T> southEast;

    public QuadTree(Quad boundary, int capacity, int minWidth, int minHeight)
    {
        this.boundary = boundary;
        quadCapacity = capacity;
        points = new List<Point<T>>();
        divided = false;
        this.minWidth = minWidth;
        this.minHeight = minHeight;
    }

    public bool Insert(Point<T> point)
    {
        if (boundary.Contains(point) == false)
        {
            return false;
        }

        if (points.Count < quadCapacity)
        {
            points.Add(point);
            return true;
        }
        else if (boundary.width / 2 <= minWidth || boundary.height / 2 <= minHeight)
        {
            points.Add(point);
            return true;
        }
        else
        {
            if (divided == false)
            {
                Subdivide();
            }
            return northWest.Insert(point) ||
                northEast.Insert(point) ||
                southWest.Insert(point) ||
                southEast.Insert(point);
        }
    }

    private void Subdivide()
    {
        float x = boundary.x;
        float y = boundary.y;
        float halfWidth = boundary.width / 2;
        float halfHeight = boundary.height / 2;
        float quadWidth = boundary.width / 4;
        float quadHeight = boundary.height / 4;

        Quad northWestBoundary = new Quad(x - quadWidth, y - quadHeight, halfWidth, halfHeight);
        northWest = new QuadTree<T>(northWestBoundary, quadCapacity, minWidth, minHeight);
        Quad northEastBoundary = new Quad(x + quadWidth, y - quadHeight, halfWidth, halfHeight);
        northEast = new QuadTree<T>(northEastBoundary, quadCapacity, minWidth, minHeight);
        Quad southWestBoundary = new Quad(x - quadWidth, y + quadHeight, halfWidth, halfHeight);
        southWest = new QuadTree<T>(southWestBoundary, quadCapacity, minWidth, minHeight);
        Quad southEastBoundary = new Quad(x + quadWidth, y + quadHeight, halfWidth, halfHeight);
        southEast = new QuadTree<T>(southEastBoundary, quadCapacity, minWidth, minHeight);

        foreach (Point<T> point in points)
        {
            northWest.Insert(point);
            northEast.Insert(point);
            southWest.Insert(point);
            southEast.Insert(point);
        }
        points.Clear();
        divided = true;
    }
    
    public HashSet<Point<T>> Query(Quad range, HashSet<Point<T>> pointsFound)
    {
        if(boundary.Intersects(range) == false)
        {
            return pointsFound;
        }
        else
        {
            foreach (Point<T> point in points)
            {
                if (point.data != null && range.Contains(point))
                {
                    pointsFound.Add(point);
                }
            }
            if (divided == true)
            {
                northWest.Query(range, pointsFound);
                northEast.Query(range, pointsFound);
                southWest.Query(range, pointsFound);
                southEast.Query(range, pointsFound);
            }
            return pointsFound;
        }
    }

    public void DebugPrint()
    {
        points.ForEach(point => Debug.Log($"x {point.x} y {point.y}"));
        if (northWest is object)
        {
            northWest.DebugPrint();
        }
        if (northEast is object)
        {
            northEast.DebugPrint();
        }
        if (southWest is object)
        {
            southWest.DebugPrint();
        }
        if (southEast is object)
        {
            southEast.DebugPrint();
        }
    }

    public System.Type GetTreeType()
    {
        return typeof(T);
    }

    public void OnDrawGizmos(float positionYValue = 30)
    {
        // Prevent error when in scene mode
        if (positionYValue == null || boundary == null) return;

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(new Vector3(boundary.x, positionYValue, boundary.y), new Vector3(boundary.width, 0f, boundary.height));

#if UNITY_EDITOR
        //if (divided == false)
        //{
        //    Handles.color = Color.red;
        //    Handles.Label(new Vector3(boundary.x, positionYValue, boundary.y), "" + points.Count);
        //}
#endif
        if (northWest is object)
        {
            northWest.OnDrawGizmos(positionYValue);
        }
        if (northEast is object)
        {
            northEast.OnDrawGizmos(positionYValue);
        }
        if (southWest is object)
        {
            southWest.OnDrawGizmos(positionYValue);
        }
        if (southEast is object)
        {
            southEast.OnDrawGizmos(positionYValue);
        }
    }
}
