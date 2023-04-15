using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static GridSystemVisual;
using System;

public class ActionButtonUI : MonoBehaviour // ������������ ������ ������� �� ������. ����� �� ����� ������
{
    [SerializeField] private TextMeshProUGUI _textMeshPro; // TextMeshProUGUI ��� ����������������� ����������
    [SerializeField] private Button _button;
    [SerializeField] private GameObject _selectedButtonVisualUI; // ����� �������� � ����. GameObject ��� �� ������ ��� �������� ����� ������ // � ���������� ���� �������� �����

    [SerializeField] private Sprite _spriteShoot; // � ������� ������ �������� ��������������� ������ ������
    [SerializeField] private Sprite _spriteGrenade;
    [SerializeField] private Sprite _spriteMove;
    [SerializeField] private Sprite _spriteSpin;
    [SerializeField] private Sprite _spriteSword;
    [SerializeField] private Sprite _spriteInteract;


    private BaseAction _baseAction;
    //3//{ ������ ������ ������ ������ ����� ����� ���������
    private Color _textColor;
    //3//}


    public void SetBaseAction(BaseAction baseAction) // ��������� ������� �������� �� ������ (� �������� �������� ���� baseAction)
    {
        _baseAction = baseAction; // �������� ���������� ��� ������� ��������

        _textMeshPro.text = baseAction.GetActionName().ToUpper(); // � �������� ������ ������� ���������� ��� ��������� ��������  //ToUpper()- � ������� ��������

        //3//{ ������ ������ ������ ������ ����� ����� ���������
        _textColor = _textMeshPro.color; // �������� ���� ������
        //3//}

        // �.�. ������ ��������� ����������� �� � ������� ����������� � ������� � �� � ����������
        //������� ������� ��� ������� �� ���� ������// AddListener() � �������� ������ �������� �������- ������ �� �������. ������� ����� ��������� �������� ����� ������ () => {...} 
        _button.onClick.AddListener(() =>
        {
            UnitActionSystem.Instance.SetSelectedAction(baseAction); //���������� ��������� ��������
        });

        switch (_baseAction) // � ����������� �� ���������� �������� �������� ��������� ������ ������
        {
            case ShootAction shootAction:
                _button.image.sprite = _spriteShoot;
                break;

            case GrenadeAction grenadeAction:
                _button.image.sprite = _spriteGrenade;
                break;

            case MoveAction moveAction:
                _button.image.sprite = _spriteMove;
                break;

            case SpinAction spinAction:
                _button.image.sprite = _spriteSpin;
                break;

            case SwordAction swordAction:
                _button.image.sprite = _spriteSword;
                break;

            case InteractAction interactAction:
                _button.image.sprite = _spriteInteract;
                break;

        }
    }
    // ������� ������������ � AddListener() ����� ���������� �������� ������� ������������� ��� ����
    /*private void MoveActionButton_Click() //
    {

    }*/

    public void UpdateSelectedVisual() // (���������� �������) ��������� � ���������� ������������ ������.(���������� �������� ��� ������ ������ �������� ��������)
    {
        BaseAction selectedBaseAction = UnitActionSystem.Instance.GetSelectedAction(); // ������� �������� ��������
        _selectedButtonVisualUI.SetActive(selectedBaseAction == _baseAction);   // �������� ����� ���� ��������� �������� ��������� � ��������� ������� �� ��������� �� ���� ������
                                                                                // ���� �� ��������� �� ������� false � ����� �����������

        // ����� �������� ���� ������ ��� ��������� ������
        /*if (selectedBaseAction == _baseAction) // ���� ������ �������
        {
            _button.image.color = _color; // ������� ��� ����
        }
        else
        {
            _button.image.color = new Color(_color.r, _color.g, _color.b, 0.5f); // ������� �� ��������������
        }*/
    }


    //3//{ ������ ������ ������ ������ ����� ����� ���������
    private void InteractableEnable() // �������� ��������������
    {
        _button.interactable = true;
        _textMeshPro.color = _textColor;
        UpdateSelectedVisual(); // ������� ����������� ����� ������ � ����������� �� ��������������� ��������
    }

    private void InteractableDesabled() // ��������� �������������� // ������ ����������� �� �������� � ������ ����(������������� � ���������� color  Desabled)
    {
        _button.interactable = false;

        Color textColor = _textColor; // �������� � ��������� ���������� ���� ������
        textColor.a = 0.1f; // ������� �������� ����� ������
        _textMeshPro.color = textColor; // ������� ������� ���� ����� (���� ����������)

        _selectedButtonVisualUI.SetActive(false); //�������� �����
    }

    public void HandleStateButton(bool isBusy) // ���������� ��������� ������
    {
        if (isBusy) // ���� �����
        {
            InteractableDesabled(); // ��������� ��������������
        }
        else
        {
            InteractableEnable(); // �������� ��������������
        }
    }//3//}
}
