using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; // ��� ���������� ��������

public class SpinAction : BaseAction // ��������
{

    private float _totalSpinAmount; // ����� ����� ��������


    private void Update()
    {
        if (!_isActive) // ���� �� ������� �� ...
        {
            return; // ������� � ���������� ��� ����
        }

        float spinAddAmount = 360f * Time.deltaTime;
        transform.eulerAngles += new Vector3(0, spinAddAmount, 0); // ������� ����� ������ ��� �

        _totalSpinAmount += spinAddAmount; // ������ ���� �������� ���� �� ������� �� ���������

        if (_totalSpinAmount >= 360f) // ��� ������ �������� �� 360 ��������...
        {
            ActionComplete(); // ������� ������� ������� �������� ���������
        }
    }


    // ������������� TakeAction (��������� �������� (�����������)) // �� ������������� Spin � TakeAction � �������� � �������� GridPosition
    public override void TakeAction(GridPosition gridPosition, Action onActionComplete) // (onActionComplete - �� ���������� ��������). � �������� ����� ���������� ������� Action 
                                                                                        // � ������ ������ �������� �������� ������� �� �� ���������� - GridPosition _gridPosition - �� �������� ���� ��� ���� ����� ��������������� ��������� ������� ������� TakeAction.
                                                                                        // ���� ������ ������, ������� ���������� -
                                                                                        // public class BaseParameters{} 
                                                                                        // � ����������� � ������� ����� �������������� ��� ������� �������� -
                                                                                        // public SpinBaseParameters : BaseParameters{}
                                                                                        // ����� ������� - public override void TakeAction(BaseParameters baseParameters ,Action onActionComplete){
                                                                                        // SpinBaseParameters spinBaseParameters = (SpinBaseParameters)baseParameters;}
    {
        _totalSpinAmount = 0f; // ��� ������ Spin() �������� �������� �������                               

        ActionStart(onActionComplete); // ������� ������� ������� ����� �������� // �������� ���� ����� � ����� ����� ���� �������� �.�. � ���� ������ ���� EVENT � �� ������ ����������� ����� ���� ��������
    }

    public override string GetActionName() // ��������� ������� �������� //������� ������������� ������� �������
    {
        return "Spin";
    }

    public override List<GridPosition> GetValidActionGridPositionList() // �������� ������ ���������� �������� ������� ��� �������� // ������������� ������� �������
                                                                        // ���������� �������� ������� ��� �������� �������� ����� ������ ��� ����� ���� 
    {
        GridPosition unitGridPosition = _unit.GetGridPosition(); // ������� �������� ������� ����� 

        return new List<GridPosition> // �������� ������ � ������� � ��� �������� ������� �����, � ����� ������ ��
        {
            unitGridPosition
        };
    }

    public override int GetActionPointCost() // ������������� ������� ������� // �������� ������ ����� �� �������� (��������� ��������)
    {
        return 1;
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition) //�������� �������� ���������� �� // ������������� ����������� ������� �����
    {
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 0, //�������� ������ �������� ��������. ����� ��������� �������� ���� ������ ������� ������� �� �����, 
        };
    }
}
