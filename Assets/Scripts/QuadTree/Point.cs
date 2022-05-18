using UnityEngine;

public class Point<T>
{
    public float x;
    public float y;
    public T data;

    public Point(float x, float y, T data)
    {
        this.x = x;
        this.y = y;
        this.data = data;
    }
}
