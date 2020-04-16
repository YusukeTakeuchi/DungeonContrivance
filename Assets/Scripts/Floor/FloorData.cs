using System;
using System.Collections.Generic;
using UnityEngine;

namespace Floor
{

    public class Block
    {
        public RectInt rect;

        public Room room;

    }

    public class Room
    {
        public RectInt rect;

    }

    public class FloorData
    {

        public FloorAttribute[,] attrs;
        public List<Room> rooms;

        public FloorData()
        {
        }

        public FloorAttribute GetAttrAt(int x, int y) =>
            attrs[y, x]; // TODO: check range

        public FloorAttribute GetAttrAt(Vector2Int pos) =>
            GetAttrAt(pos.x, pos.y);
    }
}
