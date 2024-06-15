using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 30f;
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private float zoomSpeed = 50f;
    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;

    private Vector3 targetFollowOffset;
    private CinemachineTransposer cinemachineTransposer;
    private const float MIN_FOLLOW_Y_OFFSET = 30f;
    private const float MAX_FOLLOW_Y_OFFSET = 100f;

    private void Start()
    {
        cinemachineTransposer = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        targetFollowOffset = cinemachineTransposer.m_FollowOffset;
    }
    private void Update()
    {
        Vector3 moveDir = new Vector3(0, 0, 0);
        if (Input.GetKey(KeyCode.W))
        {
            moveDir.z = 1f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveDir.z = -1f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveDir.x = -1f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveDir.x = 1f;
        }

        Vector3 moveVector = transform.forward * moveDir.z + transform.right * moveDir.x;
        transform.position += moveVector * moveSpeed * Time.deltaTime;


        Vector3 rotateDir = new Vector3(0, 0, 0);
        if (Input.GetKey(KeyCode.Q))
        {
            rotateDir.y = -1f;
        }
        if (Input.GetKey(KeyCode.E))
        {
            rotateDir.y = 1f;
        }

        transform.eulerAngles += rotateDir * rotationSpeed * Time.deltaTime;

        float zoomAmount = 1f;

        if (Input.mouseScrollDelta.y > 0)
        {
            targetFollowOffset.y -= zoomAmount;
        }
        if (Input.mouseScrollDelta.y < 0)
        {
            targetFollowOffset.y += zoomAmount;
        }


        targetFollowOffset.y = Mathf.Clamp(targetFollowOffset.y, MIN_FOLLOW_Y_OFFSET, MAX_FOLLOW_Y_OFFSET);
        cinemachineTransposer.m_FollowOffset = Vector3.Lerp(cinemachineTransposer.m_FollowOffset, targetFollowOffset, Time.deltaTime * zoomSpeed);
    }
}
