using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// � ������ ������� ActionVirtualCamera ����� ���� ����������� ������ ��� ����� ��������, �������� �� ��� � �������� CinemachineVirtualCamera
// �� ������� Add Extension �������� Cinemachine Impulse Listener � ������ �� ����������� ��� � � �������� (����� ����������� ��������� � �������� � �������� � ������ ��� ����� ������)

public class ScreenShakeActions : MonoBehaviour //����������� ����� ������� ��������� ������ ������ (��������� �����)
{


    private void Start()
    {
        ShootAction.OnAnyShoot += ShootAction_OnAnyShoot; // ���������� �� ������� ����� ����� ��������
        GrenadeProjectile.OnAnyGrenadeExploded += GrenadeProjectile_OnAnyGrenadeExploded;// ���������� �� ������� ����� ������� ����������
        SwordAction.OnAnySwordHit += SwordAction_OnAnySwordHit;// ���������� �� �������  ����� ����� ���� �����
    }

    private void SwordAction_OnAnySwordHit(object sender, SwordAction.OnSwordEventArgs e)
    {
        ScreenShake.Instance.Shake(2); // �� ��������� ������������� ������ = 1
    }

    private void GrenadeProjectile_OnAnyGrenadeExploded(object sender, System.EventArgs e)
    {
        ScreenShake.Instance.Shake(4f); // ������������� ������ ��� ������ ������� ��������� 2 //����� ���������//
    }

    private void ShootAction_OnAnyShoot(object sender, ShootAction.OnShootEventArgs e)
    {
        ScreenShake.Instance.Shake(); // �� ��������� ������������� ������ = 1
    }
   
}
