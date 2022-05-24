public class Quad
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

    public bool Contains<T>(Point<T> point) 
    {
        return (
            point.x >= x - width / 2 &&
            point.x < x + width / 2 &&
            point.y >= y - height / 2 &&
            point.y < y + height / 2
        );
    }

    public bool Intersects(Quad other)
    {
        return !((other.x - other.width / 2 > x + width / 2) ||
            (other.x + other.width / 2 < x - width / 2) ||
            (other.y + other.height / 2 < y - height / 2) ||
            (other.y - other.height / 2 > y + height / 2));
    }
}
