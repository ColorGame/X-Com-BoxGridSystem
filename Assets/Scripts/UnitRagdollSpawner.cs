using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitRagdollSpawner : MonoBehaviour // ���� ��������� ����� ����������// ����� �� �����
{
    [SerializeField] private Transform _ragdollPrefab; // ������ ��������� �����
    [SerializeField] private Transform _originalRootBone; // ������������ �������� �����(�����) // � ���������� �������� ����� ����� ��� ��������� Root 

    private HealthSystem _healthSystem;

    private Unit _keelerUnit; //�������� ����� ������� ����� ��� ����� ������.
    private Unit _unit; // ���� �� ������� ����� ���� ������

    private void Awake()
    {
        _healthSystem = GetComponent<HealthSystem>();       
        _unit= GetComponent<Unit>();
    }

    private void Start()
    {
        _healthSystem.OnDead += HealthSystem_OnDead; // ���������� �� ������� ���� (�������� ������� ����� ���� ����)

        ShootAction.OnAnyShoot += ShootAction_OnAnyShoot; // ���������� �� ������� ����� ����� �������� (�� OnShoot �� ���� ����������� �.�. �� �� static � ����� ������ �� ������� ������)
        SwordAction.OnAnySwordHit += SwordAction_OnAnySwordHit; // ���������� �� c������ - ����� ����� ���� �����
    }

    private void SwordAction_OnAnySwordHit(object sender, SwordAction.OnSwordEventArgs e)
    {
        if (e.targetUnit == _unit) // ���� ����� �������� ���� ���� , �� �������� ���� ��� �� ��� �������
        {
            _keelerUnit = e.hittingUnit;
        }
    }

    private void ShootAction_OnAnyShoot(object sender, ShootAction.OnShootEventArgs e)
    {
        if(e.targetUnit== _unit) // ���� ����� �������� ���� ���� , �� �������� ���� ��� �� ��� �������
        {
            _keelerUnit = e.shootingUnit;
        }        
    }
        

    private void HealthSystem_OnDead(object sender, System.EventArgs e)
    {
        Transform ragdollTransform = Instantiate(_ragdollPrefab, transform.position, transform.rotation); // �������� ����� �� ������� � ������� �����

        UnitRagdoll unitRagdoll = ragdollTransform.GetComponent<UnitRagdoll>(); // ������ ��������� UnitRagdoll �� �������
               
        unitRagdoll.Setup(_originalRootBone, _keelerUnit); // � ��������� � ����� Setup ��������� ������������ �������� ����� � �������
    }
}
