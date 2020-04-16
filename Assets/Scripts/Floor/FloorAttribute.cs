using System;

namespace Floor
{
    public enum FloorAttributeKind
    {
        WALL,
        HARD_WALL,
        ROOM,
        PASSAGE,
    }

    public struct FloorAttribute
    {
        public FloorAttributeKind kind { get; }

        public Room room { get; set; }

        public FloorAttribute(FloorAttributeKind kind)
        {
            this.kind = kind;
            this.room = null;
        }

        public static FloorAttribute Wall = new FloorAttribute(FloorAttributeKind.WALL);
        public static FloorAttribute HardWall = new FloorAttribute(FloorAttributeKind.HARD_WALL);
        public static FloorAttribute Passage = new FloorAttribute(FloorAttributeKind.PASSAGE);

        public static FloorAttribute Room(Room room) =>
            new FloorAttribute(FloorAttributeKind.ROOM)
            {
                room = room,
            };

        public bool IsRoom() =>
            this.kind == FloorAttributeKind.ROOM;

        public bool IsRoom(Room room) =>
            IsRoom() && this.room == room;

        public bool IsPassage() =>
            this.kind == FloorAttributeKind.PASSAGE;

        // returns normal characters enter into the cell of this attr
        public bool CanEnter()
        {
            switch (this.kind)
            {
                case FloorAttributeKind.ROOM:
                case FloorAttributeKind.PASSAGE:
                    return true;
                default:
                    return false;
            }
           
        }
    }
}