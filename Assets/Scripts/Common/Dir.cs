using System;
using System.Collections.Generic;
using UnityEngine;

public class Dir
{
    public int DeltaX { get; set; }
    public int DeltaY { get; set; }

    private int index;

    private Dir(int deltaX, int deltaY, int idx)
    {
        this.DeltaX = deltaX;
        this.DeltaY = deltaY;
        this.index = idx;
    }

    public static Dir Random() =>
        EightDirs[UnityEngine.Random.Range(0, 8)];

    public static Dir FromDelta(int deltaX, int deltaY)
    {
        if (deltaX < 0)
        {
            return Up.TurnCounterclockwise(Math.Sign(deltaY) + 2);
        }
        else if (deltaX == 0)
        {
            return (new[] { Up, None, Down })[Math.Sign(deltaY) + 1];
        }
        else
        {
            return Up.TurnClockwise(Math.Sign(deltaY) + 2);
        }
    }

    public static Dir FromDelta(Vector2Int vec) =>
        FromDelta(vec.x, vec.y);
    

    public readonly static Dir None = new Dir(0, 0, -1);
    public readonly static Dir Up = new Dir(0, -1, 0);
    public readonly static Dir UpLeft = new Dir(-1, -1, 1);
    public readonly static Dir Left = new Dir(-1, 0, 2);
    public readonly static Dir DownLeft = new Dir(-1, 1, 3);
    public readonly static Dir Down = new Dir(0, 1, 4);
    public readonly static Dir DownRight = new Dir(1, 1, 5);
    public readonly static Dir Right = new Dir(1, 0, 6);
    public readonly static Dir UpRight = new Dir(1, -1, 7);

    public static List<Dir> EightDirs = new List<Dir>()
    {
        Up, UpLeft, Left, DownLeft, Down, DownRight, Right, UpRight,
    };

    public static Vector2Int operator +(Vector2Int vec, Dir dir)
    {
        return new Vector2Int(vec.x + dir.DeltaX, vec.y + dir.DeltaY);
    }

    public static Vector2Int operator +(Dir dir, Vector2Int vec)
    {
        return new Vector2Int(vec.x + dir.DeltaX, vec.y + dir.DeltaY);
    }

    // same as '%' but always returns positive value
    private static int modPositive(int x, int r) =>
        ((x % r) + r) % r;

    // change direction counterclockwise by 45*count degrees
    public Dir TurnCounterclockwise(int count=1)
    {
        if (this.index < 0)
        {
            return this;
        }
        else
        {
            return EightDirs[modPositive(this.index + count, 8)];
        }
    }

    public Dir TurnClockwise(int count = 1) =>
        TurnCounterclockwise(-count);

    public Dir Reverse() =>
        TurnCounterclockwise(4);
}
