using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitRagdoll : MonoBehaviour //  ���� ��������� �����// ����� �� ��������� ����� . �������� �� �������������� ����� �� ��������� � � ����������. ����������� �� �� ������
{
    [SerializeField] private Transform _ragdollRootBone; // ��������� ����� �������� �����

    private float _explosionForce = 300; // ���� ������. � ������� ���� ��� ����� ������������� � Setup() //����� ���������//

    public void Setup(Transform originalRootBone, Unit keelerUnit) // ��������� ��������� ����� (� �������� �������� ������������ �������� �����(�����) � ����� ������� ����� ��� ����� - ������)
    {
        MatchAllChildrenTransform(originalRootBone, _ragdollRootBone);

        Vector3 explosionPosition; //�������������� ���������� ������� ������

        BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction(); // ������� ��������� ��������
                
        switch (selectedAction)
        {
            default:// ���� ���� ����� ����������� �� ��������� ���� ��� ��������������� selectedAction
            case GrenadeAction grenadeAction:
                
                Vector3 randomDir = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)); // ��� ����������� ����� ������ �� � � Z ��� �� ������� ����� �� �������
                explosionPosition = transform.position + randomDir;
                _explosionForce = 400f; //����� ���������//
                break;

            case ShootAction shootAction:
                
                Vector3 directionKeeler = (keelerUnit.transform.position - transform.position).normalized; //����������� � ������� , ��� �� ������ � ����� ������� ������� ����
                explosionPosition = transform.position + directionKeeler; // ������� ����� ������ � ������� �������            
                _explosionForce = 300f; //����� ���������//
                break;

            case SwordAction swordAction:

                directionKeeler = (keelerUnit.transform.position - transform.position).normalized; //����������� � ������� , ��� �� ������ � ����� ������� ������� ����
                explosionPosition = transform.position + directionKeeler; // ������� ����� ������ � ������� �������    
                _explosionForce = 500f; //����� ���������//
                break;

        }

        ApplyExplosionToRagdoll(_ragdollRootBone, _explosionForce, explosionPosition, 10f); // ��������� ����� � ��������� �����
        //�� ����� �� �������� ������� Damage, ����� �������� ������ Damager, �����, ����� ����� ����� ����������, ����� �������, �������� � ����� ������� ��� ���������, ����� ��������� ���� � ��������� �����
        //� �������� ������������ �� ����� �� �������  GrenadeDamageUnits, � ����� ����� ������� ������ ����� ���������� � �� ���� ��� ������ ��������� ����� �, ���� �� ������� ����� - ����, ��������� � ��� ����.

    }

    private void MatchAllChildrenTransform(Transform root, Transform clone) // ����������� ��� �������� ��������������// ��������� ��������� ����� �� ��������� ����� � ������� ����������� ����������� �������
    {
        foreach (Transform child in root) // � ����� ��������� �������� ������� �����(���������)
        {
            Transform cloneChild = clone.Find(child.name); // ������� � ���� � ���� � ��� �������� ������ � ������ ��� � ��������� ������� ���������(root) � �������� � cloneChild(������� �����)
            if (cloneChild != null)
            {
                cloneChild.position = child.position;// ������� � ������� ������� ����� ������������� ��� � ��������� ������� ���������(root)
                cloneChild.rotation = child.rotation;

                MatchAllChildrenTransform(child, cloneChild); // ����������� ������� (�������� ���� ����) ����� ��� ��������� ������� �������� ��������, ��� ����� �������� ���� ���� ���� �� ��������� �� ���
            }
        }
    }

    private void ApplyExplosionToRagdoll(Transform root, float explosionForce, Vector3 explosionPosition, float explosionRange) // ��������� ����� � ��������� ����� (explosionRange �������� ������)
    {
        foreach (Transform child in root)
        {
            if (child.TryGetComponent<Rigidbody>(out Rigidbody childrigidbody)) // ��������� �������� ��������� �������� �������� 
            {
                childrigidbody.AddExplosionForce(explosionForce, explosionPosition, explosionRange);
            }

            ApplyExplosionToRagdoll(child, explosionForce, explosionPosition, explosionRange);  // ����������� �������
        }
    }
}
