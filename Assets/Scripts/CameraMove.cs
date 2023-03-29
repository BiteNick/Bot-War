using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraMove : MonoBehaviour
{
    [SerializeField]private CinemachineVirtualCamera cinemachineVirtualCamera;
    private bool dragPanMoveActive;
    private Vector2 lastMousePosition;
    private Vector3 inputDir = new Vector3(0, 0, 0);
    private Vector3 moveDir;
    private Vector3 nextFramePosition;
    private Vector3 followOffset; //for zooming
    private Vector3 zoomDir; //temp for zooming
    private float zoomAmount = 5f;
    private float zoomSpeed = 2f;
    private float followOffsetMinX = 22f;
    private float followOffsetMaxX = 28f;
    private float followOffsetMinY = -10f;
    private float followOffsetMaxY = 1f;
    private float KeysMovingSpeed = 3f;
    [SerializeField]private float moveSpeed;
    [SerializeField] private float minPositionX;
    [SerializeField] private float maxPositionX;
    [SerializeField] private float minPositionZ;
    [SerializeField] private float maxPositionZ;



    private void Awake()
    {
        followOffset = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset;
    }


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
        HandleCameraZoom_MoveForward();
        HandleCameraZoom_MoveDown();
    }


    private void HandleCameraZoom_MoveForward()
    {
        zoomDir = followOffset.normalized;
        if (Input.mouseScrollDelta.y > 0)
        {
            followOffset -= zoomDir * zoomAmount;
        }
        else if (Input.mouseScrollDelta.y < 0)
        {
            followOffset += zoomDir * zoomAmount;
        }

        if (followOffset.magnitude < followOffsetMinX)
        {
            followOffset = zoomDir * followOffsetMinX;
        }
        else if (followOffset.magnitude > followOffsetMaxX)
        {
            followOffset = zoomDir * followOffsetMaxX;
        }

        cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.x = 
            Vector3.Lerp(cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset, followOffset, Time.deltaTime * zoomSpeed).x;

    }


    private void HandleCameraZoom_MoveDown()
    {
        if (Input.mouseScrollDelta.y > 0)
        {
            followOffset.y -= zoomAmount;
        }
        else if (Input.mouseScrollDelta.y < 0)
        {
            followOffset.y += zoomAmount;
        }

        followOffset.y = Mathf.Clamp(followOffset.y, followOffsetMinY, followOffsetMaxY);

        cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.y =
            Vector3.Lerp(cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset, followOffset, Time.deltaTime * zoomSpeed).y;

    }




}
