#define USE_NEW_INPUT_SYSTEM // � C# ��������� ��� �������� �������������, ����������� ������� �� ������������� ��������� ���� ��������� ������������. 
//��� ��������� ���������� ������� ������������� ������ ��������� ����� �� ����������� � ��������� ��� � ��� �������� �����, ��� ��� ����������. 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour // �������� ����� // ��� ����������� ������� ��������(��������, ����, ����������...) ������ ��������� ����� ���� �����
{

    public static InputManager Instance { get; private set; }   //(������� SINGLETON) ��� �������� ������� ����� ���� ������� (SET-���������) ������ ���� �������, �� ����� ���� �������� GET ����� ������ �������
                                                                // instance - ���������, � ��� ����� ���� ��������� InputManager ����� ���� ��� static. Instance ����� ��� ���� ����� ������ ������, ����� ����, ����� ����������� �� Event.

    private PlayerInputActions _playerInpytActions; // ������� - ��������, �������� ������� (NEW INPUT SYSTEM)

    private void Awake()
    {
        // ���� �� �������� � ���������� �� �������� �� �����
        if (Instance != null) // ������� �������� ��� ���� ������ ���������� � ��������� ����������
        {
            Debug.LogError("There's more than one InputManager!(��� ������, ��� ���� InputManager!) " + transform + " - " + Instance);
            Destroy(gameObject); // ��������� ���� ��������
            return; // �.�. � ��� ��� ���� ��������� InputManager ��������� ����������, ��� �� �� ��������� ������ ����
        }
        Instance = this;

        _playerInpytActions =  new PlayerInputActions(); //�������� � �������� ����� ��������� ����� 
        _playerInpytActions.Player.Enable(); // ������� ����� �������� (�� ������� �� Player � InputActions) � ������� ��, ��� �� ������������ ���� �����
    }


    public Vector2 GetMouseScreenPosition() // ������� ��������� ���� �� ������
    {
#if USE_NEW_INPUT_SYSTEM //���� ���������� ����� ������� ����� �� ������������� ��� ���� (���� ��������������� #define USE_NEW_INPUT_SYSTEM �� ��� ���� �� ����� ���������������)
        return Mouse.current.position.ReadValue(); 
#else //� ��������� ������ ������������� 
        return Input.mousePosition;
#endif
    }

    public bool IsMouseButtonDownThisFrame() // ������ ������ ���� � ���� ����
    {
#if USE_NEW_INPUT_SYSTEM //���� ���������� ����� ������� ����� �� ������������� ��� ���� (���� ��������������� #define USE_NEW_INPUT_SYSTEM �� ��� ���� �� ����� ���������������)
        return _playerInpytActions.Player.Click.WasPressedThisFrame(); //��� ����� ���� ���� ���������� true ���� �� ���� ����� ���� ������ ����� ������ ����
#else
        return Input.GetMouseButtonDown(0); // ��� ������� ��� ������ ���� �������� true � ��������� ������ false
#endif
    }

    public Vector2 GetCameraMoveVector() // �������� ������ �������� ������
    {
#if USE_NEW_INPUT_SYSTEM //���� ���������� ����� ������� ����� �� ������������� ��� ���� (���� ��������������� #define USE_NEW_INPUT_SYSTEM �� ��� ���� �� ����� ���������������)
       return _playerInpytActions.Player.CameraMovement.ReadValue<Vector2>(); //������ � �������� �����, � ����� �������� (Player), � �������� �������� ������ (CameraMovement), � ��������� ��������� �������� ��������� �� ��� <Vector2>
#else //� ��������� ������ ������������� 
        Vector2 inputMoveDirection = new Vector2(0, 0); // ����������� ��������� ��������� (�������� ����� ������ ��������������)

        if (Input.GetKey(KeyCode.W))
        {
            inputMoveDirection.y = +1f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            inputMoveDirection.y = -1f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            inputMoveDirection.x = -1f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            inputMoveDirection.x = +1f;
        }

        return inputMoveDirection;
#endif
    }

    public float GetCameraRotateAmount() //�������� �������� �������� ������
    {
#if USE_NEW_INPUT_SYSTEM //���� ���������� ����� ������� ����� �� ������������� ��� ���� (���� ��������������� #define USE_NEW_INPUT_SYSTEM �� ��� ���� �� ����� ���������������)
        return _playerInpytActions.Player.CameraRotate.ReadValue<float>();//������ � �������� �����, � ����� �������� (Player), � �������� �������� ������ (CameraRotate), � ��������� ��������� �������� ��������� �� ��� <float>
#else
        float rotateAmount = 0f; // �������� ��������

        if (Input.GetKey(KeyCode.Q))
        {
            rotateAmount = +1f;
        }
        if (Input.GetKey(KeyCode.E))
        {
            rotateAmount = -1f;
        }

        return rotateAmount;
#endif
    }

    public float GetCameraZoomAmount() // �������� �������� ���������� ������
    {
#if USE_NEW_INPUT_SYSTEM //���� ���������� ����� ������� ����� �� ������������� ��� ���� (���� ��������������� #define USE_NEW_INPUT_SYSTEM �� ��� ���� �� ����� ���������������)
        return _playerInpytActions.Player.CameraZoom.ReadValue<float>();//������ � �������� �����, � ����� �������� (Player), � �������� ���������� ������ (CameraZoom), � ��������� ��������� �������� ��������� �� ��� <float>
#else
        float zoomAmount = 0f; // �������� ����������

        // �� ��������� ��� ��������� �������� ��������� �������� � �� ��������� �� �������� (���� ����� ��� � ����������� ��� ������� ������� �������� Input.GetKeyDown)
        if (Input.mouseScrollDelta.y > 0)
        {
            zoomAmount = -1f;
        }
        if (Input.mouseScrollDelta.y < 0)
        {
            zoomAmount = +1f;
        }

        return zoomAmount;
#endif
    }
}
