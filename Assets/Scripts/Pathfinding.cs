using System.Collections.Generic;
using UnityEngine;

// �������� ������� ���������� ������� Pathfinding, ������� � Project Settings/ Script Execution Order � �������� ���������� Pathfinding ���� Default Time, ����� Pathfinding ���������� ������ �� ���� ��� ��������� �������� ����� ���� 

public class Pathfinding : MonoBehaviour // ����� ���� // ������ ������� ����� ������������ ������ ��� ������ ���� (����� �� ����� LevelGrid)
{

    public static Pathfinding Instance { get; private set; }   //(������� SINGLETON) ��� �������� ������� ����� ���� ������� (SET-���������) ������ ���� �������, �� ����� ���� �������� GET ����� ������ �������
                                                               // instance - ���������, � ��� ����� ���� ��������� Pathfinding ����� ���� ��� static. Instance ����� ��� ���� ����� ������ ������, ����� ����, ����� ����������� �� Event.



    private const int MOVE_STRAIGHT_COST = 10; // ��������� �������� ����� (��� �������� ����� 10 � �� 1 ��� �� �� ������������ float)
    private const int MOVE_DIAGONAL_COST = 14; // ��������� �������� �� ��������� ( ������������� �� ������� �������� ������ ���������� �� ����� ��������� ������� �������������� ������������. � ����� ��� �������� ����� 14 � �� 1,4 ��� �� �� ������������ float)

    //���������� ��� ������ ���������� ������ GetNeighbourList() ����� ��������
    /*// ���������� �������� ������� ��� ������ �������� �����
    private GridPosition UP = new(0, 1);
    private GridPosition DOWN = new(0, -1);
    private GridPosition RIGHT = new(1, 0);
    private GridPosition LEFT = new(-1, 0);*/

    [SerializeField] private Transform _pathfindingGridDebugObject; // ������ ������� ����� //������������ ��� ������ ��������� � ����� ��������� ������ CreateDebugObject
    [SerializeField] private LayerMask _obstaclesLayerMask; // ����� ���� ����������� (�������� � ����������) ���� ������� Obstacles // ����� �� ���� ������ � ���� ���������� ����� ����� -Obstacles ����� ������

    private int _width;     // ������
    private int _height;    // ������
    private float _cellSize;// ������ ������
    private GridSystem<PathNode> _gridSystem; // ������� �������� ������� � ����� PathNode

    private void Awake()
    {
        // ���� �� �������� � ���������� �� �������� �� �����
        if (Instance != null) // ������� �������� ��� ���� ������ ���������� � ��������� ����������
        {
            Debug.LogError("There's more than one Pathfinding!(��� ������, ��� ���� Pathfinding!) " + transform + " - " + Instance);
            Destroy(gameObject); // ��������� ���� ��������
            return; // �.�. � ��� ��� ���� ��������� Pathfinding ��������� ����������, ��� �� �� ��������� ������ ����
        }
        Instance = this;
    }

    public void Setup(int width, int height, float cellSize) // �������� ���� ������ �����
    {
        _width = width;
        _height = height;
        _cellSize = cellSize;

        _gridSystem = new GridSystem<PathNode>(_width, _height, _cellSize,                          // �������� ����� 10 �� 10 � �������� 2 ������� � � ������ ������ �������� ������ ���� PathNode
                (GridSystem<PathNode> g, GridPosition gridPosition) => new PathNode(gridPosition)); //� ��������� ��������� ��������� ������� ������� �������� ����� ������ => new PathNode(_gridPosition) � ��������� �� ��������. (������ ��������� ����� ������� � ��������� �����)
        
       // _gridSystem.CreateDebugObject(_pathfindingGridDebugObject); // �������� ��� ������ � ������ ������// ���������� ������ ����� ������� �.�. ��������� ���������

        // � ����� �������� ��� ������ �� ����������� ������������, ����� �������� ����� �� ������ ������� � ���� ��� ���������� � ������������ _obstaclesAndDoorLayerMask, ��������� ��� ������ �� ����������
        for (int x = 0; x < _width; x++)
        {
            for (int z = 0; z < _height; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z); // ������� �����
                Vector3 worldPosition = LevelGrid.Instance.GetWorldPosition(gridPosition); // ������� ������� ����������
                float raycastOffsetDistance = 5f; // ��������� �������� ����

                //��������� ���. ��� �� ����� �������� ������ ���������(������ �����), ������� ������� ��� ����, � ����� �������� �����, ����� ������ �������� � ��� ���� ������ ��� �������� ����, ��� ����� ����������������� � ��������� ������ ����
                // ����� ������� ��������� � UNITY � �� ������� ������� ����. Project Settings/Physics/Queries Hit Backfaces - ��������� �������, � ����� ����� �������� �� ����� ���������
                if (Physics.Raycast(
                     worldPosition + Vector3.down * raycastOffsetDistance,
                     Vector3.up,
                     raycastOffsetDistance * 2,
                     _obstaclesLayerMask)) // ���� ��� ����� � ����������� �� ��������� ������ �� ���������� (Raycast -������ bool ����������)
                {
                    GetNode(x, z).SetIsWalkable(false);
                }                
            }
        }
    }

    public List<GridPosition> FindPath(GridPosition startGridPosition, GridPosition endGridPosition, out int pathLength) // ����� ���� // ���� �� ����������� �������� ����� "out", �� ������� ������ ���������� �������� ��� ���� ���������� pathLength-����� ����, �� ���� ��� ������ ������� ��������.
    {
        List<PathNode> openList = new List<PathNode>();     // �������� ������ "�������� ������" - ��� ���� ������� ��������� ����� 
        List<PathNode> closedList = new List<PathNode>();   // �������� ������ "�������� ������" - ��� ����, � ������� ��� ��� ���������� �����

        PathNode startNode = _gridSystem.GetGridObject(startGridPosition); // ������� ��������� ����, ������ GridObject ���� PathNode � ���������� ��� startGridPosition
        PathNode endNode = _gridSystem.GetGridObject(endGridPosition); // ������� �������� ����, ������ GridObject ���� PathNode � ���������� ��� endGridPosition

        openList.Add(startNode); //������� � ������ ��������� ����

        for (int x = 0; x < _gridSystem.GetWidth(); x++) // � ����� ������� ��� ���� �������� ������� � ������� ���������
        {
            for (int z = 0; z < _gridSystem.GetHeight(); z++)
            {
                GridPosition gridPosition = new GridPosition(x, z); // ������� �����

                PathNode pathNode = _gridSystem.GetGridObject(gridPosition); // ������� ������ ����� ���� PathNode

                pathNode.SetGCost(int.MaxValue); // ��������� �������� G ������������ ������
                pathNode.SetHCost(0);            // ��������� �������� H =0
                pathNode.CalculateFCost();       // ��������� �������� F
                pathNode.ResetCameFromPathNode(); // ������� ������ �� ���������� ���� ���� 
            }
        }

        startNode.SetGCost(0); // G -��� ��������� �������� �� ����������� ���� � ���������� ������� ���������. �� ��� �� ������ �� ������������ ������� G=0
        startNode.SetHCost(CalculateDistance(startGridPosition, endGridPosition)); // ��������� ����������� H ���������
        startNode.CalculateFCost();

        while (openList.Count > 0) // ���� � �������� ������ ���� �������� �� ��� �������� ��� ���� ���� ��� ������. ���� ����� �������� ���� �� ��������� ��� ������
        {
            PathNode currentNode = GetLowestFCostPathNode(openList); // ������� ���� ���� � ���������� ���������� F �� openList � ������� ��� ������� �����

            if (currentNode == endNode) // ��������� ����� �� ��� ������� ���� ��������� ����
            {
                // �������� ��������� ����
                pathLength = endNode.GetFCost(); // ������ ��������� ����� ����
                return CalculatePath(endNode); // ������ ���������� ����
            }

            openList.Remove(currentNode); // ������ ������� ���� �� ��������� ������
            closedList.Add(currentNode);  // � ������� � �������� ������ // ��� �������� ��� �� ������ �� ����� ����

            foreach (PathNode neighbourNode in GetNeighbourList(currentNode)) // ��������� ���� �������� ����
            {
                if (closedList.Contains(neighbourNode)) // ��������� ��� �� �������� ���� ����� � "�������� ������"
                {
                    //�� ��� ������ �� ����� ����
                    continue;
                }

                //  ��������� ���� �� ��������� ��� ������
                if (!neighbourNode.GetIsWalkable()) // ��������, �������� ���� - �������� ��� ������ // ���� �� �������� �� ������� � "�������� ������" � �������� � ���������� ����
                {
                    closedList.Add(neighbourNode);
                    continue;
                }

                int tentativeGCost = currentNode.GetGCost() + CalculateDistance(currentNode.GetGridPosition(), neighbourNode.GetGridPosition()); // ��������������� ��������� G = ������� G + ��������� ����������� �� �������� � ��������� ����

                if (tentativeGCost < neighbourNode.GetGCost()) // ���� ��������������� ��������� G ������ ��������� G ��������� ����(�� ��������� ��� ����� �������� MaxValue ) (�� ����� ����� ���� ��� �� ������� � ���� �������� ����)
                {
                    neighbourNode.SetCameFromPathNode(currentNode); // ���������� - �� �������� ���� ������ - � �������� ���� ����
                    neighbourNode.SetGCost(tentativeGCost); // ��������� �� �������� ���� ����������� �������� G
                    neighbourNode.SetHCost(CalculateDistance(neighbourNode.GetGridPosition(), endGridPosition)); // ��������� �������� H �� �������� �� ���������
                    neighbourNode.CalculateFCost();

                    if (!openList.Contains(neighbourNode)) // ���� �������� ������ �� �������� ����� ��������� ���� �� ������� ���
                    {
                        openList.Add(neighbourNode);
                    }
                }
            }
        }

        // ���� �� ������
        pathLength = 0;
        return null;
    }

    public int CalculateDistance(GridPosition gridPositionA, GridPosition gridPositionB) // �������� ��������������� ��������� ����� ������ 2�� ��������� ��������� ��� ����� �������� "H"
    {
        GridPosition gridPositionDistance = gridPositionA - gridPositionB; //����� ���� ������������� (��������� ���������� ��������� ����� ������ � ������ ������ - �� ������� ���� �������� ������������ ������ �����)

        // ��� �������� ������ �� ������
        //int totalDistance = Mathf.Abs(gridPositionDistance.x) + Mathf.Abs(gridPositionDistance.z); // ������� ����� ��������� �� ������ (�� ��������� �������� �� ��������� ������ �� ������. �������� �� ����� (0,0) � ����� (3,2) �������� ��� ���� � ����� � ��� ���� ����� ����� 5)

        int xDistsnce = Mathf.Abs(gridPositionDistance.x); //����������� � (0,0) �� (1,1) ����� �������� 1 ����������, ������ ���� ����������� �� ������ // ���� � (0,0) �� (2,1) �� ��������� ����� ��� ����� ����. ������� ��� ������� ���������� ���������� ���� ����� ����������� ����� �� ���������� (x,z)
        int zDistsnce = Mathf.Abs(gridPositionDistance.z);
        int remaining = Mathf.Abs(xDistsnce - zDistsnce); // ���������� ��������� ����� ������������ �� ������, ����� �� ������
        int CalculateDistance = MOVE_DIAGONAL_COST * Mathf.Min(xDistsnce, zDistsnce) + MOVE_STRAIGHT_COST * remaining; // ������ ����������� ���������� � ������� GridPosition ��� ����� �������� �� ��������� � �������� �� ������
        return CalculateDistance;
    }

    private PathNode GetLowestFCostPathNode(List<PathNode> pathNodeList) // �������� ���� ���� � ���������� ���������� F  � �������� ��������� ������ ����� ����// ��� ����������� ������ ����� �� ������ �� ���� ������
                                                                         // ���� ��������� ����� ����� ���������� ���������� F �� �������� ������ ���������� � ������
                                                                         // � ����� ������ �� ���������� ������ ������ ������� (������� �� ������������ ������ � ������ GetNeighbourList())
    {
        PathNode lowestFCostPathNode = pathNodeList[0]; // ������� ������ ������� � ������
        for (int i = 0; i < pathNodeList.Count; i++) // ��������� � ����� ������ ��������
        {
            if (pathNodeList[i].GetFCost() < lowestFCostPathNode.GetFCost()) // ���� �������� F ������ �������� ������ �������� �� ������� ��� lowestFCostPathNode
            {
                lowestFCostPathNode = pathNodeList[i];
            }
        }
        return lowestFCostPathNode;
    }

    private PathNode GetNode(int x, int z) // �������� ���� � ������������ �� GridPosition(x,z)
    {
        return _gridSystem.GetGridObject(new GridPosition(x, z));
    }

    private List<PathNode> GetNeighbourList(PathNode currentNode) // �������� ������ ������� ��� currentNode
    {
        List<PathNode> neighbourList = new List<PathNode>(); // �������������� ����� ������ �������

        GridPosition gridPosition = currentNode.GetGridPosition();

        // ������� �������� ����� �� ���� �� ������� ����� ����� ������� ������ ������ �� ������� ������

        if (gridPosition.x - 1 >= 0)
        {
            //Left
            neighbourList.Add(GetNode(gridPosition.x - 1, gridPosition.z + 0)); // ������� ���� �����
            if (gridPosition.z - 1 >= 0)
            {
                //Left Down
                neighbourList.Add(GetNode(gridPosition.x - 1, gridPosition.z - 1)); // ������� ���� ����� � ����
            }

            if (gridPosition.z + 1 < _gridSystem.GetHeight())
            {
                //Left Up
                neighbourList.Add(GetNode(gridPosition.x - 1, gridPosition.z + 1)); // ������� ���� ����� � �����
            }
        }

        if (gridPosition.x + 1 < _gridSystem.GetWidth())
        {
            //Right
            neighbourList.Add(GetNode(gridPosition.x + 1, gridPosition.z + 0)); // ������� ���� ������

            if (gridPosition.z - 1 >= 0)
            {
                //Right Down
                neighbourList.Add(GetNode(gridPosition.x + 1, gridPosition.z - 1)); // ������� ���� ������ � ����
            }

            if (gridPosition.z + 1 < _gridSystem.GetHeight())
            {
                //Right Up
                neighbourList.Add(GetNode(gridPosition.x + 1, gridPosition.z + 1)); // ������� ���� ������ � �����
            }
        }

        if (gridPosition.z - 1 >= 0)
        {
            //Down
            neighbourList.Add(GetNode(gridPosition.x + 0, gridPosition.z - 1)); // ������� ���� �����
        }
        if (gridPosition.z + 1 < _gridSystem.GetHeight())
        {
            //Up
            neighbourList.Add(GetNode(gridPosition.x + 0, gridPosition.z + 1)); // ������� ���� ������
        }
        return neighbourList;
    }


    //2 ������ ���������� ������ GetNeighbourList() ����� ��������
    /* private List<PathNode> GetNeighbourList(PathNode currentNode) // �������� ������ ������� ��� currentNode
     {
         List<PathNode> neighbourList = new List<PathNode>(); // �������������� ����� ������ �������

         GridPosition _gridPosition = currentNode.GetGridPosition(); // ������� �������� ������� ������ ������

         GridPosition[] neigboursPositions = //�������� ������ �������� ������� �������� �����
         {
         _gridPosition + UP,
         _gridPosition + UP + RIGHT,
         _gridPosition + RIGHT,
         _gridPosition + RIGHT + DOWN,
         _gridPosition + DOWN,
         _gridPosition + DOWN + LEFT,
         _gridPosition + LEFT,
         _gridPosition + LEFT + UP
         };

         foreach (GridPosition p in neigboursPositions) // � ����� �������� �� ������������ ���� �������� �������
         {
             if (_gridSystem.IsValidGridPosition(p))
             {
                 neighbourList.Add(GetNode(p.x, p.z));
             }                
         }

         return neighbourList;
     }*/

    private List<GridPosition> CalculatePath(PathNode endNode) //���������� ���� (����� ����������� ������ � �������� �����������)
    {
        List<PathNode> pathNodeList = new List<PathNode>(); // �������������� ������ ����� ����
        pathNodeList.Add(endNode);
        PathNode currentNod = endNode;

        while (currentNod.GetCameFromPathNode() != null) //� ����� ������� ������������ ���� � ������. ������������ ���� - ��� �� � �������� �� ���� ������ (� ���� ����� 1 ������ �� �������� � �������� ������). �������� ���� ����� ��������� � � ���� GetCameFromPathNode() = null - ���� ���������
        {
            pathNodeList.Add(currentNod.GetCameFromPathNode());//������� � ������ ��� ������������ ����
            currentNod = currentNod.GetCameFromPathNode(); // ������������ ���� ����������� �������
        }

        pathNodeList.Reverse(); // �.�. �� ������ � ����� ���� ����������� ��� ������ ��� �� �������� �������� �� ������ � ������
        // ��������� ��� ������ "PathNode-�����" � ������ "GridPosition - ������� �����"
        List<GridPosition> gridPositionList = new List<GridPosition>();
        foreach (PathNode pathNode in pathNodeList)
        {
            gridPositionList.Add(pathNode.GetGridPosition());
        }

        return gridPositionList;
    }

    public void SetIsWalkableGridPosition(GridPosition gridPosition, bool isWalkable) // ���������� ��� ����� ��� ������ (� ����������� �� isWalkable)  ������ �� ���������� � �������� �������� �������
    {
       _gridSystem.GetGridObject(gridPosition).SetIsWalkable(isWalkable);
    }

    public bool IsWalkableGridPosition(GridPosition gridPosition) // ����� ������ �� ���������� � �������� �������� �������
    {
        return _gridSystem.GetGridObject(gridPosition).GetIsWalkable(); 
    }

    public bool HasPath(GridPosition startGridPosition, GridPosition endGridPosition) // ����� ���� ?
    {
        return FindPath(startGridPosition, endGridPosition, out int pathLength) != null; // ���� ���� ���� �� startGridPosition � endGridPosition �� �������� true ���� ���� ��� �������� false (out int pathLength �������� ��� �� ��������������� ���������)
    }

    public int GetPathLength(GridPosition startGridPosition, GridPosition endGridPosition) // �������� ����� ���� (�������� F -endGridPosition)  
    {
        FindPath(startGridPosition, endGridPosition, out int pathLength);
        return pathLength;
    }

}
