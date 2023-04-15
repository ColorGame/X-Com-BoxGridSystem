using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitWorldUI : MonoBehaviour // ������� ���������������� ��������� ����� //����� � canvas �� �����
{
    [SerializeField] private TextMeshProUGUI _actionPointsText; // �������� ���� UI
    [SerializeField] private Unit _unit; // � ���������� �������� �����
    [SerializeField] private Image _healthBarImage; // � ���������� �������� ����� �������� "Bar"
    [SerializeField] private HealthSystem _healthSystem; // �������� ������ ����� �� ������ ����� �� ���
    [SerializeField] private TextMeshProUGUI _healthPointsText; // �������� ����� ��������


    private void Start()
    {
        Unit.OnAnyActionPointsChanged += Unit_OnAnyActionPointsChanged; // ���������� �� ����������� ������� (����� ��������� ����� ��������) // ��������� ���������� - ��� ������� ���������� ����� ���������� ActionPoints � ������ �����, � ��� ������� ������������� �� �������������
        _healthSystem.OnDamage += HealthSystem_OnDamage; // ���������� �� ������� ������� �����������.

        UpdateActionPointsText();
        UpdateHealthBar();

       
    }


    private void UpdateActionPointsText() // ���������� ������ ����� ��������
    {
        _actionPointsText.text = _unit.GetActionPoints().ToString(); // ������ ���� �������� ����� ����������� � ������ � ��������� � ����� ������� ������������ ��� ������
    }

    private void Unit_OnAnyActionPointsChanged(object sender, EventArgs e)
    {
        UpdateActionPointsText();
    }

    /*//  ���� �� ������ ����� �����, ����� ���� ��������� ��������� � OnAnyActionPointsChanged, ��� ������ ����� ������� ����������� ��� �����.
    private void Unit_OnAnyActionPointsChanged(object sender, EventArgs args)
    {
        Unit unit = sender as Unit;
        Debug.Log($"� {unit} ���� �������� ����������.");
    }*/
    private void UpdateHealthBar() // ���������� ����� ��������
    {
        _healthBarImage.fillAmount = _healthSystem.GetHealthNormalized();
        _healthPointsText.text = _healthSystem.GetHealth().ToString();

    }
    private void HealthSystem_OnDamage(object sender, EventArgs e) // ��� ����������� ������� ������� ����� �����
    {
        UpdateHealthBar();
    }

}
