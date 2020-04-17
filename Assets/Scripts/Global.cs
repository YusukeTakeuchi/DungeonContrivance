using System.Collections;
using System.Collections.Generic;
using Floor;
using UnityEngine;

public class Global : MonoBehaviour
{
    public static Global GetInstance() =>
        GameObject.Find("Global").GetComponent<Global>();
    

    private static T GetComponent<T>(string name) =>
        GameObject.Find(name).GetComponent<T>();

    private FloorManager floorManager;

    private GridLayout grid;

    public FloorData floor
    {
        get => floorManager.floor;
    }

    public Vector2Int CellPositionAt(Vector2 worldPos)
    {
        var v3 = grid.WorldToCell(worldPos);
        return new Vector2Int(v3.x, v3.y);
    }
    

    private void Awake()
    {
        floorManager = GetComponent<FloorManager>("FloorManager");
        grid = GetComponent<GridLayout>("GridBackground");
    }

}
