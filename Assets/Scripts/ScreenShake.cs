using Cinemachine; // ������� ��� ������ � Cinemachine
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//��������������� ��������� - � CinemachineVirtualCamera �� ������� Add Extension �������� Cinemachine Impulse Listener //https://docs.unity3d.com/Packages/com.unity.cinemachine@2.9/manual/CinemachineImpulseListener.html
// Secondary Noise  -6DShake    ��������� ���
// Amplituda Gain   -10         ����������� �������� �� ���������
// Frequency Gain   -5          �������� �������
// Duration         -0.5        �����������������

public class ScreenShake : MonoBehaviour //�������� ������ //���������� ������������ ������� ������� ����� ������� CinemachineVirtualCamera
{
    public static ScreenShake Instance { get; private set; }   //(������� SINGLETON) ��� �������� ������� ����� ���� ������� (SET-���������) ������ ���� �������, �� ����� ���� �������� GET ����� ������ �������
                                                               // instance - ���������, � ��� ����� ���� ��������� ScreenShake ����� ���� ��� static. Instance ����� ��� ���� ����� ������ ������, ����� ����, ����� ����������� �� Event.


    private CinemachineImpulseSource _cinemachineImpulseSource; // �������� ������������������� ���������

    private void Awake()
    {
        // ���� �� �������� � ���������� �� �������� �� �����
        if (Instance != null) // ������� �������� ��� ���� ������ ���������� � ��������� ����������
        {
            Debug.LogError("There's more than one ScreenShake!(��� ������, ��� ���� ScreenShake!) " + transform + " - " + Instance);
            Destroy(gameObject); // ��������� ���� ��������
            return; // �.�. � ��� ��� ���� ��������� ScreenShake ��������� ����������, ��� �� �� ��������� ������ ����
        }
        Instance = this;

        _cinemachineImpulseSource = GetComponent<CinemachineImpulseSource>();
    }     

    public void Shake(float intensity = 1f) // ��������. � �������� �������� �������� ������������� � �� ��������� ��������� �� 1
    {
        _cinemachineImpulseSource.GenerateImpulse(intensity); //������� ������� ������������ �������
    }


    //��� ����� � ��������� ���� ������
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            _cinemachineImpulseSource.GenerateImpulse(5);
        }
    }
}
