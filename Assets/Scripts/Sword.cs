using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    private Vector2Int _pos;

    public Vector2Int pos
    {
        get => _pos;

        set
        {
            _pos = value;
            this.gameObject.transform.position = Plane.PlanePosToWorldPoint(_pos);
        }
    }
}
