using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public class InteractAction : BaseAction // Действие взаимодействия
{

    private int _maxInteractDistance = 1; // Дистанция взаимодействия


    private void Update()
    {
        if (!_isActive) // Если не активны то ...
        {
            return; // выходим и игнорируем код ниже
        }
        
        //Что бы мы не смогли открывать и тутже закрывать дверь не дождавшись окончания действия - полного открытия или закрытия
        //Будем вызывать данную функцию через делегат на самой ДВЕРИ когда на ней закончиться таймер
        //ActionComplete(); //Действие завершено

    }

    public override string GetActionName() // Получим имя для кнопки
    {
        return "Interact";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition) //Получить действие вражеского ИИ  для переданной нам сеточной позиции// Переопределим абстрактный базовый метод //EnemyAIAction создан в каждой Допустимой Сеточнй Позиции, наша задача - настроить каждую ячейку в зависимости от состоянии юнита который там стоит
    {
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 0
        };
    }

    public override List<GridPosition> GetValidActionGridPositionList() // Получить Список Допустимых Сеточных Позиция для Действий // переопределим базовую функцию  
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>(); 

        GridPosition unitGridPosition = _unit.GetGridPosition(); // Получим позицию в сетке юнита

        for (int x = -_maxInteractDistance; x <= _maxInteractDistance; x++) // Юнит это центр нашей позиции с координатами unitGridPosition, поэтому переберем допустимые значения в условном радиусе _maxInteractDistance
        {
            for (int z = -_maxInteractDistance; z <= _maxInteractDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);  // Смещенная сеточная позиция. Где началом координат(0,0) является сам юнит 
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;  // Тестируемая Сеточная позиция

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) // Проверим Является ли testGridPosition Допустимой Сеточной Позицией если нет то переходим к след циклу
                {
                    continue;
                }

                /*//Проверим тестируемую сеточную позицию на наличие двери
                DoorInteract door = LevelGrid.Instance.GetDoorAtGridPosition(testGridPosition);

                if (door == null)
                {
                    // В этой позиции сетки нет двери
                    continue;
                }*/
                // Применим интерфейс ВЗАИМОДЕЙСТВИЯ что бы мы могли взимодействовать не только с дверью
                IInteractable interactable  = LevelGrid.Instance.GetInteractableAtGridPosition(testGridPosition);

                if (interactable == null)
                {
                    // В этой позиции сетки нет объекта взаимодействия
                    continue;
                }
                

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete) // Переопределим TakeAction (Применить Действие (Действовать). (Делегат onActionComplete - по завершении действия). в нашем случае делегату передаем функцию ClearBusy - очистить занятость
    {
        IInteractable interactable = LevelGrid.Instance.GetInteractableAtGridPosition(gridPosition); // Получим IInteractable(интерфейс взаимодействия) из переданной сеточной позиции // НАМ БЕЗ РАЗНИЦЫ КАКОЙ ОБЪЕКТ МЫ ПОЛУЧИМ (дверь, сфера, кнопка...) - лиш бы он реализовал этот интерфейс

        interactable.Interact(OnInteractComplete); //Произведем Взаимодействие с полученной IInteractable(интерфейс взаимодействия) и Передадим делгат - При завершении взаимодействия (этот делегат будет вызывать сама дверь)

        ActionStart(onActionComplete); // Вызовим базовую функцию СТАРТ ДЕЙСТВИЯ // Вызываем этот метод в конце после всех настроек т.к. в этом методе есть EVENT и он должен запускаться после всех настроек
    }

    private void OnInteractComplete() //При завершении взаимодействия
    {
        ActionComplete(); //Действие завершено
        //Что бы мы не смогли открывать и тутже закрывать дверь не дождавшись окончания действия - полного открытия или закрытия
        //Будем вызывать данную функцию через делегат на самой ДВЕРИ когда на ней закончиться таймер
        
    }

}

