using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTarget : MonoBehaviour
{
    public float scrollSpeed = 1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 inputDelta = Vector2.zero;
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) {inputDelta.y = 1;}
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) {inputDelta.y = -1;}
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) {inputDelta.x = -1;}
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) {inputDelta.x = 1;}
        Vector3 moveDelta = new Vector3(inputDelta.x, 0, inputDelta.y)*scrollSpeed*Time.deltaTime;
        transform.position += moveDelta;
    }
}
