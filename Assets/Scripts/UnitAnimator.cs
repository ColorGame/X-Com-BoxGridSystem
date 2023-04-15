using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimator : MonoBehaviour // �������� ����� � �������� ���� (�����: � ���������� ����� ������� ����� ������ Arms � ��������� ���, ����� � ���� ������� � ����� ��������� ����)
{
    [SerializeField] private Animator _animator;
    [SerializeField] private Transform _bulletProjectilePrefab; // � ���������� �������� ������ ����
    [SerializeField] private Transform _shootPointTransform; // � ���������� �������� ����� �������� ����� �� ��������

    // ���� ������� ������� ������ �� ����� ������� ��������� ������ ������� �������� �� ����� ������
    // �������� �� ������ �������� ������ https://community.gamedev.tv/t/weapon-manager/213840
    [SerializeField] private Transform _rifleTransform; //� ���������� �������� ��������� ��������
    [SerializeField] private Transform _swordTransform; //� ���������� �������� ��������� ����

    private void Awake()
    {
        if (TryGetComponent<MoveAction>(out MoveAction moveAction)) // ��������� �������� ��������� MoveAction � ���� ���������� �������� � moveAction
        {
            moveAction.OnStartMoving += MoveAction_OnStartMoving; // ���������� �� �������
            moveAction.OnStopMoving += MoveAction_OnStopMoving; // ���������� �� �������
        }

        if (TryGetComponent<ShootAction>(out ShootAction shootAction)) // ��������� �������� ��������� ShootAction � ���� ���������� �������� � shootAction
        {
            shootAction.OnShoot += ShootAction_OnShoot; // ���������� �� �������
        }
        
        if (TryGetComponent<SwordAction>(out SwordAction swordAction)) // ��������� �������� ��������� SwordAction � ���� ���������� �������� � swordAction
        {
            swordAction.OnSwordActionStarted += SwordAction_OnSwordActionStarted; // ���������� �� �������
            swordAction.OnSwordActionCompleted += SwordAction_OnSwordActionCompleted;
        }

    }

    private void Start()
    {
        EquipRifle(); // ������� ��������
    }

    private void SwordAction_OnSwordActionCompleted(object sender, EventArgs e) // ����� �������� ��������� �������� ��� �� ��������
    {
        EquipRifle(); // ������� ��������
    }

    private void SwordAction_OnSwordActionStarted(object sender, EventArgs e)
    {
        EquipSword();   // ������� ���
        _animator.SetTrigger("SwordSlash");// ���������� ������( ������ ��������� ������)
    }

    private void MoveAction_OnStartMoving(object sender, EventArgs empty)
    {
        _animator.SetBool("IsWalking", true); // �������� �������� ������w
    }
    private void MoveAction_OnStopMoving(object sender, EventArgs empty)
    {
        _animator.SetBool("IsWalking", false); // ��������� �������� ������
    }

    private void ShootAction_OnShoot(object sender, ShootAction.OnShootEventArgs e) 
    {
        _animator.SetTrigger("Shoot"); // ���������� ������( ������ ��������� ������)

        Transform bulletProjectilePrefabTransform = Instantiate(_bulletProjectilePrefab, _shootPointTransform.position, Quaternion.identity); // �������� ������ ���� � ����� ��������
        
        BulletProjectile bulletProjectile  = bulletProjectilePrefabTransform.GetComponent<BulletProjectile>(); // ������ ��������� BulletProjectile ��������� ����

        Vector3 targetUnitShootAtPosition = e.targetUnit.GetWorldPosition(); // ������� ������� �������� �����. (������� ���� �������� �� �������)

        targetUnitShootAtPosition.y = _shootPointTransform.position.y; // � ���������� ���� ����� ��������� �������������
        bulletProjectile.Setup(targetUnitShootAtPosition); // � �������� ������� ������� ������� �������� �����. � �������������� ����������� �� �
    }

    private void EquipSword() // ���������� ���
    {
        _swordTransform.gameObject.SetActive(true);
        _rifleTransform.gameObject.SetActive(false);
    }

    private void EquipRifle() // ���������� ��������
    {
        _swordTransform.gameObject.SetActive(false);
        _rifleTransform.gameObject.SetActive(true);
    }

}
