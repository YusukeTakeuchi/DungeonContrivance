using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{

    public void OnClick()
    {
        SetSwordPos();
    }


    private void SetSwordPos()
    {
        var sword = GameObject.Find("Sword");
        float z = sword.transform.position.z;
        var pointClicked = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        var pos = Plane.WorldPointToPlanePos(pointClicked);
        Debug.Log(pointClicked);
        Debug.Log(pos);
        GameObject.Find("Sword").GetComponent<Sword>().pos = pos;
    }
}
