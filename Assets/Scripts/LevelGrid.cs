using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �������� ������� ���������� ������� LevelGrid, ������� � Project Settings/ Script Execution Order � �������� ���������� LevelGrid ���� Default Time, ����� LevelGrid ���������� ������ �� ���� ��� ��������� �������� ����� ���� ( � Start() �� ��������� ����� Pathfinding - ��������� ������ ����)

public class LevelGrid : MonoBehaviour // �������� ������ ������� ��������� ������ ������� ������ . �������� ������ ��������� ��� �������� ������������� ����� � �������� ������� �����
{

    public static LevelGrid Instance { get; private set; }   //(������� SINGLETON) ��� �������� ������� ����� ���� ������� (SET-���������) ������ ���� �������, �� ����� ���� �������� GET ����� ������ �������
                                                             // instance - ���������, � ��� ����� ���� ��������� LevelGrid ����� ���� ��� static. Instance ����� ��� ���� ����� ������ ������, ����� ����, ����� ����������� �� Event.

    public event EventHandler<OnAnyUnitMovedGridPositionEventArgs> OnAnyUnitMovedGridPosition; //������� ������� ����� - ����� ���� ��������� � �������� �������  // <OnAnyUnitMovedGridPositionEventArgs>- ������� �������� ����� ������� ������ ���������

    public class OnAnyUnitMovedGridPositionEventArgs : EventArgs // �������� ����� �������, ����� � ��������� ������� �������� ����� � �������� �������
    {
        public Unit unit;
        public GridPosition fromGridPosition;
        public GridPosition toGridPosition;
    }


    [SerializeField] private Transform _gridDebugObjectPrefab; // ������ ������� ����� //������������ ��� ������ ��������� � ����� ��������� ������ CreateDebugObject

    [SerializeField] private int _width = 10;     // ������
    [SerializeField] private int _height = 10;    // ������
    [SerializeField] private float _cellSize = 2f;// ������ ������

    private GridSystem<GridObject> _gridSystem; // �������� ������� .� �������� ������� ��� GridObject

    private void Awake()
    {
        // ���� �� �������� � ���������� �� �������� �� �����
        if (Instance != null) // ������� �������� ��� ���� ������ ���������� � ��������� ����������
        {
            Debug.LogError("There's more than one LevelGrid!(��� ������, ��� ���� LevelGrid!) " + transform + " - " + Instance);
            Destroy(gameObject); // ��������� ���� ��������
            return; // �.�. � ��� ��� ���� ��������� LevelGrid ��������� ����������, ��� �� �� ��������� ������ ����
        }
        Instance = this;

        _gridSystem = new GridSystem<GridObject>(_width, _height, _cellSize,                               // �������� ����� 10 �� 10 � �������� 2 ������� � � ������ ������ �������� ������ ���� GridObject
                (GridSystem<GridObject> g, GridPosition gridPosition) => new GridObject(g, gridPosition)); //� ��������� ��������� ��������� ������� ������� �������� ����� ������ => new GridObject(g, _gridPosition) � ��������� �� ��������. (������ ��������� ����� ������� � ��������� �����)
        // _gridSystem.CreateDebugObject(_gridDebugObjectPrefab); // �������� ��� ������ � ������ ������ // �������������� �.�. PathfindingGridDebugObject ����� ��������� ��������������� ������ _gridDebugObjectPrefab
    }

    private void Start()
    {
        Pathfinding.Instance.Setup(_width, _height, _cellSize); // �������� ����� ����� ������ ���� // �������� ��� ���� ����� �������� ������ �� ���� ��� ��������� �������� ����� ����
    }

    public void AddUnitAtGridPosition(GridPosition gridPosition, Unit unit) // �������� ������������� ����� � �������� ������� �����
    {
        GridObject gridObject = _gridSystem.GetGridObject(gridPosition); // ������� GridObject ������� ��������� � _gridPosition
        gridObject.AddUnit(unit); // �������� ����� 
    }

    public List<Unit> GetUnitListAtGridPosition(GridPosition gridPosition) // �������� ������ ������ � �������� ������� �����
    {
        GridObject gridObject = _gridSystem.GetGridObject(gridPosition); // ������� GridObject ������� ��������� � _gridPosition
        return gridObject.GetUnitList();// ������� �����
    }

    public void RemoveUnitAtGridPosition(GridPosition gridPosition, Unit unit) // �������� ����� �� �������� ������� �����
    {
        GridObject gridObject = _gridSystem.GetGridObject(gridPosition); // ������� GridObject ������� ��������� � _gridPosition
        gridObject.RemoveUnit(unit); // ������ �����
    }

    public void UnitMovedGridPosition(Unit unit, GridPosition fromGridPosition, GridPosition toGridPosition) // ���� ��������� � �������� ������� �� ������� fromGridPosition � ������� toGridPosition
    {
        RemoveUnitAtGridPosition(fromGridPosition, unit); // ������ ����� �� ������� ������� �����

        AddUnitAtGridPosition(toGridPosition, unit);  // ������� ����� � ��������� ������� �����

        OnAnyUnitMovedGridPosition?.Invoke(this, new OnAnyUnitMovedGridPositionEventArgs // ������� ����� ��������� ������ OnAnyUnitMovedGridPositionEventArgs
        {
            unit = unit, 
            fromGridPosition = fromGridPosition, 
            toGridPosition = toGridPosition,

        }); // �������� ������� ����� ���� ��������� � �������� ������� ( � ��������� ��������� ����� ���� ������ � ����)
    }

    // ��� �� �� ���������� ��������� ���������� LevelGrid (� �� ������ ��������� ����_gridSystem) �� ������������ ������ � GridPosition ������� �������� ������� ��� ������� � GridPosition
    /*public GridPosition GetGridPosition(Vector3 worldPosition)
    {
      return _gridSystem.GetGridPosition(worldPosition);
    }*/
    public GridPosition GetGridPosition(Vector3 worldPosition) => _gridSystem.GetGridPosition(worldPosition); // ����������� ������ ���� ����
    public Vector3 GetWorldPosition(GridPosition gridPosition) => _gridSystem.GetWorldPosition(gridPosition); // �������� �������
    public bool IsValidGridPosition(GridPosition gridPosition) => _gridSystem.IsValidGridPosition(gridPosition); // �������� ������� ��� ��������� ������� � IsValidGridPosition �� _gridSystem
    public int GetWidth() => _gridSystem.GetWidth();
    public int GetHeight() => _gridSystem.GetHeight();
    public float GetCellSize() => _gridSystem.GetCellSize();

    public bool HasAnyUnitOnGridPosition(GridPosition gridPosition) // ���� �� ����� ������ ���� �� ���� �������� �������
    {
        GridObject gridObject = _gridSystem.GetGridObject(gridPosition); // ������� GridObject ������� ��������� � _gridPosition
        return gridObject.HasAnyUnit();
    }
    public Unit GetUnitAtGridPosition(GridPosition gridPosition) // �������� ����� � ���� �������� �������
    {
        GridObject gridObject = _gridSystem.GetGridObject(gridPosition); // ������� GridObject ������� ��������� � _gridPosition
        return gridObject.GetUnit();
    }


    // IInteractable ��������� �������������� - ��������� � ������ InteractAction ����������������� � ����� �������� (�����, �����, ������...) - ������� ��������� ���� ���������
    public IInteractable GetInteractableAtGridPosition(GridPosition gridPosition) // �������� ��������� �������������� � ���� �������� �������
    {
        GridObject gridObject = _gridSystem.GetGridObject(gridPosition); // ������� GridObject ������� ��������� � _gridPosition
        return gridObject.GetInteractable();
    }
    public void SetInteractableAtGridPosition(GridPosition gridPosition, IInteractable interactable) // ���������� ���������� ��������� �������������� � ���� �������� �������
    {
        GridObject gridObject = _gridSystem.GetGridObject(gridPosition); // ������� GridObject ������� ��������� � _gridPosition
        gridObject.SetInteractable(interactable);
    }
    public void ClearInteractableAtGridPosition (GridPosition gridPosition) // �������� ��������� �������������� � ���� �������� �������
    {
        GridObject gridObject = _gridSystem.GetGridObject(gridPosition); // ������� GridObject ������� ��������� � _gridPosition
        gridObject.ClearInteractable(); // �������� ��������� �������������� � ���� �������� �������
    }

}
