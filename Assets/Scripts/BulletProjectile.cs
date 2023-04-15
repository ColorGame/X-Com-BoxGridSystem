using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BulletProjectile : MonoBehaviour // ������ ����
{
    [SerializeField] private TrailRenderer _trailRenderer; // � ���������� �������� ����� ���� �� ����� � ����� ���� // � TRAIL �������� ��������� ������� Autodestruct
    [SerializeField] private Transform _bulletHitVfxPrefab; // � ���������� �������� ������� ������ (����� �� ���������)

    private Vector3 _targetPosition; // ������� ������� ����
    public void Setup(Vector3 targetPosition) // ��������� ����
    {
        _targetPosition = targetPosition;
    }

    private void Update()
    {
        Vector3 moveDirection = (_targetPosition - transform.position).normalized; // ����������� ��������, ��������� ������

        //float distanceBeforeMoving = Vector3.Distance(transform.position, _targetPosition); //(���������� �� ��������) ������ � �������� ��������� �� �������� �����, ������ ��� ������ ���������
        //float moveSpead = 200f;
        //transform.position += moveDirection * moveSpead * Time.deltaTime; // ���������� ����
        //float distanceAfterMoving = Vector3.Distance(transform.position, _targetPosition); //(���������� ����� ��������) ������ � �������� ��������� �� �������� �����, ����� ������ ��������

        // ���� ��� ����� �������� ����� ������ ��������� ��������, �� ������ �������� �� ���������� �����, ��������� sqrMagnitude .
        // ���������� ��������� ������� ���������� ������� �������:
        float sqrMagnitudeBeforeMoving = (transform.position - _targetPosition).sqrMagnitude; //(������� ���������� �� ��������) ������ � �������� ������� ��������� �� �������� �����, ������ ��� ������ ���������
        float moveSpead = 200f;
        transform.position += moveDirection * moveSpead * Time.deltaTime; // ���������� ����
        float sqrMagnitudeAfterMoving = (transform.position - _targetPosition).sqrMagnitude; //(������� ���������� ����� ��������) ������ � �������� ������� ��������� �� �������� �����, ����� ������ ��������


        if (sqrMagnitudeBeforeMoving < sqrMagnitudeAfterMoving) // if ���������, ����� �� ��������� ����, � ��������� �� ���, �� ���������� ������, ��� ����� �� ���� �� ����. ����� � ���� ��� ���������� ����������� ����������. ���� ������ stoppingDistance ��� � MoveAction �� �� ������� �������� ���� ����� ����� ���������� ����� ���� �������� ��������� ���������.
        {
            transform.position = _targetPosition; // �� ��������� ���� ������� ������ ���� � ������� �������. (��� �������)

            _trailRenderer.transform.parent = null; // ���������� ����� �� �������� ��� �� �� ��� ���. � � ���������� �������� ������� Autodestruct - ����������� ����� ���������� ����������

            Destroy(gameObject);

            Instantiate(_bulletHitVfxPrefab, _targetPosition, Quaternion.identity); // �������� ������ ������ (�� ������ � ���������� �������� � ������ Stop Action - Destroy)
        }
    }
}
