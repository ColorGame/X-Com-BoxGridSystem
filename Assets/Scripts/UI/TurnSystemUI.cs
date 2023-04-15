using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnSystemUI : MonoBehaviour // ������� ����� UI ������������ ������� � ����������� ������
{
    [SerializeField] private TextMeshProUGUI _turnNumberText; // � ���������� ������� ����� ����� ����
    [SerializeField] private Button _endTurnButton; // � ���������� ������� ������
    [SerializeField] private GameObject _enemyTurnVisualGameObject; // � ���������� �������� ������ "��� �����"

    private void Start()
    {
        //������� ������� ��� ������� �� ���� ������// AddListener() � �������� ������ �������� �������- ������ �� �������. ������� ����� ��������� �������� ����� ������ () => {...} 
        _endTurnButton.onClick.AddListener(() =>
        {
            TurnSystem.Instance.NextTurn();

        });

        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged; // ������������� �� Event // ����� ����������� (��������� ����� ����) ������ ��� ����� ������� ����� ����.

        UpdateNamberTurnText();
        UpdateEnemyTurnVisual();
        UpdateEndTurnButtonVisibility();
    }
    


    private void TurnSystem_OnTurnChanged(object sender, EventArgs empty) // ����� ������� ����.
    {
        UpdateNamberTurnText();
        UpdateEnemyTurnVisual();
        UpdateEndTurnButtonVisibility();
    }

    private void UpdateNamberTurnText() // ���������� ����� ��������
    {
        _turnNumberText.text = ("Turn " + TurnSystem.Instance.GetTurnNumber()).ToUpper();
    }

    private void UpdateEnemyTurnVisual() //���������� ���������� �������� ��� �����
    {
        _enemyTurnVisualGameObject.SetActive(!TurnSystem.Instance.IsPlayerTurn()); // ������������ �� ��� ������ � ������� �� ��� �����
    }

    private void UpdateEndTurnButtonVisibility() // �������� ��������� ������ End Turn
    {
        _endTurnButton.gameObject.SetActive(TurnSystem.Instance.IsPlayerTurn()); // ���������� ������ �� ����� ���� ������
    }
}
