using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class DestructibleCrate : MonoBehaviour // ����������� ����
{
    public static event EventHandler OnAnyDestroyed; // static - ���������� ��� event ����� ������������ ��� ����� ������, � �� ��� ����������� �����. ������� ��� ������������� ����� ������� ��������� �� ����� ������ �� ���������� ������, ��� ����� �������� ������ � ������� ����� �����, ������� ����� ��������� ���� � �� �� ������� ��� ������� �������. 
                                                     // �� �������� ������� Event ����(Any) ������ ��������.
   
    [SerializeField] private Transform _crateDestroyedPrefab; // ������ ������������ ����� 

    private GridPosition _gridPosition; // ������� ����� ������ �����


    private void Start()
    {
        _gridPosition = LevelGrid.Instance.GetGridPosition(transform.position); //������� �������� ������� �����
    }
    public void Damage()
    {
       Transform crateDestroyedTransform = Instantiate(_crateDestroyedPrefab, transform.position, transform.rotation); // ������ ��� ���������� ���� �������� �� ��� ����� �����������

        ApplyExplosionToChildren(crateDestroyedTransform, 150f, transform.position, 10f); // �������� ����� � ������������ �����, � ����� 150, � ��� �� �������, � ������ �������� 10

        Destroy(gameObject); //��������� �������

        OnAnyDestroyed?.Invoke(this, EventArgs.Empty);
    }

    public GridPosition GetGridPosition()
    {
        return _gridPosition;
    }

    private void ApplyExplosionToChildren(Transform root, float explosionForce, Vector3 explosionPosition, float explosionRange) // ��������� ����� � ����� (explosionRange �������� ������)
    {
        foreach (Transform child in root)
        {
            if (child.TryGetComponent<Rigidbody>(out Rigidbody childrigidbody)) // ��������� �������� ��������� �������� �������� 
            {
                childrigidbody.AddExplosionForce(explosionForce, explosionPosition, explosionRange);
            }

            ApplyExplosionToChildren(child, explosionForce, explosionPosition, explosionRange);  // ����������� �������
        }
    }
}
