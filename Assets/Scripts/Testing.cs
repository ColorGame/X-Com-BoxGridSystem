using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    [SerializeField] private Transform _gridDebugObjectPrefab; // Префаб отладки сетки //Передоваемый тип должен совподать с типом аргумента метода CreateDebugObject
    [SerializeField] private Unit _unit;
    private GridSystem<GridObject> _gridSystem;

    private void Start()
    {


        //ТЕСТ CreateDebugObject визуализация координат сетки в каждой ячейки
        /*_gridSystem = new GridSystem(10, 10, 2f); // построим сетку 10 на 10 и размером 2 еденицы
        _gridSystem.CreateDebugObject(_gridDebugObjectPrefab); // Создадим наш префаб в каждой ячейки

        Debug.Log(new GridPosition(5, 7)); // тестируем что возвращает GridPosition*/
    }

    private void Update()
    {
        //ТЕСТ 
        //Debug.Log(_gridSystem.GetGridPosition(MouseWorld.GetPosition())); // Получим положение сетки прямо под мышкой

        //ТЕСТ Допустимых Сеточных Позиция для Действий




        
        if(Input.GetKeyDown(KeyCode.T))
        {
            //Для отрисовки линии следования из (0,0) в место указателя мыши
            /*GridPosition _mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());

            GridPosition startGridPosition = new GridPosition(0, 0);

            List<GridPosition> gridPositionList = Pathfinding.Instance.FindPath(startGridPosition, _mouseGridPosition);

            for (int i = 0; i < gridPositionList.Count -1 ; i++)
            {
                Debug.DrawLine(
                    LevelGrid.Instance.GetWorldPosition(gridPositionList[i]),
                    LevelGrid.Instance.GetWorldPosition(gridPositionList[i + 1]),
                    Color.white,
                    10f
                    );
            }*/

            
        }      
    }
}

