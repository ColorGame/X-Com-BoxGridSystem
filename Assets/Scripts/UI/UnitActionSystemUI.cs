using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI; // Для работы с Пользовательским интерфейсом

public class UnitActionSystemUI : MonoBehaviour // Система действий UI юнита // Динамически создавать кнопки при выборе юнита // Лежит в Canvas
{

    [SerializeField] private Transform _actionButtonPrefab; // В инспекторе закинем префаб Кнопки
    [SerializeField] private Transform _actionButtonContainerTransform; // В инспекторе назначить  Контейнер для кнопок( находиться в сцене в Canvas)
    [SerializeField] private TextMeshProUGUI _actionPointsText; // Ссылка на текст очков

    private List<ActionButtonUI> _actionButtonUIList; // Список кнопок действий

    private void Awake()
    {
        _actionButtonUIList = new List<ActionButtonUI> (); // Создадим экземпляр списка
    }

    private void Start()
    {
        UnitActionSystem.Instance.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged; // подписываемся на Event из UnitActionSystem (становимся слушателями). Обозначает что мы выполняем функцию UnitActionSystem_OnSelectedUnitChanged()
                                                                                                   // Будет выполняться каждый раз когда мы меняем выбранного юнита. //OnSelectedUnitChanged - Выбранный Юнит Изменен
        UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged; // подписываемся на Event// Будет выполняться каждый раз когда мы меняем Базовое Действие // Выбранное Действие Изменено

        UnitActionSystem.Instance.OnActionStarted += UnitActionSystem_OnActionStarted; // подписываемся на Event// Будет выполняться каждый раз при старте действия. // Действие Начато

        //2//3//{ Еще несколько способов скрыть кнопки когда занят действием
        UnitActionSystem.Instance.OnBusyChanged += UnitActionSystem_OnBusyChanged; // Подписываюсь на Event и выполним UnitActionSystem_OnBusyChanged, эта фунуция получит от события булевый аргумент // Занятость Изменена
        //2//3//}
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged; // подписываемся на Event // Будет выполняться (изминение текста очков действий) каждый раз когда изменен номер хода.
        // РЕШЕНИЕ 2 //{
        Unit.OnAnyActionPointsChanged += Unit_OnAnyActionPointsChanged; //подписываемся на статический Event // Буудет выполняться каждый раз при изменении очков действий у ЛЮБОГО(Any) юнита а не только у выбранного.
        // РЕШЕНИЕ 2 //}

        CreateUnitActionButtons();
        UpdateSelectedVisual();
        UpdateActionPoints();
    }

    /*//2//{ Второй способ скрыть кнопки когда занят действием
    private void Show() // Показать
    {
        gameObject.SetActive(true);
    }

    private void Hide() // Скрыть
    {
        gameObject.SetActive(false);
    }

    private void UnitActionSystem_OnBusyChanged(object sender, bool isBusy)
    {
        if (isBusy) // Если занят то скрыть если нет то показать кнопки
        {
            Hide();
        }
        else
        {
            Show();
        }
    } //2//}*/

    //3//{ Третий способ скрыть кнопки когда занят действием
    private void UnitActionSystem_OnBusyChanged(object sender, bool isBusy)
    {
        foreach (ActionButtonUI actionButtonUI in _actionButtonUIList) // В цикле обработаем состояние кнопок
        {
            actionButtonUI.HandleStateButton(isBusy);
        }


    } //3//}


    private void CreateUnitActionButtons() // Создать Кнопки для Действий Юнита 
    {
        foreach (Transform buttonTransform in _actionButtonContainerTransform) // Очистим контейнер с кнопками
        {
            Destroy (buttonTransform.gameObject); // Удалим игровой объект прикрипленный к Transform
        }

        _actionButtonUIList.Clear (); // Очистим сисок кнопок

        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit(); // Получим выбранного юнита

        foreach (BaseAction baseAction in selectedUnit.GetBaseActionsArray()) // В цикле переберем массив базовых действий у выбранного юнита
        {
            Transform actionButtonTransform = Instantiate(_actionButtonPrefab, _actionButtonContainerTransform); // Для каждого baseAction создадим префаб кнопки и назначим родителя - Контейнер для кнопок
            ActionButtonUI actionButtonUI = actionButtonTransform.GetComponent<ActionButtonUI>(); // У кнопки найдем компонент ActionButtonUI
            actionButtonUI.SetBaseAction(baseAction); //Назвать и Присвоить базовое действие (нашей кнопке)
            
            _actionButtonUIList.Add (actionButtonUI); // Добавим в список полученный компонент ActionButtonUI
        }
    }

    private void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs empty) //sender - отправитель // Подписка должна иметь туже сигнатуру что и функция отправителя OnSelectedUnitChanged
    {
        CreateUnitActionButtons(); // Создать Кнопки для Действий Юнита 
        UpdateSelectedVisual(); 
        UpdateActionPoints();
    }

    private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs empty)
    {
        UpdateSelectedVisual();
    }

    private void UnitActionSystem_OnActionStarted(object sender, EventArgs empty) // Действие началось - это означает что очки уже потрачены и надо их обновить
    {
        UpdateActionPoints();
    }
    private void UpdateSelectedVisual() //Обнавление визуализации выбора( при выборе кнопки включим рамку)
    {
        foreach (ActionButtonUI actionButtonUI in _actionButtonUIList)
        {
            actionButtonUI.UpdateSelectedVisual();
        }
    }

    private void UpdateActionPoints() // Обнавление очков действий (над кнопками действий)
    {
        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();// Возьмем выбранного юнита
        
        _actionPointsText.text = "Action Points; " + selectedUnit.GetActionPoints(); //Изменим текст добавив в него количество очков
    }

    // ВНИМАНИЕ // Может возникнуть ошибка. Сброс очков в классе Unit и обновление текста очков в этом классе, СЛУШАЮТ одно и тоже событие. Что выполниться позже или раньше неизвестно, текст может обновиться раньше и показывать еще не сброшенные очки действий "0" а по факту их "2".
    // РЕШЕНИЕ 1 //- НАСТРОИМ ПОРЯДОК ВЫПОЛНЕНИЯ СКРИПТА UnitActionSystemUI , добавим в Project Settings/ Script Execution Order и поместим НИЖЕ Deafault Time в конец
    private void TurnSystem_OnTurnChanged(object sender, EventArgs empty) // Номер хода изменен - это означает что очки действий восстановились, обновим их.
    {
        UpdateActionPoints();

        // ЗДЕСЬ МОЖНО РЕАЛИЗОВАТЬ ОТКЛЮЧЕНИЕ КНОПОК ВО ВРЕМЯ ХОДА ВРАГА
    }

    // РЕШЕНИЕ 2 //{
    private void Unit_OnAnyActionPointsChanged(object sender, EventArgs empty) //Произошло изменение очков действий у ЛЮБОГО(Any) юнита а не только у выбранного. обновим их.
    {
        UpdateActionPoints();
    }// РЕШЕНИЕ 2 //}
}
