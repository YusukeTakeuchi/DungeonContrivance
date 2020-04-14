using System.Collections;
using System.Collections.Generic;
using Floor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Background : MonoBehaviour
{
    public const int TILE_SIZE = 32;
    Tilemap tilemap;

    // Start is called before the first frame update
    void Start()
    {
        tilemap = gameObject.GetComponent<Tilemap>();

        var floorData = (new FloorGenerator()).generate();
        for (int y=0; y<floorData.GetLength(0); y++)
        {
            for (int x=0; x<floorData.GetLength(1); x++)
            {
                Vector3Int pos = new Vector3Int(x, -y, 0);
               
                tilemap.SetTile(pos, GetTileFromFloorAttribute(floorData[y, x]));
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private Tile GetTileFromFloorAttribute(FloorAttribute attr)
    {
        string name = null;
        switch (attr)
        {
            case FloorAttribute.WALL:
                name = "Wall";
                break;
            case FloorAttribute.HARD_WALL:
                name = "HardWall";
                break;
            case FloorAttribute.ROOM:
                name = "Room";
                break;
            case FloorAttribute.PASSAGE:
                name = "Passage";
                break;
        }
        return Resources.Load($"Tiles/{name}") as Tile;
    }
}
