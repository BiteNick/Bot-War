using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    private bool dragPanMoveActive;
    private Vector2 lastMousePosition;
    private Vector3 inputDir = new Vector3(0, 0, 0);
    private Vector3 moveDir;
    private Vector3 nextFramePosition;
    private float KeysMovingSpeed = 3f;
    [SerializeField]private float moveSpeed;
    [SerializeField] private float minPositionX;
    [SerializeField] private float maxPositionX;
    [SerializeField] private float minPositionZ;
    [SerializeField] private float maxPositionZ;



    void Update()
    {
        inputDir = new Vector3(0, 0, 0);
        if (Input.GetKey(KeyCode.W)) inputDir.z = KeysMovingSpeed;
        else if (Input.GetKey(KeyCode.S)) inputDir.z = -KeysMovingSpeed;
        if (Input.GetKey(KeyCode.A)) inputDir.x = KeysMovingSpeed;
        else if (Input.GetKey(KeyCode.D)) inputDir.x = -KeysMovingSpeed;

        if (Input.GetMouseButtonDown(0))
        {
            dragPanMoveActive = true;
            lastMousePosition = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(0))
        {
            dragPanMoveActive = false;
        }

        if (dragPanMoveActive)
        {
            Vector2 mouseMovementDelta = (Vector2)Input.mousePosition - lastMousePosition;

            inputDir.x = mouseMovementDelta.x;
            inputDir.z = mouseMovementDelta.y * -1;

            lastMousePosition = Input.mousePosition;
        }

        moveDir = transform.right * inputDir.z + transform.forward * inputDir.x;
        
        nextFramePosition = transform.position - moveDir * moveSpeed * Time.deltaTime;

        if (nextFramePosition.x > minPositionX && nextFramePosition.x < maxPositionX && nextFramePosition.z > minPositionZ && nextFramePosition.z < maxPositionZ)
        {
            transform.position = nextFramePosition;
        }
    }
}
