using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraMove : MonoBehaviour
{
    [SerializeField]private CinemachineVirtualCamera cinemachineVirtualCamera;
    private bool dragPanMoveActive;
    private bool keyboardMoving = false;
    private Vector2 lastMousePosition;
    private Vector3 inputDir = new Vector3(0, 0, 0);
    private Vector3 moveDir;
    private Vector3 nextFramePosition;
    private Vector3 followOffset; //for zooming
    private Vector3 zoomDir; //temp for zooming
    private float zoomAmount = 5f;
    private float maxZoomSpeed = 4f;
    [SerializeField, Range(1, 4)]private float zoomSpeed = 2f;
    private float followOffsetMinX = 22f;
    private float followOffsetMaxX = 28f;
    private float followOffsetMinY = -10f;
    private float followOffsetMaxY = 1f;
    private const float maxKeysMovingSpeed = 8f;
    private const float maxMoveSpeed = 8f;
    [SerializeField, Range(1, 8)] private float KeysMovingSpeed;
    [SerializeField, Range(1, 8)] private float moveSpeed;
    [SerializeField] private float minPositionX;
    [SerializeField] private float maxPositionX;
    [SerializeField] private float minPositionZ;
    [SerializeField] private float maxPositionZ;

    private CinemachineTransposer CinemachineCameraTransposer;

    private void Awake()
    {
        followOffset = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset;
        CinemachineCameraTransposer = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        gameManagerStatic.CameraMaxKeysMovingSpeed = maxKeysMovingSpeed;
        gameManagerStatic.CameraMaxMoveSpeed = maxMoveSpeed;
        gameManagerStatic.CameraMaxScrollSpeed = maxZoomSpeed;
        KeysMovingSpeed = gameManagerStatic.CameraKeysMovingSpeed;
        moveSpeed = gameManagerStatic.CameraMoveSpeed;

    }

    private void Start()
    {
        minPositionX = 0;
        maxPositionX = gameManagerStatic.currentMapWidthX - 24f;
        minPositionZ = 30;
        maxPositionZ = gameManagerStatic.currentMapWidthZ - 15f;
    }


    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StartGameScript gameManagerScript = GameObject.FindGameObjectWithTag("GameManager").GetComponent<StartGameScript>();
            gameManagerScript.PauseSwitcer();
        }

        inputDir = new Vector3(0, 0, 0);
        if (Input.GetKey(KeyCode.W))
        {
            inputDir.z = KeysMovingSpeed;
            keyboardMoving = true;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            inputDir.z = -KeysMovingSpeed;
            keyboardMoving = true;
        }
        if (Input.GetKey(KeyCode.A))
        {
            inputDir.x = KeysMovingSpeed;
            keyboardMoving = true;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            inputDir.x = -KeysMovingSpeed;
            keyboardMoving = true;
        }

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


        if (keyboardMoving)
        {
            nextFramePosition = transform.position - moveDir * KeysMovingSpeed * Time.deltaTime;
            keyboardMoving = false;
        }
        else
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

        CinemachineCameraTransposer.m_FollowOffset.x = 
            Vector3.Lerp(CinemachineCameraTransposer.m_FollowOffset, followOffset, Time.deltaTime * zoomSpeed).x;

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

        CinemachineCameraTransposer.m_FollowOffset.y =
            Vector3.Lerp(CinemachineCameraTransposer.m_FollowOffset, followOffset, Time.deltaTime * zoomSpeed).y;

    }

    public void SetKeysMovingSpeed(float speed)
    {
        KeysMovingSpeed = speed * maxKeysMovingSpeed;
        gameManagerStatic.CameraKeysMovingSpeed = KeysMovingSpeed;
    }

    public void SetMoveSpeed(float speed)
    {
        moveSpeed = speed * maxMoveSpeed;
        gameManagerStatic.CameraMoveSpeed = moveSpeed;
    }

    public void SetScrollSpeed(float speed)
    {
        zoomSpeed = speed * maxZoomSpeed;
        gameManagerStatic.CameraScrollSpeed = zoomSpeed;
    }
}
