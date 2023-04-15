using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour // Искуственный интелект юнита
{

    private enum State // Состояния Врага
    {
        WaitingForEnemyTurn,// Ожидание вражеского хода(хода игрока)
        TakingTurn,         // Выполнить ход (Принять ход)
        Busy,               // Занятый
    }

    private State _state; // Частное состояние

    private void Awake()
    {
        _state = State.WaitingForEnemyTurn; // Установим состояние "Ожидание вражеского хода" по умолчанию, т.к. игрок будет ходить первым
    }

    private float _timer;

    private void Start()
    {
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged; // Подпишемся на событие Ход Изменен
    }

    private void Update()
    {
        if (TurnSystem.Instance.IsPlayerTurn()) // Проверяем это ход врага если ходит игрок то остановить выполнение (таймер работать не будет)
        {
            return;
        }

        switch (_state) // Автомат действий
        {
            case State.WaitingForEnemyTurn:
                break;
            case State.TakingTurn:
                _timer -= Time.deltaTime;
                if (_timer <= 0) // Таймер хода врага по завершении выполнить следущее ...
                {
                    if (TryTakeEnemyAIAction(SetStateTakingTurn))   //Проверим мы можем ПОПРОБОВАТЬ Выполнить Действие Вражеского Искуственного Интелекта.В аргумент передаем делегат который вернет нас опять в исходное состояние TakingTurn,
                                                                    // и враг опять по истечении _timer предпримет действия пока не выполнить все возможные действия ИИ
                    {
                        _state = State.Busy; // переключим на состояние "Занятый" если есть чем заняться
                    }
                    else
                    {
                        // У ВРАГОВ НЕТ ДЕЙСТВИЙ КОТОРЫЕ МОЖНО ВЫПОЛНИТЬ,  ВРАЖЕСКИЙ ХОД ЗАВЕРШЕН
                        TurnSystem.Instance.NextTurn(); // ПЕРЕЙДЕМ К СЛЕДУЮЩЕМУ ХОДУ
                    }
                }
                break;
            case State.Busy:
                break;
        }
    }

    private void SetStateTakingTurn() // Установить состояние Выполнить ход (Принять ход)
    {
        _timer = 0.5f; // Чтобы не было мгновенно
        _state = State.TakingTurn; // Назначим состояние Выполнить ход (Принять ход)
    }

    private void TurnSystem_OnTurnChanged(object seder, EventArgs emptu) // Во время смены хода установим таймер
    {
        if (!TurnSystem.Instance.IsPlayerTurn()) // Проверим что это НЕ ход игрока
        {
            _state = State.TakingTurn; // Назначим состояние Выполнить ход (Принять ход)
            _timer = 2;
        }
    }

    private bool TryTakeEnemyAIAction(Action onEnemyAIActionComplete)   // ПОПРОБОВАТЬ Выполнить Действие Вражеского Искуственного Интелекта. В аргумент передаем делегат onEnemyAIActionComplete (Действие Вражеского Искуственного Интелекта завершено)
                                                                        // Проидем по списку врагов и выполним возможные действия
    {        
        foreach (Unit enemyUnit in UnitManager.Instance.GetEnemyUnitList()) // В цикле переберем врагов в списке Врагов
        {
            if (TryTakeEnemyAIAction(enemyUnit, onEnemyAIActionComplete))  // Проверим можем ли мы попробовать Выполним действие ИИ
            {
                return true;
            }
        }
        return false; // Если делать ИИ нечего то вернем ложь
    }

    //Похожий метод но с другой СИГНАТУРОЙ
    private bool TryTakeEnemyAIAction(Unit enemyUnit, Action onEnemyAIActionComplete) // Выполнить Действие Вражеского Искуственного Интелекта для enemyUnit. В аргумент передаем делегат onEnemyAIActionComplete (Действие Вражеского Искуственного Интелекта завершено) и вражеского юнита
    {
        EnemyAIAction bestEnemyAIAction = null; // Лучшего действия вражеского ИИ по умолчпнию = null 
        BaseAction bestBaseAction = null; // Лучшее базовое действие по умолчпнию = null 

        foreach (BaseAction baseAction in enemyUnit.GetBaseActionsArray()) // Переберем массив базовых действий и НАЙДЕМ ЛУЧШЕЕ
        {
            if (!enemyUnit.CanSpendActionPointsToTakeAction(baseAction)) // Спросим у enemyUnit мы МОЖЕМ Потратить Очки Действия, Чтобы Выполнить Действие ? Если НЕ можем то...
            {
                // Враг не может позволить себе выполнить это действие
                continue; // continue заставляет программу переходить к следующей итерации цикла 'for' игнорируя код ниже
            }

            if (bestEnemyAIAction == null) // В начале цикла лучшее действие Вражеского ИИ не установлено, поэтому установим 1-е действие в списке лучшим по умолчанию
            {
                bestEnemyAIAction = baseAction.GetBestEnemyAIAction(); // Получим Лучшее действие Вражеского ИИ для данного базового действия
                bestBaseAction = baseAction; //Данное(перебираемое) базовое действие является лучшим
            }
            else // Если уже установлено "Лучшее действие Вражеского ИИ" то сравним с другими и выясним какое лучше
            {
                EnemyAIAction testEnemyAIAction = baseAction.GetBestEnemyAIAction(); // Тестируемое Лучшее действие Вражеского ИИ
                if (testEnemyAIAction != null && testEnemyAIAction.actionValue > bestEnemyAIAction.actionValue) // Если тестируемое действие не нулевое и ее значение действия больше предыдушего действия то установим его лучшим
                {
                    bestEnemyAIAction = testEnemyAIAction;
                    bestBaseAction = baseAction;
                }
            }
        }
        //ПОПРОБУЕМ РАСКРУТИТЬ ТОЧКИ ДЕЙСТВИЯ
        if (bestEnemyAIAction != null && enemyUnit.TrySpendActionPointsToTakeAction(bestBaseAction)) // ПОПРОБУЕМ Потратить Очки Действия, Чтобы Выполнить Действие 
        {
            bestBaseAction.TakeAction(bestEnemyAIAction.gridPosition, onEnemyAIActionComplete); //У выбранного действия вызовим метод "Применить Действие (Действовать)" и передадим в делегат функцию SetStateTakingTurn  который вернет нас опять в исходное состояние TakingTurn
            return true; // Если можем что то сделать то вернем ИСТИНА
        }
        else
        {
            return false; // Остановить выполнение и вернуть ЛОЖЬ 
        }        
    }
}
