using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static Util.MatrixUtil;
using UnityEngine;
using Util;
using Math = System.Math;

namespace Floor {

    public enum FloorAttribute
    {
        WALL,
        HARD_WALL,
        ROOM,
        PASSAGE,
    }

    public class Block
    {
        public RectInt rect;

        public Room room;

    }

    public class Room
    {
        public RectInt rect;

    }

    public class FloorGenerator
    {
        public const int FLOOR_WIDTH = 32;
        public const int FLOOR_HEIGHT = 48;

        public const int BLOCK_WIDTH = 3;
        public const int BLOCK_HEIGHT = 4;

        public const int MIN_ROOM_NUM = 4;
        public const int MAX_ROOM_NUM = 7;

        public const int MIN_ROOM_WIDTH = 4;
        public const int MAX_ROOM_WIDTH = 10;

        public const int MIN_ROOM_HEIGHT = 4;
        public const int MAX_ROOM_HEIGHT = 10;

        public const int ROOM_MARGIN = 2;

        public readonly FloorAttribute[,] floor;
        public readonly Block[,] blocks;
        public List<Room> rooms;
        

        public FloorGenerator()
        {
            floor = new FloorAttribute[FLOOR_HEIGHT, FLOOR_WIDTH];
            blocks = InitBlocks();
        }

        public FloorAttribute[,] generate()
        {
            InitFloorAttributes();
            CreateRooms();
            SetFloorAttributesFromRooms();
            CreatePassages();
            return floor;
        }

        private Block[,] InitBlocks()
        {
            Block[,] blocks = new Block[BLOCK_HEIGHT, BLOCK_WIDTH];
            for (int y=0; y<BLOCK_HEIGHT; y++)
            {
                for(int x=0; x<BLOCK_WIDTH; x++)
                {
                    int left = x * FLOOR_WIDTH / BLOCK_WIDTH;
                    int right = (x + 1) * FLOOR_WIDTH / BLOCK_WIDTH;
                    int top = y * FLOOR_HEIGHT / BLOCK_HEIGHT;
                    int bottom = (y + 1) * FLOOR_HEIGHT / BLOCK_HEIGHT;
                    blocks[y, x] = new Block()
                    {
                        rect = new RectInt(left, top, right - left, bottom - top),
                    };
                }
            }
            return blocks;
        }

        private void InitFloorAttributes()
        {
            // put walls to the entire floor.
            // put hard walls to outmost edge.
            SetAttributeRect(FloorAttribute.HARD_WALL, 0, FLOOR_WIDTH, 0, FLOOR_HEIGHT);
            SetAttributeRect(FloorAttribute.WALL, 1, FLOOR_WIDTH - 1, 1, FLOOR_HEIGHT - 1);
        }

        private void CreateRooms()
        {
            int roomCount = UnityEngine.Random.Range(MIN_ROOM_NUM, MAX_ROOM_NUM);
            var blocksFlattened = new Flattened<Block>(blocks);
            var blocksOfRooms = blocksFlattened.TakeRandomN(roomCount).ToList();
            rooms = blocksOfRooms
                .Select(block => MakeRoomInBlock(block.rect))
                .ToList();

            foreach (var (block,room) in blocksOfRooms.Zip(rooms, System.ValueTuple.Create))
            {
                block.room = room;
            }

        }

        private void SetFloorAttributesFromRooms()
        {
            foreach (var room in rooms)
            {
                SetAttributeRect(FloorAttribute.ROOM, room.rect);
            }
        }

        private void CreatePassages()
        {
            // unsed if there is a room for the block-coord
            var passageJunctions = new Vector2Int[BLOCK_HEIGHT, BLOCK_WIDTH];
            for (int y=0; y<BLOCK_HEIGHT; y++)
            {
                for (int x=0; x<BLOCK_WIDTH; x++)
                {
                    var roomRangeRect = PossibleRoomRect(blocks[y,x].rect);
                    passageJunctions[y, x] = new Vector2Int
                    {
                        x = Random.Range(roomRangeRect.xMin, roomRangeRect.xMax),
                        y = Random.Range(roomRangeRect.yMin, roomRangeRect.yMax),
                    };
                }
            }

            for (int y=0; y<BLOCK_HEIGHT; y++)
            {
                for (int x=0; x<BLOCK_WIDTH; x++)
                {
                    var currentBlock = blocks[y, x];

                    Vector2Int horizontalPassageStartPos = new Vector2Int(); ;
                    Vector2Int verticalPassageStartPos = new Vector2Int();

                    if (currentBlock.room == null)
                    {
                        horizontalPassageStartPos = verticalPassageStartPos = passageJunctions[y, x];
                    }
                    else
                    {
                        var rect = currentBlock.room.rect;
                        horizontalPassageStartPos.x = rect.xMax;
                        horizontalPassageStartPos.y = Random.Range(rect.yMin, rect.yMax);
                        verticalPassageStartPos.x = Random.Range(rect.xMin, rect.xMax);
                        verticalPassageStartPos.y = rect.yMax;
                    }


                    if (x < BLOCK_WIDTH - 1)
                    {
                        Vector2Int horizontalPassageEndPos = new Vector2Int();

                        var rightBlock = blocks[y, x + 1];

                        if (rightBlock.room == null)
                        {
                            horizontalPassageEndPos = passageJunctions[y, x + 1];
                        }
                        else
                        {
                            var rect = rightBlock.room.rect;
                            horizontalPassageEndPos.x = rect.xMin - 1;
                            horizontalPassageEndPos.y = Random.Range(rect.yMin, rect.yMax);
                        }

                        MakeHorizontalPassageConnecting(horizontalPassageStartPos, horizontalPassageEndPos);
                    }

                    if (y < BLOCK_HEIGHT - 1)
                    {
                        Vector2Int verticalPassageEndPos = new Vector2Int();

                        var downBlock = blocks[y + 1, x];

                        if (downBlock.room == null)
                        {
                            verticalPassageEndPos = passageJunctions[y + 1, x];
                        }
                        else
                        {
                            var rect = downBlock.room.rect;
                            verticalPassageEndPos.x = Random.Range(rect.xMin, rect.xMax);
                            verticalPassageEndPos.y = rect.yMin - 1;
                        }

                        MakeVerticalPassageConnecting(verticalPassageStartPos, verticalPassageEndPos);
                    }


                }
            }

        }

        private Room MakeRoomInBlock(RectInt blockRect)
        {
            var roomRangeRect = PossibleRoomRect(blockRect);

            int maxRoomWidth = System.Math.Min(MAX_ROOM_WIDTH, roomRangeRect.width);
            int width = Random.Range(MIN_ROOM_WIDTH, maxRoomWidth+1);
            int left = Random.Range(roomRangeRect.xMin, roomRangeRect.xMax - width + 1);

            int maxRoomHeight = System.Math.Min(MAX_ROOM_HEIGHT, roomRangeRect.height);
            int height = Random.Range(MIN_ROOM_HEIGHT, maxRoomHeight+1);
            int top = Random.Range(roomRangeRect.yMin, roomRangeRect.yMax - height + 1);

            return new Room() {
                rect = new RectInt(left, top, width, height),
            };
        }

        private void MakeHorizontalPassageConnecting(Vector2Int start, Vector2Int end)
        {
            int cornerX = Random.Range(start.x + 1, end.x);
            SetAttributeRect(FloorAttribute.PASSAGE, start.x, cornerX + 1, start.y, start.y + 1);
            SetAttributeRect(FloorAttribute.PASSAGE, cornerX, cornerX + 1,
                Math.Min(start.y, end.y),
                Math.Max(start.y, end.y) + 1);
            SetAttributeRect(FloorAttribute.PASSAGE, cornerX, end.x + 1, end.y, end.y + 1);
        }

        private void MakeVerticalPassageConnecting(Vector2Int start, Vector2Int end)
        {
            int cornerY = Random.Range(start.y + 1, end.y);
            SetAttributeRect(FloorAttribute.PASSAGE, start.x, start.x + 1, start.y, cornerY + 1);
            SetAttributeRect(FloorAttribute.PASSAGE,
                Math.Min(start.x, end.x),
                Math.Max(start.x, end.x) + 1,
                cornerY, cornerY + 1);
            SetAttributeRect(FloorAttribute.PASSAGE, end.x, end.x + 1, cornerY, end.y + 1);
        }


        private RectInt PossibleRoomRect(RectInt blockRect) =>
            new RectInt(
                blockRect.xMin + ROOM_MARGIN,
                blockRect.yMin + ROOM_MARGIN,
                blockRect.width - ROOM_MARGIN*2,
                blockRect.height - ROOM_MARGIN*2
            );

        private void SetAttributeRect(FloorAttribute attr, int left, int right, int top, int bottom)
        {
            for (int y=top; y < bottom; y++)
            {
                for (int x = left; x < right; x++)
                {
                    floor[y, x] = attr;
                }
            }
        }

        private void SetAttributeRect(FloorAttribute attr, RectInt rect)
        {
            SetAttributeRect(attr, rect.xMin, rect.xMax, rect.yMin, rect.yMax);
        }


    }

}
