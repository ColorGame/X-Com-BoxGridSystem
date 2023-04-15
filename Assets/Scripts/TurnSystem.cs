using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnSystem : MonoBehaviour // ������� ������� (�����) . ������������ ������ �����
{
    public static TurnSystem Instance { get; private set; }   //(������� SINGLETON) ��� �������� ������� ����� ���� ������� (SET-���������) ������ ���� �������, �� ����� ���� �������� GET ����� ������ �������
                                                              // instance - ���������, � ��� ����� ���� ��������� UnitActionSystem ����� ���� ��� static. Instance ����� ��� ���� ����� ������ ������, ����� ����, ����� ����������� �� Event.

    public event EventHandler OnTurnChanged; // ��� ������� (����� �������� ��� �� �������� ������� Event)

    private int _turnNumber = 1; // ����� ������� (����)
    private bool _isPlayerTurn = true; // ��� ������, �� ��������� true �.�. �� ����� ������


    private void Awake() //��� ��������� ������ Awake ����� ������������ ������ ��� ������������� � ���������� ��������
    {
        // ���� �� �������� � ���������� �� �������� �� �����
        if (Instance != null) // ������� �������� ��� ���� ������ ���������� � ��������� ����������
        {
            Debug.LogError("There's more than one TurnSystem!(��� ������, ��� ���� TurnSystem!) " + transform + " - " + Instance);
            Destroy(gameObject); // ��������� ���� ��������
            return; // �.�. � ��� ��� ���� ��������� TurnSystem ��������� ����������, ��� �� �� ��������� ������ ����
        }
        Instance = this;
    }

    public void NextTurn() // ��������� ������� (���) �������� ��� ������� �� Button "END TURN"
    {
        _turnNumber++; // ����������� ����� �� �������
        _isPlayerTurn = !_isPlayerTurn; // ���������� ��� ������. ������ ������ ��� ����� ������� ��� ������

        OnTurnChanged?.Invoke(this, EventArgs.Empty); // "?"- ��������� ��� !=0. Invoke ������� (this-������ �� ������ ������� ��������� ������� "�����������" � ����� TurnSystemUI � UnitActionSystemUI, Unit ����� ��� ������������ "������������")
    }

    public int GetTurnNumber() //������� ����� ����
    {
        return _turnNumber;
    }

    public bool IsPlayerTurn()
    {
        return _isPlayerTurn;
    }
}
