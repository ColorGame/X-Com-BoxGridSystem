using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GridSystem<TGridObject>  // �������� ������� // ����������� ����� C#// ����� ������������ ����������� ��� �������� ����� ����� ������� �� �� ��������� MonoBehaviour/
                                      //<TGridObject> - Generic, ��� ���� ����� GridSystem ����� �������� �� ������ � GridObject �� � � ��. ������������� �� ������ �������� �����
                                      // Generic - �������� ����������� ����� ���� GridSystem ��� ������ ���� (��� ���� ��� �� �������� ����������� ��� � ������ ������� �����)

{

    private int _width;     // ������
    private int _height;    // ������
    private float _cellSize;// ������ ������
    private TGridObject[,] _gridObjectArray; // ��������� ������ �������� �����


    public GridSystem(int width, int height, float cellSize, Func<GridSystem<TGridObject>, GridPosition,TGridObject>  createGridObject)  // �����������
                                                                                                                                        // Func - ��� ���������� ������� (������ �������� � ��������� ��� ���<TGridObject> ������� ���������� ��� ������� � ������� ��� createGridObject)
    {
        _width = width; // ���� �� �� ������� �� _width � width �� ������ ��� ��� // this.width = width;
        _height = height;
        _cellSize = cellSize;

        _gridObjectArray = new TGridObject[width, height]; // ������� ������ ����� ������������� �������� width �� height
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                _gridObjectArray[x, z] = createGridObject (this, gridPosition); // ������� ������� createGridObject � � �������� ��������� ���� GridSystem � ������� �����. ��������� ��� � ������ ������� ����� � ��������� ������ ��� x,z ��� ����� ������� �������.

                // ��� �����                
                //Debug.DrawLine(GetWorldPosition(_gridPosition), GetWorldPosition(_gridPosition) + Vector3.right* .2f, Color.white, 1000); // ��� ����� �������� ��������� ����� � ������ ������ ������ �����
            }
        }
    }

    public Vector3 GetWorldPosition(GridPosition gridPosition) // �������� ������� ���������
    {
        return new Vector3(gridPosition.x, 0, gridPosition.z) * _cellSize;
    }

    public GridPosition GetGridPosition(Vector3 worldPosition) // �������� �������� ��������� (��������� ������������ ����� ��������� �����)
    {
        return new GridPosition
            (
            Mathf.RoundToInt(worldPosition.x / _cellSize),  // ��������� Mathf.RoundToInt ��� �������������� float � int
            Mathf.RoundToInt(worldPosition.z / _cellSize)
            );
    }

    public void CreateDebugObject(Transform debugPrefab) // ������� ������ ������� ( public ��� �� ������� �� ������ Testing � ������� ������� �����)   // ��� Transform � GameObject ��������������� �.�. � ������ GameObject ���� Transform � � ������� Transform ���� ������������� GameObject
                                                         // � �������� ��� ������ ��� ����� Transform �������� �������. ���� � ��������� ������� ��� GameObject, ����� � ������, ���� �� �� ������ ����� ������� GameObject �������� ��� �������, ��� �������� ������ �������������� ��� "debugGameObject.Transform.LocalScale..."
                                                         // ������� ��� ��������� ���� � ��������� ��������� ��� Transform.
    {
        for (int x = 0; x < _width; x++)
        {
            for (int z = 0; z < _height; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z); // ������� �����

                Transform debugTransform =GameObject.Instantiate(debugPrefab, GetWorldPosition(gridPosition), Quaternion.identity);  // �������� ��������� ����������� �������(debugPrefab) � ������ ������ ����� // �.�. ��� ���������� MonoBehaviour �� �� ����� �������� ������������ Instantiate ������ ����� GameObject.Instantiate
                GridDebugObject gridDebugObject = debugTransform.GetComponent<GridDebugObject>(); // � ���������� ������ ������� ��������� GridDebugObject
                gridDebugObject.SetGridObject(GetGridObject(gridPosition)); // �������� ����� SetGridObject() � �������� ���� ������� ����� ����������� � ������� _gridPosition // GetGridObject(_gridPosition) as GridObject - �������� ��������� <TGridObject> ��� GridObject

                // debugTransform.GetComponentInChildren<TextMeshPro>().text = _gridPosition.ToString(); // ��� �������� ������� ��� ����������� ��������� ������ �����( �� ����� ������ �� ���������� debugPrefab) � ������ ����� GridDebugObject
            }
        }
    }

    public TGridObject GetGridObject(GridPosition gridPosition) // ������ ������� ������� ��������� � ������ ������� ����� .������� ��������� �.�. ����� ����������� �������� �� ���.
    {
        return _gridObjectArray[gridPosition.x, gridPosition.z]; // x,z ��� ������� ������� �� ������� ����� ������� ������ �������
    }

    public bool IsValidGridPosition(GridPosition gridPosition) // �������� �� ���������� �������� ��������
    {
        return  gridPosition.x >=0 && 
                gridPosition.z >=0 && 
                gridPosition.x<_width && 
                gridPosition.z<_height; 
        // ��������� ��� ���������� ��� �������� ������ 0 � ������ ������ � ������ ����� �����
    }

    public int GetWidth()
    {
        return _width;
    }

    public int GetHeight()
    {
        return _height;
    }

    public float GetCellSize()
    {
        return _cellSize;
    }

}
