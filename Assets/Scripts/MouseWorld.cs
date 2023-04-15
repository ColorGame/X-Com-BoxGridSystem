using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MouseWorld : MonoBehaviour // ����� ���������� �� ��������� ������� ����
{

    public static MouseWorld Instance { get; private set; }   //(������� SINGLETON) ��� �������� ������� ����� ���� ������� (SET-���������) ������ ���� �������, �� ����� ���� �������� GET ����� ������ �������
                                                              // instance - ���������, � ��� ����� ���� ��������� MouseWorld ����� ���� ��� static. Instance ����� ��� ���� ����� ������ ������, ����� ����, ����� ����������� �� Event.

    public static event EventHandler OnMouseGridPositionChanged; // ������� ������� ���� �� ����� ����������

    [SerializeField] private LayerMask _mousePlaneLayerMask; // ����� ���� ��������� ���� (�������� � ����������)

    private GridPosition _mouseGridPosition;

    private void Awake()
    {
        // ���� �� �������� � ���������� �� �������� �� �����
        if (Instance != null) // ������� �������� ��� ���� ������ ���������� � ��������� ����������
        {
            Debug.LogError("There's more than one MouseWorld!(��� ������, ��� ���� MouseWorld!) " + transform + " - " + Instance);
            Destroy(gameObject); // ��������� ���� ��������
            return; // �.�. � ��� ��� ���� ��������� MouseWorld ��������� ����������, ��� �� �� ��������� ������ ����
        }
        Instance = this;
    }

    private void Start()
    {
        _mouseGridPosition = LevelGrid.Instance.GetGridPosition(GetPosition());  // ��������� ��� ������ �������� ������� ���� // ������ �������� � Awake() �.�. � ���������� ����� ��������� ������� ������ (��� ��������� ������ InputManager ��� MouseWorld ����������)
    }

    // ��� �����, �������� ��� ������� �� �������� ����.
    /*private void Update()
    {
        transform.position = MouseWorld.GetPosition(); // ��� ����� �������� �� ������ �����
    }*/

    private void Update()
    {
        GridPosition newMouseGridPosition = LevelGrid.Instance.GetGridPosition(GetPosition());
        if (_mouseGridPosition != newMouseGridPosition)
        {
            _mouseGridPosition = newMouseGridPosition;
            OnMouseGridPositionChanged?.Invoke(this, EventArgs.Empty); // �������� �������
        }
    }

    public static Vector3 GetPosition() // �������� ������� (static ���������� ��� ����� ����������� ������ � �� ������ ������ ����������)
    {
        Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMouseScreenPosition()); // ��� �� ������ � ����� �� ������ ��� ���������� ������ ����
        Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, Instance._mousePlaneLayerMask); // Instance._mousePlaneLayerMask - ����� ������ ��� �������� ����� ����� 1<<6  �.�. mousePlane ��� 6 �������
        return raycastHit.point; // ���� ��� ������� � �������� �� Physics.Raycast ����� true, � raycastHit.point ������ "����� ����� � ������� ������������, ��� ��� ����� � ���������", � ���� false �� ����� ������� ����������� ������ ������ ��������(� ����� ������ ������ ������� ������).
    }


}
