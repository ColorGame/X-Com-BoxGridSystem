using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelInteract : MonoBehaviour, IInteractable // ����� ��������������
{
    public event EventHandler OnBarrelInteractlActivated; //�������� ������� ����� - ����� �������������� �������������

    [SerializeField] private Transform _barrelDestroyedPrefab; //������ ����������� �����
    [SerializeField] private GameObject _visualGameObject; // ������ �����(����� ��������� �� ����� ��������������) �������� ������ ����� �����


    private GridPosition _gridPosition;
    private Action _onInteractionComplete; // ������� �������������� ���������// �������� ������� � ������������ ���� - using System;
                                           //�������� ��� ������� ��� ������������ ���������� (� ��� ����� ��������� ������ ������� �� ���������).
                                           //Action- ���������� �������. ���� ��� �������. ������� Func<>. 
                                           //�������� ��������. ����� ���������� ������� � ������� �� �������� ��������, ����� �����, � ������������ ����� ����, ����������� ����������� �������.
                                           //�������� ��������. ����� �������� �������� ������� �� ������� ������
    private bool _isActive;
    private float _timer;// ������ ��� �� �������������� ���� �� ���������� � ������ ������ ���� ��������� ��������� �����

    private void Start()
    {
        _gridPosition = LevelGrid.Instance.GetGridPosition(transform.position); // ������� �������� ������� �����
        LevelGrid.Instance.SetInteractableAtGridPosition(_gridPosition, this); // ���������� ���������� ��������� �������������� � ���� �������� �������
        Pathfinding.Instance.SetIsWalkableGridPosition(_gridPosition, false); // ���������� ��� ����� ��� ������ (� ����������� �� isWalkable)  ������ �� ���������� � �������� �������� �������
    }

    private void Update()
    {
        if (!_isActive) // ���� ���������� �������������� � ������ �� ��� ������� � ����������� ��� ����
        {
            return; // ������� � ���������� ��� ����
        }

        _timer -= Time.deltaTime; // �������� ������

        if (_timer <= 0f) 
        {
            _isActive = false;// ��������� �������� ���������                    
            Destroy(gameObject); // ��������� ������� ������ "�����" 
            _onInteractionComplete(); // ������� ����������� ������� ������� ��� �������� ������� Interact(). � ����� ������ ��� ActionComplete() �� ������� ��������� � ������ UI
        }
    }

    public void Interact(Action onInteractionComplete) // ��������������. � �������� ������� ������� �������������� ���������
    {
        _onInteractionComplete = onInteractionComplete;// �������� ���������� �������
        _isActive = true; //����������� ����� � �������� ���������
        _timer = 0.5f;// ������ ����� ��������� ���������  //����� ���������//

        _visualGameObject.SetActive(false); // �������� ������ ����� ����� (�� ���� �����)

        Transform barrelDestroyedTransform = Instantiate(_barrelDestroyedPrefab, transform.position, transform.rotation); // �������� ����������� �����
        ApplyExplosionToChildren(barrelDestroyedTransform, 150f, transform.position, 10f); // �������� ���� � ����������� �����

        LevelGrid.Instance.ClearInteractableAtGridPosition(_gridPosition); // ������� ��������� �������������� � ���� �������� �������
        Pathfinding.Instance.SetIsWalkableGridPosition(_gridPosition, true); // ���������� ��� ����� ������ �� ���� ������

        OnBarrelInteractlActivated?.Invoke(this, EventArgs.Empty); // �������� ������� - ����� �������������� �������������
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
