public class Quad<T>
{
    public float x;
    public float y;
    public float width;
    public float height;

    public Quad(float x, float y, float width, float height)
    {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
    }

    public bool Contains(Point<T> point)
    {
        return (
            point.x >= x - width &&
            point.x < x + width &&
            point.y >= y - height &&
            point.y < y + height
        );
    }

    public bool Intersects(Quad<T> other)
    {
        return !((other.x - other.width > x + width) ||
            (other.x + other.width < x - width) ||
            (other.y + other.height < y - height) ||
            (other.y - other.height > y + height));
    }
}
