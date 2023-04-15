using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnSystemUI : MonoBehaviour // Система Ходов UI обрабатывает нажатие и отображение текста
{
    [SerializeField] private TextMeshProUGUI _turnNumberText; // В инспекторе закинем текст Номер Хода
    [SerializeField] private Button _endTurnButton; // В инспекторе закинем кнопку
    [SerializeField] private GameObject _enemyTurnVisualGameObject; // В инспекторе закинуть лэйбел "Ход ВРАГА"

    private void Start()
    {
        //Добавим событие при нажатии на нашу кнопку// AddListener() в аргумент должен получить делегат- ссылку на функцию. Функцию будем объявлять АНАНИМНО через лямбду () => {...} 
        _endTurnButton.onClick.AddListener(() =>
        {
            TurnSystem.Instance.NextTurn();

        });

        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged; // подписываемся на Event // Будет выполняться (обновлять текст хода) каждый раз когда изменен номер хода.

        UpdateNamberTurnText();
        UpdateEnemyTurnVisual();
        UpdateEndTurnButtonVisibility();
    }
    


    private void TurnSystem_OnTurnChanged(object sender, EventArgs empty) // когда изменен хода.
    {
        UpdateNamberTurnText();
        UpdateEnemyTurnVisual();
        UpdateEndTurnButtonVisibility();
    }

    private void UpdateNamberTurnText() // Обнавление очков действий
    {
        _turnNumberText.text = ("Turn " + TurnSystem.Instance.GetTurnNumber()).ToUpper();
    }

    private void UpdateEnemyTurnVisual() //Обновление Визуальной таблички ход врага
    {
        _enemyTurnVisualGameObject.SetActive(!TurnSystem.Instance.IsPlayerTurn()); // Деактивируем на ход игрока и включим на ход врага
    }

    private void UpdateEndTurnButtonVisibility() // Обновить видимость кнопки End Turn
    {
        _endTurnButton.gameObject.SetActive(TurnSystem.Instance.IsPlayerTurn()); // Показываем только во время хода игрока
    }
}
