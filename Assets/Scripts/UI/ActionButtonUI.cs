using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static GridSystemVisual;
using System;

public class ActionButtonUI : MonoBehaviour // Обрабатываем логику нажатия на кнопку. Лежит на самой кнопке
{
    [SerializeField] private TextMeshProUGUI _textMeshPro; // TextMeshProUGUI для пользовательского интерфейса
    [SerializeField] private Button _button;
    [SerializeField] private GameObject _selectedButtonVisualUI; // Будем включать и выкл. GameObject что бы скрыть или показать рамку кнопки // В инспекторе надо закинуть рамку

    [SerializeField] private Sprite _spriteShoot; // В префабе кнопки закинуть соответствующий спрайт кнопки
    [SerializeField] private Sprite _spriteGrenade;
    [SerializeField] private Sprite _spriteMove;
    [SerializeField] private Sprite _spriteSpin;
    [SerializeField] private Sprite _spriteSword;
    [SerializeField] private Sprite _spriteInteract;


    private BaseAction _baseAction;
    //3//{ Третий способ скрыть кнопки когда занят действием
    private Color _textColor;
    //3//}


    public void SetBaseAction(BaseAction baseAction) // Присвоить базовое действие на кнопку (в аргумент передаем наше baseAction)
    {
        _baseAction = baseAction; // Сохраним переданное нам базовое действие

        _textMeshPro.text = baseAction.GetActionName().ToUpper(); // В название кнопки запишем Полученное имя Активного Действия  //ToUpper()- В верхнем регистре

        //3//{ Третий способ скрыть кнопки когда занят действием
        _textColor = _textMeshPro.color; // Сохраним цвет текста
        //3//}

        // т.к. кнопки создаются динамически то и события настраиваем в скрипте а не в инспекторе
        //Добавим событие при нажатии на нашу кнопку// AddListener() в аргумент должен получить делегат- ссылку на функцию. Функцию будем объявлять АНАНИМНО через лямбду () => {...} 
        _button.onClick.AddListener(() =>
        {
            UnitActionSystem.Instance.SetSelectedAction(baseAction); //Установить Выбранное Действие
        });

        switch (_baseAction) // В зависимости от выбранного базового действия присвоить спрайт кнопке
        {
            case ShootAction shootAction:
                _button.image.sprite = _spriteShoot;
                break;

            case GrenadeAction grenadeAction:
                _button.image.sprite = _spriteGrenade;
                break;

            case MoveAction moveAction:
                _button.image.sprite = _spriteMove;
                break;

            case SpinAction spinAction:
                _button.image.sprite = _spriteSpin;
                break;

            case SwordAction swordAction:
                _button.image.sprite = _spriteSword;
                break;

            case InteractAction interactAction:
                _button.image.sprite = _spriteInteract;
                break;

        }
    }
    // Функцию передоваемую в AddListener() будем определять ананимно поэтому закоментируем код ниже
    /*private void MoveActionButton_Click() //
    {

    }*/

    public void UpdateSelectedVisual() // (Обновление визуала) Включение и выключение визуализации выбора.(вызывается событием при выборе кнопки базового действия)
    {
        BaseAction selectedBaseAction = UnitActionSystem.Instance.GetSelectedAction(); // Получим выбраное действие
        _selectedButtonVisualUI.SetActive(selectedBaseAction == _baseAction);   // Включить рамку если выбранное действие совподает с действием которое мы назначили на нашу кнопку
                                                                                // Если не совподает то получим false и рамка отключиться

        // Можно поменять цвет кнопки при активации кнопки
        /*if (selectedBaseAction == _baseAction) // Если кнопка активна
        {
            _button.image.color = _color; // оставим как есть
        }
        else
        {
            _button.image.color = new Color(_color.r, _color.g, _color.b, 0.5f); // сделаем ее полупрозрачной
        }*/
    }


    //3//{ Третий способ скрыть кнопки когда занят действием
    private void InteractableEnable() // Включить взаимодействие
    {
        _button.interactable = true;
        _textMeshPro.color = _textColor;
        UpdateSelectedVisual(); // Обновим отображение рамки кнопки в зависимости от активированного действия
    }

    private void InteractableDesabled() // Отключить взаимодействие // Кнопка становиться не активная и меняет цвет(Настраивается в инспекторе color  Desabled)
    {
        _button.interactable = false;

        Color textColor = _textColor; // Сохраним в локальную переменную цвет текста
        textColor.a = 0.1f; // Изменим значение альфа канала
        _textMeshPro.color = textColor; // Изменим текущий цвет текса (сдел прозрачным)

        _selectedButtonVisualUI.SetActive(false); //Отключим рамку
    }

    public void HandleStateButton(bool isBusy) // Обработать состояние кнопки
    {
        if (isBusy) // Если занят
        {
            InteractableDesabled(); // Отключить взаимодействие
        }
        else
        {
            InteractableEnable(); // Включить взаимодействие
        }
    }//3//}
}
