using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour // � ���������� ����� ������� ����������� � �������� InputSystem
{

    private const float MIN_FOLLOW_Y_OFFSET = 2f;
    private const float MAX_FOLLOW_Y_OFFSET = 12f;

    [SerializeField] private CinemachineVirtualCamera _cinemachineVirtualCamera;

    private CinemachineTransposer _cinemachineTransposer;
    private Vector3 _targetFollowOffset; // ������� �������� ����������


    private void Start()
    {
        _cinemachineTransposer = _cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>(); // ������� � �������� ��������� CinemachineTransposer �� ����������� ������, ����� � ���������� �������� �� ��������� ��� ZOOM ������
        _targetFollowOffset = _cinemachineTransposer.m_FollowOffset; // �������� ����������
    }
    
    private void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleZoom();
    }

    private void HandleMovement() // ������ ��������
    {
        Vector2 inputMoveDirection = InputManager.Instance.GetCameraMoveVector(); // ����������� ��������� ��������� (�������� ����� ������ ��������������)
        
        float moveSpeed = 10f; // �������� ������

        //����� �������� ��������� �������� ����������� ������ inputMoveDirection � moveVector
        Vector3 moveVector = transform.forward * inputMoveDirection.y + transform.right * inputMoveDirection.x; // �������� ��������� ��������. ��������� ������ forward(z) ������� �� inputMoveDirection.y, � ��������� ������ right(x) ������� �� inputMoveDirection.x
        transform.position += moveVector * moveSpeed * Time.deltaTime;
    }

    private void HandleRotation() // ������ �������
    {
        Vector3 rotationVector = new Vector3(0, 0, 0); // ������ �������� // ����� ������� ������ ������ ��� Y (�������� ����� ������ ��������������)

        rotationVector.y = InputManager.Instance.GetCameraRotateAmount(); //�������� �������� �������� ������ �� ��� �
        
        float rotationSpeed = 100f;
        transform.eulerAngles += rotationVector * rotationSpeed * Time.deltaTime;
        //��� ���� ������
        //transform .Rotate(rotationVector, rotationSpeed * Time.deltaTime);
    }

    private void HandleZoom() // ������ ���������������
    {
        //Debug.Log(InputManager.Instance.GetCameraZoomAmount()); // ������� ����� ������� �������� ������

        float zoomIncreaseAmount = 1f; //������� �������� ���������� (�������� ����������)

        _targetFollowOffset.y += InputManager.Instance.GetCameraZoomAmount() * zoomIncreaseAmount; // �������� �������� ���������� ������

        // �� �� ���������� Time.deltaTime �.�. ��������� ��� ��������� �������� ��������� �������� � �� ��������� �� �������� (���� ����� ��� � ����������� ��� ������� ������� �������� Input.GetKeyDown)
       
        _targetFollowOffset.y = Mathf.Clamp(_targetFollowOffset.y, MIN_FOLLOW_Y_OFFSET, MAX_FOLLOW_Y_OFFSET);// ��������� �������� ���������������
        float zoomSpeed = 5f;
        _cinemachineTransposer.m_FollowOffset = Vector3.Lerp(_cinemachineTransposer.m_FollowOffset, _targetFollowOffset, Time.deltaTime * zoomSpeed); // ��������� ���� ��������� ��������, ��� ��������� ���������� Lerp
    }

}
