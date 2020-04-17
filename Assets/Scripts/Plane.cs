using System;
using UnityEngine;

public static class Plane
{
    public const int TILE_SIZE = 32;

    private static Vector2 PlanePosFloatToWorldPoint(Vector2 pos) =>
        new Vector2((pos.x + 0.5f) * TILE_SIZE, -(pos.y + 0.5f) * TILE_SIZE);

    public static Vector2 PlanePosToWorldPoint(Vector2Int pos) =>
        PlanePosFloatToWorldPoint(pos);

    public static Vector2 PlanePosIntermediateToWorldPoint(Vector2Int pos1, Vector2 pos2, float ratio) =>
        PlanePosFloatToWorldPoint(pos1 + (pos2 - pos1) * ratio);

    public static Vector2Int WorldPointToPlanePos(Vector2 point) =>
        new Vector2Int(
            Mathf.FloorToInt((point.x) / TILE_SIZE),
            Mathf.FloorToInt((-point.y) / TILE_SIZE));

}
