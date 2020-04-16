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

    public FloorData floor
    {
        get => floorManager.floor;
    }

    private void Awake()
    {
        floorManager = GetComponent<FloorManager>("FloorManager");
    }

}
