using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private GameObject _actionCameraGameObject; // � ���������� ������� ActionVirtualCamera � �������� �� ��� � ���������� ��������� Priority = 20// �� �������� ������ � ������� Cinemachine ��������� ����� �������� Defauld Blend = 0.2


    [SerializeField] LayerMask _actionCameraCullingMaskNoUI;//��������� ���� "���������������� ���������" �� ����� ����-������ ��������� ��������. //��������� � ����� ������ CANVAS - Render mode - Screen space Camera

    private LayerMask _normalCameraCullingMask;

    private void Start()
    {
        BaseAction.OnAnyActionStart += BaseAction_OnAnyActionStart; // ���������� �� ������� ����� ����������� ��� ������ ������ ��������
        BaseAction.OnAnyActionCompleted += BaseAction_OnAnyActionCompleted; //���������� �� ������� ����� ����������� ��� ���������� ������ ��������
        LevelScripting.Instance.OnInteractSphereAndDoor += Instance_OnInteractSphereAndDoor; // ���������� �� ������� ����� ����������� ����� ����� � ����� ��������������� 

        _normalCameraCullingMask = Camera.main.cullingMask; // �������� ���������� ��������� ����� ���������� ������

        HideActionCamera(); //�� ������ ������ ������ ������ , ���� ������� ������� ��������� ActionVirtualCamera � ����� 

    }

    private void Instance_OnInteractSphereAndDoor(object sender, DoorInteract e)
    {
        Transform transformDoor = e.gameObject.GetComponent<Transform>(); // ������� Transform ����� ������� ���������

        Vector3 cameraCharacterHeight = Vector3.up * 10f; // �������� ������ �� ������

        float ZAxisOffsetAmount = 5f; // �������� �������� ������������ ��� Z 
        Vector3 ZAxisOffset = Quaternion.Euler(0, -90, 0) * transformDoor.forward * ZAxisOffsetAmount; // ����� ����������� ��������� ��� Z �����, ������������ �� �� -90 �������� �� ��� Y (�����) � ��������� ZAxisOffsetAmount � ���� �����������, ����� �������� �����, � ������� ������ ���������� ������.)

        Vector3 actionCameraDoorPosition =
                   transformDoor.position +        //������� �����
                   cameraCharacterHeight +         //�������� �� ������
                   ZAxisOffset +                    //������� � ����� ������������ ��������� ��� Z 
                   (transformDoor.forward * -10);          //������� ������ �����

        _actionCameraGameObject.transform.position = actionCameraDoorPosition;// ���������� ���� ����� ������ � ����������� �������
        _actionCameraGameObject.transform.LookAt(transformDoor.position + Vector3.up * 2f); // ��������� �������� 2 � ������� ����� 

        ShowActionCamera();
    }

    private void ShowActionCamera() // �������� ������ ��������
    {
        Camera.main.cullingMask = _actionCameraCullingMaskNoUI; // �������� ���������� ����� ������ ��� �� �� ���������� UI
        _actionCameraGameObject.SetActive(true);
    }

    private void HideActionCamera() // ������ ������ ��������
    {
        Camera.main.cullingMask = _normalCameraCullingMask; // ������ ����������� ��������� ���������� ����� �����
        _actionCameraGameObject.SetActive(false);
    }

    private void BaseAction_OnAnyActionStart(object sender, System.EventArgs e)
    {
        // ����� ��������� ����������� ��� �� ������� ������ ������� � �������� ������� ������������ ������� ShootAction
        // if (sender is ShootAction) { }
        // ��� ������������ ������������� ������� � ���������� ����� ��������� ������ �������
        switch (sender)
        {
            case ShootAction shootAction:
                Unit shooterUnit = shootAction.GetUnit(); // ������ �������
                Unit targetUnit = shootAction.GetTargetUnit(); // ������ � ���� ��������

                Vector3 cameraCharacterHeight = Vector3.up * 1.7f; // �������� ������ �� ������� �����

                Vector3 shootDirection = (targetUnit.GetWorldPosition() - shooterUnit.GetWorldPosition()).normalized; //��������� ������ ����������� ��������

                float shoulderOffsetAmount = 0.5f; // �������� �������� ������������ �����
                Vector3 shoulderOffset = Quaternion.Euler(0, 90, 0) * shootDirection * shoulderOffsetAmount; // �������� ������������ ����� (�� ����� ����������� �� ���� (shootDirection), ������������ �� �� 90 �������� �� ��� Y (������) � ��������� shoulderOffsetAmount � ���� �����������, ����� �������� �����, � ������� ������ ���������� ������.)

                Vector3 actionCameraPosition =
                    shooterUnit.GetWorldPosition() +    //������� � ��������� ��� �������
                    cameraCharacterHeight +             //�������� �� ������
                    shoulderOffset +                    //������� � ����� ������������ ����� 
                    (shootDirection * -1);              //������� ������ �����

                _actionCameraGameObject.transform.position = actionCameraPosition; // ���������� ���� ����� ������ � ����������� �������
                _actionCameraGameObject.transform.LookAt(targetUnit.GetWorldPosition() + cameraCharacterHeight); // ��������� � ������� ����� �� ������ cameraCharacterHeight (targetUnit.GetWorldPosition() ������ ������� � ��������� ���)

                ShowActionCamera();
                break;

            case SwordAction swordAction:
                Unit swordUnit = swordAction.GetUnit(); // ������ ����� ������� �������                
                targetUnit = swordAction.GetTargetUnit(); // ������ ����� �������� �������

                cameraCharacterHeight = Vector3.up * 5f; // �������� ������ 

                Vector3 swordDirection = (targetUnit.GetWorldPosition() - swordUnit.GetWorldPosition()).normalized; //��������� ������ ����������� ����� �����

                float shoulderOffsetSwordAmount = 6f; // �������� �������� ������������ �����
                Vector3 shoulderSwordOffset = Quaternion.Euler(0, 90, 0) * swordDirection * shoulderOffsetSwordAmount;

                Vector3 actionCameraSwordPosition =
                    swordUnit.GetWorldPosition() +  //������� � ��������� ��� �������
                    cameraCharacterHeight +         //�������� �� ������
                    shoulderSwordOffset +         //������� � ����� ������������ �����
                    (swordDirection * -5);          //������� ������ �����

                _actionCameraGameObject.transform.position = actionCameraSwordPosition;
                _actionCameraGameObject.transform.LookAt(targetUnit.GetWorldPosition() + Vector3.up * 1.7f); // ��������� � ������� ����� �� ������ ����� (targetUnit.GetWorldPosition() ������ ������� � ��������� ���)

                ShowActionCamera();
                break;

        }
    }
    private void BaseAction_OnAnyActionCompleted(object sender, System.EventArgs e)
    {
        //����� ������������ ������������� ������� � ���������� ����� ��������� ������ �������
        switch (sender)
        {
            case ShootAction shootAction:
                HideActionCamera();
                break;

            case SwordAction swordAction:
                HideActionCamera();
                break;

            case InteractAction interactAction:
                HideActionCamera();
                break;
        }
    }
}
