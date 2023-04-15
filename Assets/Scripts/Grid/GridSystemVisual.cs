using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ������� GridSystemVisual ��� ������� ����� ������� �� ���������, ��������� �� �����, ����� ���������� ������� ����������� ����� ����� ����������.
// (Project Settings/ Script Execution Order � �������� ���������� GridSystemVisual ���� Default Time)
public class GridSystemVisual : MonoBehaviour //�������� ������� ������������  ������������ ��������� ����� �� ����� 
{
    public static GridSystemVisual Instance { get; private set; }   //(������� SINGLETON) ��� �������� ������� ����� ���� ������� (SET-���������) ������ ���� �������, �� ����� ���� �������� GET ����� ������ �������
                                                                    // instance - ���������, � ��� ����� ���� ��������� UnitActionSystem ����� ���� ��� static. Instance ����� ��� ���� ����� ������ ������, ����� ����, ����� ����������� �� Event.

    [Serializable] // ����� ��������� ��������� ����� ������������ � ����������
    public struct GridVisualTypeMaterial    //������ ����� ��� ��������� // �������� ��������� ����� � ��������� ������. ������ � �������� ��������� ������������ ��� ���� ������ �������� ����������� ����� ������ � C#
    {                                       //� ������ ��������� ��������� ��������� ����� � ����������
        public GridVisualType gridVisualType;
        public Material materialGrid;
    }

    public enum GridVisualType //���������� ��������� �����
    {
        White,
        Blue,
        Red,
        RedSoft,
        Yellow
    }


    [SerializeField] private Transform _gridSystemVisualSinglePrefab; // ������ ������������ 
    [SerializeField] private List<GridVisualTypeMaterial> _gridVisualTypeMaterialList; // ������ ��� ��������� ����������� ��������� ����� (������ �� ���������� ���� ������) ����������� ��������� ����� // � ���������� ��� ������ ��������� ���������� ��������������� �������� �����



    private GridSystemVisualSingle[,] _gridSystemVisualSingleArray; // ��������� ������


    private void Awake() //��� ��������� ������ Awake ����� ������������ ������ ��� ������������� � ���������� ��������
    {
        // ���� �� �������� � ���������� �� �������� �� �����
        if (Instance != null) // ������� �������� ��� ���� ������ ���������� � ��������� ����������
        {
            Debug.LogError("There's more than one UnitActionSystem!(��� ������, ��� ���� UnitActionSystem!) " + transform + " - " + Instance);
            Destroy(gameObject); // ��������� ���� ��������
            return; // �.�. � ��� ��� ���� ��������� UnitActionSystem ��������� ����������, ��� �� �� ��������� ������ ����
        }
        Instance = this;
    }

    private void Start()
    {
        _gridSystemVisualSingleArray = new GridSystemVisualSingle[ // ������� ������ ������������� �������� width �� height
            LevelGrid.Instance.GetWidth(),
            LevelGrid.Instance.GetHeight()
        ];

        for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++)
        {
            for (int z = 0; z < LevelGrid.Instance.GetHeight(); z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);

                Transform gridSystemVisualSingleTransform = Instantiate(_gridSystemVisualSinglePrefab, LevelGrid.Instance.GetWorldPosition(gridPosition), Quaternion.identity); // �������� ��� ������ � ������ ������� �����

                _gridSystemVisualSingleArray[x, z] = gridSystemVisualSingleTransform.GetComponent<GridSystemVisualSingle>(); // ��������� ��������� GridSystemVisualSingle � ��������� ������ ��� x,z ��� ����� ������� �������.
            }
        }

        UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged; // ���������� �� ������� ��������� �������� �������� (����� �������� �������� �������� � ����� ������ �� �������� ������� Event)
        LevelGrid.Instance.OnAnyUnitMovedGridPosition += LevelGrid_OnAnyUnitMovedGridPosition; // ���������� �� ������� ����� ���� ��������� � �������� �������

        MouseWorld.OnMouseGridPositionChanged += MouseWorld_OnMouseGridPositionChanged;// ���������� �� ������� �������� ������� ���� ��������
              
        UpdateGridVisual();
    }

   

    private void MouseWorld_OnMouseGridPositionChanged(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }

    public void HideAllGridPosition() // ������ ��� ������� �����
    {
        for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++)
        {
            for (int z = 0; z < LevelGrid.Instance.GetHeight(); z++)
            {
                _gridSystemVisualSingleArray[x, z].Hide();
            }
        }
    }

    private void ShowGridPositionRange(GridPosition gridPosition, int range, GridVisualType gridVisualType, bool showFigureRhombus) // �������� ��������� �������� �������� ������� ��� �������� (� ��������� �������� �������� �������, ������ ��������, ��� ��������� ������� �����, ������� ���������� ���� ���� ���������� � ���� ����� �� �������� true, ���� � ���� �������� �� - false )
    {
        // �� �������� ��� � ShootAction � ������ "public override List<GridPosition> GetValidActionGridPositionList()"

        List<GridPosition> gridPositionList = new List<GridPosition>();

        for (int x = -range; x <= range; x++)  // ���� ��� ����� ����� ������� � ������������ unitGridPosition, ������� ��������� ���������� �������� � �������� ������� range
        {
            for (int z = -range; z <= range; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z); // ��������� �������� �������. ��� ������� ���������(0,0) �������� ��� ���� 
                GridPosition testGridPosition = gridPosition + offsetGridPosition; // ����������� �������� �������

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) // �������� �������� �� testGridPosition ���������� �������� �������� ���� ��� �� ��������� � ���� �����
                {
                    continue; // continue ���������� ��������� ���������� � ��������� �������� ����� 'for' ��������� ��� ����
                }

                if (showFigureRhombus)
                {
                    // ��� ������� �������� ������� ���� � �� �������
                    int testDistance = Mathf.Abs(x) + Mathf.Abs(z); // ����� ���� ������������� ��������� �������� �������
                    if (testDistance > range) //������� ������ �� ����� � ���� ����� // ���� ���� � (0,0) �� ������ � ������������ (5,4) ��� �� ������� �������� 5+4>7
                    {
                        continue;
                    }
                }                

                gridPositionList.Add(testGridPosition);
            }
        }

        ShowGridPositionList(gridPositionList, gridVisualType); // ������� ��������� �������� ��������
    }       

    public void ShowGridPositionList(List<GridPosition> gridPositionlist, GridVisualType gridVisualType)  //������� ������ GridPosition (� ��������� ���������� ������ GridPosition � ��������� ������������ ����� gridVisualType)
    {
        foreach (GridPosition gridPosition in gridPositionlist) // � ����� ��������� ������ � �������(�������) ������ �� ������� ������� ��� ��������
        {
            _gridSystemVisualSingleArray[gridPosition.x, gridPosition.z].
                Show(GetGridVisualTypeMaterial(gridVisualType)); // � �������� Show �������� �������� � ����������� �� ����������� ��� �������
        }
    }

    public void UpdateGridVisual() // ���������� ������� �����
    {
        HideAllGridPosition(); // ������ ��� ������� �����

        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit(); //������� ���������� �����

        BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction(); // ������� ��������� ��������

        GridVisualType gridVisualType;  // �������� ����� ���� GridVisualType

        switch (selectedAction) // ������������� ��������� ������� ����� � ����������� �� ���������� ��������
        {
            default: // ���� ���� ����� ����������� �� ��������� ���� ��� ��������������� selectedAction
            case MoveAction moveAction: // �� ����� ������ -�����
                gridVisualType = GridVisualType.White;
               
                ShowGridPositionList(selectedAction.GetValidActionGridPositionList(), gridVisualType); // �������(�������) ������ �� ������� ������� ��� �������� (� �������� �������� ������ ���������� ������� ����� ���������� ��������, � ��� ��������� ������������ ������� ��� ����� switch)
                break;

            case SpinAction spinAction: // �� ����� �������� -�������
                gridVisualType = GridVisualType.Blue;
               
                ShowGridPositionList(selectedAction.GetValidActionGridPositionList(), gridVisualType); // �������(�������) ������ �� ������� ������� ��� �������� (� �������� �������� ������ ���������� ������� ����� ���������� ��������, � ��� ��������� ������������ ������� ��� ����� switch)
                break;

            case ShootAction shootAction: // �� ����� �������� -�������
                gridVisualType = GridVisualType.Red;

                ShowGridPositionRange(selectedUnit.GetGridPosition(), shootAction.GetMaxShootDistance(), GridVisualType.RedSoft,true); // ������� �������� ��������
                ShowGridPositionList(selectedAction.GetValidActionGridPositionList(), gridVisualType); // �������(�������) ������ �� ������� ������� ��� �������� (� �������� �������� ������ ���������� ������� ����� ���������� ��������, � ��� ��������� ������������ ������� ��� ����� switch)

                break;

            case GrenadeAction grenadeAction:// �� ����� ������� ������� -������
                gridVisualType = GridVisualType.Yellow;

                List<GridPosition> validActionGridPositionList = selectedAction.GetValidActionGridPositionList(); // ���������� �������� ������� ��� �������� �������
                GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition()); // �������� ������� ����

                ShowGridPositionList(validActionGridPositionList, gridVisualType); // �������(�������) ������ �� ������� ������� ��� �������� (� �������� �������� ������ ���������� ������� ����� ���������� ��������, � ��� ��������� ������������ ������� ��� ����� switch)

                if (validActionGridPositionList.Contains(mouseGridPosition)) // ���� �������� ������� ���� ������ � ���������� �������� �� ...
                {
                    ShowGridPositionRange(mouseGridPosition, grenadeAction.GetDamageRadiusInCells(), GridVisualType.Red, false); // ������� ������ �������� ������� ���� �������
                }
                break;

            case SwordAction swordAction: // �� ����� ����� ����� -�������
                gridVisualType = GridVisualType.Red;

                ShowGridPositionRange(selectedUnit.GetGridPosition(), swordAction.GetMaxSwordDistance(), GridVisualType.RedSoft, false); // ������� �������� �����
                ShowGridPositionList(selectedAction.GetValidActionGridPositionList(), gridVisualType); // �������(�������) ������ �� ������� ������� ��� �������� (� �������� �������� ������ ���������� ������� ����� ���������� ��������, � ��� ��������� ������������ ������� ��� ����� switch)
                break;

            case InteractAction interactAction : // �� ����� �������������� -�������
                gridVisualType = GridVisualType.Blue;
                
                ShowGridPositionList(selectedAction.GetValidActionGridPositionList(), gridVisualType); // �������(�������) ������ �� ������� ������� ��� �������� (� �������� �������� ������ ���������� ������� ����� ���������� ��������, � ��� ��������� ������������ ������� ��� ����� switch)
                break;
        }

        //ShowGridPositionList(selectedAction.GetValidActionGridPositionList(), gridVisualType); // �������(�������) ������ �� ������� ������� ��� �������� (� �������� �������� ������ ���������� ������� ����� ���������� ��������, � ��� ��������� ������������ ������� ��� ����� switch)
    }

    private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }

    private void LevelGrid_OnAnyUnitMovedGridPosition(object sender, LevelGrid.OnAnyUnitMovedGridPositionEventArgs e)
    {
        UpdateGridVisual();

        //�������� ���������, ������� �� ������ �������, ��� ��������� ��������� ���������� ������ ���, ����� ���� ���������� �������,
        //������ ����� ��������� ���������� ������ �����, ����� ���� ��������� �������� �����.
        //��� �������� �������� ��� �������� ����������, � ��� ����� ��������� ������� �������.
    }

    private Material GetGridVisualTypeMaterial(GridVisualType gridVisualType) //(������� �������� � ����������� �� ���������) �������� ��� ��������� ��� �������� ������������ � ����������� �� ����������� � �������� ��������� �������� ������������
    {
        foreach (GridVisualTypeMaterial gridVisualTypeMaterial in _gridVisualTypeMaterialList) // � ����� ��������� ������ ��� ��������� ����������� ��������� ����� 
        {
            if (gridVisualTypeMaterial.gridVisualType == gridVisualType) // ����  ��������� �����(gridVisualType) ��������� � ���������� ��� ��������� �� ..
            {
                return gridVisualTypeMaterial.materialGrid; // ������ �������� ��������������� ������� ��������� �����
            }
        }

        Debug.LogError("�� ���� ����� GridVisualTypeMaterial ��� GridVisualType " + gridVisualType); // ���� �� ������ ����������� ������ ������
        return null;
    }
}
