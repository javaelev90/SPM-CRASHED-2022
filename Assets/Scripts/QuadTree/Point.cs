using System.Collections.Generic;
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

    public override bool Equals(object obj)
    {
        if (this == null) return false;
        if (obj == null) return false;

        return obj is Point<T> point &&
               EqualityComparer<T>.Default.Equals(data, point.data);
    }

    public override int GetHashCode()
    {
        int hashCode = -274470864;
        hashCode = hashCode * -1521134295 + EqualityComparer<T>.Default.GetHashCode(data);
        return hashCode;
    }
}
