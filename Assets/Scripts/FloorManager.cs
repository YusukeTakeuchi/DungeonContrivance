using System.Collections;
using System.Collections.Generic;
using Floor;
using UnityEngine;

public class FloorManager : MonoBehaviour
{
    private FloorData _floor;
    public FloorData floor
    {
        get => _floor;
    }

    private void Awake()
    {
        _floor = (new FloorGenerator()).generate();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
