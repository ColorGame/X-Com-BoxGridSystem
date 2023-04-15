using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereInteract : MonoBehaviour, IInteractable // �������������� � ������ // �������� ����� ����������� 
{


    public event EventHandler OnInteractSphereActivated; //�������� ������� ����� - ����� �������������� �������������

    [SerializeField] private Material _greenMaterial;
    [SerializeField] private Material _redMaterial;
    [SerializeField] private MeshRenderer _meshRenderer;

    [SerializeField] private bool _isGreen; //�� ������� (��� ������������ ��������� ����)

    private GridPosition _gridPosition; // �������� ������� ����    
    private Action _onInteractionComplete; // ������� �������������� ���������// �������� ������� � ������������ ���� - using System;
                                           //�������� ��� ������� ��� ������������ ���������� (� ��� ����� ��������� ������ ������� �� ���������).
                                           //Action- ���������� �������. ���� ��� �������. ������� Func<>. 
                                           //�������� ��������. ����� ���������� ������� � ������� �� �������� ��������, ����� �����, � ������������ ����� ����, ����������� ����������� �������.
                                           //�������� ��������. ����� �������� �������� ������� �� ������� ������
    private bool _isActive;
    private float _timer; // ������ ������� �� ����� ��������� ���������� ����������������� � �����



    private void Start()
    {
        if (gameObject.activeSelf) // ���� ��� � �������� ��������� �� ...
        {
            UpdateInteractableAtGridPosition(); // �������� �������������� � �������� ��������
        }

        if (_isGreen) // ���������� ������ � ������������ � �������� ����������
        {
            SetColorGreen();
        }
        else
        {
            SetColorRed();
        };
    }

    public void UpdateInteractableAtGridPosition() // �������� �������������� � �������� ��������
    {
        _gridPosition = LevelGrid.Instance.GetGridPosition(transform.position); // ��������� �������� ������� ����
        LevelGrid.Instance.SetInteractableAtGridPosition(_gridPosition, this); // � � ���������� �������� ������� ��������� ��� ��� � ����������� Interactable(��������������)
    }



    private void Update()
    {
        if (!_isActive) // ���� ���������� �������������� � ����� �� ��� ������� � ����������� ��� ����
        {
            return;// ������� � ���������� ��� ����
        }

        _timer -= Time.deltaTime; // �������� ������

        if (_timer <= 0f)
        {
            _isActive = false; // ��������� �������� ���������
            _onInteractionComplete(); // ������� ����������� ������� ������� ��� �������� ������� Interact(). � ����� ������ ��� ActionComplete() �� ������� ��������� � ������ UI
        }
    }

    private void SetColorGreen()
    {
        _isGreen = true;
        _meshRenderer.material = _greenMaterial;
    }

    private void SetColorRed()
    {
        _isGreen = false;
        _meshRenderer.material = _redMaterial;
    }

    public void Interact(Action onInteractionComplete) // ��������������. � �������� ������� ������� �������������� ��������� 
    {
        _onInteractionComplete = onInteractionComplete; // �������� ���������� �������
        _isActive = true; // ����������� ��� � �������� ���������
        _timer = 1.5f; // ������ ����� ��������� ���������  //����� ���������//

        if (_isGreen) // ��� �������������� ������� ����. ���� ��� �������� �� ���������� �� ��������
        {
            SetColorRed();
        }
        else
        {
            SetColorGreen();
            OnInteractSphereActivated?.Invoke(this, EventArgs.Empty); // �������� ������� - ����� �������������� �������������
        }

    }
}
