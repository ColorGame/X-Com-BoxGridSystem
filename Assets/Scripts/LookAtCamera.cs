using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour // ����� �������� ����� �������� � ������� ������
{
    [SerializeField] private bool _invert; // ��� 1 ������ �������� (������ ������� ���� ���� �������������)

    private Transform _cameraTransform;

    private void Awake()
    {
        _cameraTransform = Camera.main.transform; // �������� ��������� ������ (������ ������������� ��� �� ����� ������� ��������� ��������)
    }

    private void LateUpdate() // ����� ��������� ����� ���� Update ����� ������� ������������� ����� � ����� ����������� UI
    {
        /*// 1 �����//{ // � ������ ������ ������� UI ������ ������� �� ��������� �� ����� � ������� ��� ��������
        if (_invert) // ���� ����� ������� ��������������
        {
            Vector3 directionCamera = (_cameraTransform.position - transform.position).normalized; // �������������� ������ ������������ � ������� ������
            transform.LookAt(transform.position + directionCamera*(-1)); // ��������� � �������� - �������� �������� ����� ������� � ��������������� ������� ������� directionCamera (�� ����� ��� �� �������� �� ������ �� �����)
        }
        else
        {
            transform.LookAt(_cameraTransform);
        }// 1 �����//}*/

        // 2 ������ �����//{  � ������ ������ UI ����� ��������� ������ ������
        transform.rotation = _cameraTransform.rotation;
    }
}
