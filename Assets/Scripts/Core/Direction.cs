using UnityEngine;

public enum Direction { Up, Right, Down, Left }

public static class DirectionExtensions
{
    public static Vector2Int ToVector(this Direction d)
    {
        switch (d)
        {
            case Direction.Up: return Vector2Int.up;
            case Direction.Right: return Vector2Int.right;
            case Direction.Down: return Vector2Int.down;
            default: return Vector2Int.left;
        }
    }

    public static Direction RotateCW(this Direction d)
    {
        return (Direction)(((int)d + 1) % 4);
    }

    // Угол поворота спрайта по Z (Up = 0, по часовой стрелке).
    public static float ToAngle(this Direction d)
    {
        return -90f * (int)d;
    }
}
