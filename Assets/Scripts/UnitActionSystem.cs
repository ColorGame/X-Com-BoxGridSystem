using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;


// ���� ���� ����� � �� ������ ����������� ����� ������. �������� ���, ������� � Project Settings/ Script Execution Order � �������� ���� Deafault Time
public class UnitActionSystem : MonoBehaviour // ������� �������� ����� (��������� ������ �������� �����)
{

    public static UnitActionSystem Instance { get; private set; }   //(������� SINGLETON) ��� �������� ������� ����� ���� ������� (SET-���������) ������ ���� �������, �� ����� ���� �������� GET ����� ������ �������
                                                                    // instance - ���������, � ��� ����� ���� ��������� UnitActionSystem ����� ���� ��� static. Instance ����� ��� ���� ����� ������ ������, ����� ����, ����� ����������� �� Event.

    public event EventHandler OnSelectedUnitChanged; // ��������� ���� ������� (����� ���������� ��������� ���� �� �������� ������� Event)
    public event EventHandler OnSelectedActionChanged; // ��������� �������� �������� (����� �������� �������� �������� � ����� ������ �� �������� ������� Event)
    public event EventHandler<bool> OnBusyChanged; // ��������� �������� (����� �������� �������� _isBusy, �� �������� ������� Event, � �������� �� � ���������) � <> -generic ���� ��� ����� ������ ����������
    public event EventHandler OnActionStarted; // �������� ������ ( �� �������� ������� Event ��� ������ ��������)


    [SerializeField] private Unit _selectedUnit; // ��������� ���� (�� ���������).���� ������� ������������� ����� ������� ����� ���������� ���������� �����
    [SerializeField] private LayerMask _unitLayerMask; // ����� ���� ������ (�������� � ����������) ���� ������� Units

    private BaseAction _selectedAction; // ��������� ��������// ����� ���������� � Button
    private bool _isBusy; // ����� (������� ���������� ��� ���������� ������������� ��������)


    private void Awake() //��� ��������� ������ Awake ����� ������������ ������ ��� ������������� � ���������� ��������
    {
        // ���� �� �������� � ���������� �� �������� �� �����
        if (Instance != null) // ������� �������� ��� ���� ������ ���������� � ��������� ����������
        {
            Debug.LogError("There's more than one UnitActionSystem!(��� ������, ��� ���� UnitActionSystem!) " + transform + " - " + Instance);
            Destroy(gameObject); // ��������� ���� ��������
            return; // �.�. � ��� ��� ���� ��������� UnitActionSystem ��������� ����������, ��� �� �� ��������� ������ ����
        }
        Instance = this;
    }

    private void Start()
    {
        SetSelectedUnit(_selectedUnit); // ���������(����������) ���������� �����, ���������� ��������� ��������, 
                                        // ��� ������ � _selectedUnit ���������� ���� �� ���������
        
        UnitManager.OnAnyUnitDeadAndRemoveList += UnitManager_OnAnyUnitDeadAndRemoveList; //���������� �� ������� ����� ���� ���� � ������ �� ������
    }

    private void UnitManager_OnAnyUnitDeadAndRemoveList(object sender, EventArgs e)
    {
        if (_selectedUnit.IsDead()) // ���� ���������� ���� ������� �� ...
        {
            List<Unit> friendlyUnitList = UnitManager.Instance.GetFriendlyUnitList(); // ������ ������ ������������� ������
            if (friendlyUnitList.Count > 0) // ���� ���� ����� �� �������� ��������� ������� �� ������ �����
            {
                SetSelectedUnit(friendlyUnitList[0]);
            }
            else // ���� ��� ������ � ����� �� �����
            {
                Debug.Log("GAME OVER");
            }
        }
    }   
  

    private void Update()
    {      
        if (_isBusy) // ���� ����� ... �� ���������� ����������
        {
            return;
        }

        if (!TurnSystem.Instance.IsPlayerTurn()) // ��������� ��� ������� ������ ���� ��� �� ���������� ����������
        {
            return;
        }

        if (EventSystem.current.IsPointerOverGameObject())  // ��������, ������� �� ��������� ���� �� �������� ����������������� ����������  
                                                            // ���������� � ����� ������� �������. (current - ���������� ������� ������� �������.) (IsPointerOverGameObject() -��������� ��������� (����) �� ������� ������)
        {
            return; // ���� ��������� ���� ��� �������(UI), ����� ������������� ����� , ��� �� �� ����� �������� �� ������, ���� �� ����� � ����� ����� ������� ���������� ��� �������
        }
        if (TryHandleUnitSelection()) // ������� ��������� ������ �����
        {
            return; //���� �� ������� ����� �� TryHandleUnitSelection() ������ true. ����� ������������� �����, ����� �� ����� �������� �� �����, �������� ����� �������, ���������� ��������� ���� �� ��� � ����� ����� �����
        }

        HandleSelectedAction(); // ���������� ��������� ��������        
    }

    private void HandleSelectedAction() // ���������� ��������� ��������
    {
        if (InputManager.Instance.IsMouseButtonDownThisFrame()) // ��� ������� ��� ������ ���� 
        {
            GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition()); // ����������� ������� ���� �� �������� � ��������.

            if (!_selectedAction.IsValidActionGridPosition(mouseGridPosition)) // ��������� ��� ������ ���������� ��������, �������� ������� ���� �� ������������ �������� . ���� �� ��������� ��...
            {
                return; // ���������� ����������  //���������� ! � return; �������� �������� ������ if()
            }

            if (!_selectedUnit.TrySpendActionPointsToTakeAction(_selectedAction)) // ��� ���������� ����� ��������� ��������� ���� ��������, ����� ��������� ��������� ��������. ���� �� ����� ��...
            {
                return; // ���������� ����������
            }

            SetBusy(); // ���������� �������
            _selectedAction.TakeAction(mouseGridPosition, ClearBusy); //� ���������� �������� ������� ����� "��������� �������� (�����������)" � ��������� � ������� ������� ClearBusy

            OnActionStarted?.Invoke(this, EventArgs.Empty); // "?"- ��������� ��� !=0. Invoke ������� (this-������ �� ������ ������� ��������� ������� "�����������" � ����� UnitActionSystemUI ����� ��� ������������ "������������"

            // ������������� ������ �� ����� �.�. �� ������������� Move � Spin � TakeAction, � �������� ��� � ������� �����. 
            /*switch (_selectedAction) // ������������� ������� �� ����������� ��� _selectedAction // ���� ������ ��������� � ���������� ��������� ����� �������� 
            {
                case MoveAction moveAction:
                    if (moveAction.IsValidActionGridPosition(_mouseGridPosition)) // ��������� �������� ������� ���� �� ������������ �������� . ���� ������ ��...
                    {
                        SetBusy(); // ���������� �������
                        moveAction.Move(_mouseGridPosition, ClearBusy); // ���������� ����������� ����� � _mouseGridPosition (� ��� ����� ������ �����) // ����� � �������� Move ������� �������(������ �� �������)
                    }
                    break;
                case SpinAction spinAction:
                    SetBusy(); // ���������� �������
                    spinAction.Spin(ClearBusy); // � �������� Spin ������ �������(������ �� �������). ������� ��������(���������) ������ ��������� � �������� ������� �� �������� .
                                                // ����� ���������� Spin() ����� � ������������ ����� ������ SpinAction ����������� ����������� ������� ClearBusy()
                    break;
            }*/
        }
    }

    private void SetBusy() // ���������� �������
    {
        _isBusy = true;

        OnBusyChanged?.Invoke(this, _isBusy); // "?"- ��������� ��� !=0. Invoke ������� (this-������ �� ������ ������� ��������� ������� "�����������" � ����� ActionBusyUI ����� ��� ������������ "������������"
    }

    private void ClearBusy() // �������� ��������� ��� ����� ���������
    {
        _isBusy = false;
        OnBusyChanged?.Invoke(this, _isBusy);
    }

    private bool TryHandleUnitSelection() // ������� ��������� ������ �����
    {
        if (InputManager.Instance.IsMouseButtonDownThisFrame()) // ��� ������� ��� ������ ���� 
        {
            Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMouseScreenPosition()); // ��� �� ������ � ����� �� ������ ��� ���������� ������ ����
            if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, _unitLayerMask)) // ������ true ���� �� ���-�� �������. �.�. ������� ����� �������������� �� ����������� ����� ������ �� ������
            {   // �������� ���� �� �� ������� � ������� �� ������ ���������  <Unit>
                if (raycastHit.transform.TryGetComponent<Unit>(out Unit unit)) // ������������ TryGetComponent ����� GetComponent � ��� ��� �� ���� ������ ������� ��������. TryGetComponent - ���������� true, ���� ��������� < > ������. ���������� ��������� ���������� ����, ���� �� ����������.
                {
                    if (unit == _selectedUnit) // ������ �������� ��������� �������� �� ���������� ����� ��� ���������� _selectedAction (�������� ������ ���������� ����� �� �������� ������� ���������� �� ���) ���� ��� ������ ������ �� ������ ���������� _selectedAction �� ������ ����� ������� �����.
                    {
                        // ���� ���� ��� ������
                        return false;
                    }

                    if (unit.IsEnemy()) // ���� ��� ����� � ����� 
                    {
                        // ��� ���� ��� �������� �� ����
                        return false;
                    }
                    SetSelectedUnit(unit); // ������ � ������� ����� ��� ����������� ���������.
                    return true;
                }
            }
        }
        return false; // ���� ������ �� �������
    }

    private void SetSelectedUnit(Unit unit) // ���������(����������) ���������� �����, ���������� ��������� ��������, � ��������� �������   
    {
        _selectedUnit = unit; // �������� ���������� � ���� ����� ����������� ��������� ������.

        SetSelectedAction(unit.GetAction<MoveAction>()); // ������� ��������� "MoveAction"  ������ ���������� ����� (�� ��������� ��� ������ ������� ��������� ����� MoveAction). �������� � ���������� _selectedAction ����� ������� SetSelectedAction()

        /*if (OnSelectedUnitChanged != null)
        {
            OnSelectedUnitChanged(this, EventArgs.Empty); //this-������ �� ������ ������� ��������� ������� (������ �����������). � ��� ��� ��������� ������� �������� ������� Empty
        }*/
        // ���������� ������ ������ ���� ����
        //  ��� ������ ������� �� ����� ����������� � ���, ��� ������� ����� null � ������, ���� ��� ���� �� ��������� ����������(���������).
        //  ������� ��� ������ ������� (event) ����� ��� ������ ��������� �� null.
        OnSelectedUnitChanged?.Invoke(this, EventArgs.Empty); // "?"- ��������� ��� !=0. Invoke ������� (this-������ �� ������ ������� ��������� ������� "�����������" � ����� UnitSelectedVisual � UnitActionSystemUI ����� ��� ������������ "������������" ��� ����� ��� ����� ������ �� _selectedUnit)
    }

    public void SetSelectedAction(BaseAction baseAction) //���������� ��������� ��������, � ��������� �������  
    {
        _selectedAction = baseAction;

        OnSelectedActionChanged?.Invoke(this, EventArgs.Empty); // "?"- ��������� ��� !=0. Invoke ������� (this-������ �� ������ ������� ��������� ������� "�����������" � ����� UnitActionSystemUI  GridSystemVisual ����� ��� ������������ "������������")
    }
    public BaseAction GetSelectedAction() // ������� ��������� ��������
    {
        return _selectedAction;
    }

    public Unit GetSelectedUnit() // ������� ������������� ����� ������� ����� ���������� ���������� ����� (��� �� �� ������ ���������� ��������� _selectedUnit)
    {
        return _selectedUnit;
    }



}
