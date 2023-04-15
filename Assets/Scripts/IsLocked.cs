using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsLocked : MonoBehaviour // ��� �������������� � ������, ���� �� ������ �������, ������� ������� "�������"
{
    
    private void Start()
    {
        DoorInteract.OnAnyDoorIsLocked += DoorInteract_OnAnyDoorIsLocked; //���������� �� ������� ����� ����� �������(������ ������� �������)
        
        UnitActionSystem.Instance.OnBusyChanged += UnitActionSystem_OnBusyChanged; // ������������ �� Event ��������� ��������  � �������� UnitActionSystem_OnBusyChanged, ��� ������� ������� �� ������� ������� ��������


        Hide(); // ������ ��� ������
    }

    private void DoorInteract_OnAnyDoorIsLocked(object sender, System.EventArgs e)
    {
        Show(); 
    }
    
    private void UnitActionSystem_OnBusyChanged(object sender, bool isBusy)
    {
        if (!isBusy) // ����� ����� ������������� �� ������ �������
        {
            Hide();
        }
       
    }

    private void Show() // ��������
    {
        gameObject.SetActive(true);
    }

    private void Hide() // ������
    {
        gameObject.SetActive(false);
    }
}
