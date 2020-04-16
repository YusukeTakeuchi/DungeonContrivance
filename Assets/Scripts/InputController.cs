using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            var sword = GameObject.Find("Sword");
            float z = sword.transform.position.z;
            var posClicked = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            posClicked.z = z;
            sword.transform.position = posClicked;
        }
    }
}
