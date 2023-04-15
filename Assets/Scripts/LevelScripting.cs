using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelScripting : MonoBehaviour // �������. �������� �� �������������� (������ � ������) (����-�������������� � ������) (����-�������������� � �����-��������������)
{
    public static LevelScripting Instance { get; private set; }   //(������� SINGLETON) ��� �������� ������� ����� ���� ������� (SET-���������) ������ ���� �������, �� ����� ���� �������� GET ����� ������ �������
                                                                  // instance - ���������, � ��� ����� ���� ��������� LevelScripting ����� ���� ��� static. Instance ����� ��� ���� ����� ������ ������, ����� ����, ����� ����������� �� Event.

    public event EventHandler<DoorInteract> OnInteractSphereAndDoor; // �������� ������� - ����� � ����� ��������������� //  <> -generic ���� ��� ����� ������ ����������


    // ����� ����� �������������� ��� ���������� ���������� ������(������ ����� ������������ ��� ������������ �� �����).
    // ��������� ����� ����� ����������� �� ����-��������������.
    // ������� ����-�������������� ����� ���������� ��� �������������� � ������
    [Header("HIDER")]
    // ������ ���������� �������
  
    [SerializeField] private List<GameObject> _hider1List; 
    [SerializeField] private List<GameObject> _hider2List;
    [SerializeField] private List<GameObject> _hider3List;
    [SerializeField] private List<GameObject> _hider4List;
    [SerializeField] private List<GameObject> _hider5List;
    [SerializeField] private List<GameObject> _hider6List;
    [SerializeField] private List<GameObject> _hider7List;
    
    [Header("ENEMY LIST")]
    // ������ ������
   
    [SerializeField] private List<GameObject> _enemy1List;
    [SerializeField] private List<GameObject> _enemy2List;
    [SerializeField] private List<GameObject> _enemy3List;
    [SerializeField] private List<GameObject> _enemy4List;
    [SerializeField] private List<GameObject> _enemy5List;
    [SerializeField] private List<GameObject> _enemy6List;
    [SerializeField] private List<GameObject> _enemy7List;

    [Header("DOOR")]
    // �����
    
    [SerializeField] private DoorInteract _door1;
    [SerializeField] private DoorInteract _door2;
    [SerializeField] private DoorInteract _door3;
    [SerializeField] private DoorInteract _door4;
    [SerializeField] private DoorInteract _doorSphere5; // ����� ����������� � ������� �����
    [SerializeField] private DoorInteract _doorSphere6;
    [SerializeField] private DoorInteract _doorSphere7;

    [Header("SPHERE")]
    // ����� �������������� ����� ��������� ������������ �����
    [SerializeField] private SphereInteract _sphere5; 
    [SerializeField] private SphereInteract _sphere6;
    [SerializeField] private SphereInteract _sphere7;

    [Header("BARREL")]
    // ����� �������������� ����� �������� ���� ����� ��������������
    [SerializeField] private BarrelInteract _barrelSphere7;


   /* private List<List<GameObject>> _hiderListList;
    private List<List<GameObject>> _enemyListList;
    private List<DoorInteract> _doorInteractList;*/
    //private bool _hasShownFirstHider = false; // ����� �������� ������ ������� ����� ������


    private void Awake() //��� ��������� ������ Awake ����� ������������ ������ ��� ������������� � ���������� ��������
    {
        // ���� �� �������� � ���������� �� �������� �� �����
        if (Instance != null) // ������� �������� ��� ���� ������ ���������� � ��������� ����������
        {
            Debug.LogError("There's more than one LevelScripting!(��� ������, ��� ���� LevelScripting!) " + transform + " - " + Instance);
            Destroy(gameObject); // ��������� ���� ��������
            return; // �.�. � ��� ��� ���� ��������� LevelScripting ��������� ����������, ��� �� �� ��������� ������ ����
        }
        Instance = this;

        // �������� ������� ������
        SetActiveGameObjectList(_enemy1List, false);
        SetActiveGameObjectList(_enemy2List, false);
        SetActiveGameObjectList(_enemy3List, false);
        SetActiveGameObjectList(_enemy4List, false);
        SetActiveGameObjectList(_enemy5List, false);
        SetActiveGameObjectList(_enemy6List, false);
        SetActiveGameObjectList(_enemy7List, false);
    }


    private void Start()
    {
        //LevelGrid.Instance.OnAnyUnitMovedGridPosition += LevelGrid_OnAnyUnitMovedGridPosition; // ���������� �� ������� ����� ���� ��������� � �������� �������
        
        // �������� �������������� � ������� ������� ����������� �� �����
        _doorSphere5.SetIsInteractable(false);
        _doorSphere6.SetIsInteractable(false);
        _doorSphere7.SetIsInteractable(false);
                

        _door1.OnDoorOpened += (object sender, EventArgs e) => // ���������� �� ������� ����� �������. ������� ����� ��������� �������� ����� ������ () => {...} 
        {   
            // ����������� �����������
            /*DoorInteract doorInteract = (DoorInteract)sender;
            int inexList =  _doorInteractList.IndexOf(doorInteract); // ������� ������ �������� �����
            SetActiveGameObjectList(_hiderListList[inexList], false);
            SetActiveGameObjectList(_enemyListList[inexList], true);*/

            SetActiveGameObjectList(_hider1List, false); // �������� ���������� ������ ���������� �������
            SetActiveGameObjectList(_enemy1List, true);  // ������� ���������� ������ ������
        };

        _door2.OnDoorOpened += (object sender, EventArgs e) =>
        {
            SetActiveGameObjectList(_hider2List, false);
            SetActiveGameObjectList(_enemy2List, true);
        };
        
        _door3.OnDoorOpened += (object sender, EventArgs e) =>
        {
            SetActiveGameObjectList(_hider3List, false);
            SetActiveGameObjectList(_enemy3List, true);
        };

        _door4.OnDoorOpened += (object sender, EventArgs e) =>
        {
            SetActiveGameObjectList(_hider4List, false);
            SetActiveGameObjectList(_enemy4List, true);
        };

        _doorSphere5.OnDoorOpened += (object sender, EventArgs e) =>
        {
            SetActiveGameObjectList(_hider5List, false);
            SetActiveGameObjectList(_enemy5List, true);
        };

        _doorSphere6.OnDoorOpened += (object sender, EventArgs e) =>
        {
            SetActiveGameObjectList(_hider6List, false);
            SetActiveGameObjectList(_enemy6List, true);
        };

        _doorSphere7.OnDoorOpened += (object sender, EventArgs e) =>
        {
            SetActiveGameObjectList(_hider7List, false);
            SetActiveGameObjectList(_enemy7List, true);
        };



        _barrelSphere7.OnBarrelInteractlActivated += (object sender, EventArgs e) =>// ���������� �� ������� ����� �������������.   
        {
            _sphere7.gameObject.SetActive(true); // ��� ��������� ����� ������� ����� �������������� ��������
            _sphere7.UpdateInteractableAtGridPosition(); // �������� � �����, �������������� � �������� ��������
        };



        _sphere5.OnInteractSphereActivated += (object sender, EventArgs e) => // ���������� �� ������� ����� �������������.
        {
            InteractSphereAndDoor(_doorSphere5);      
        };

        _sphere6.OnInteractSphereActivated += (object sender, EventArgs e) => // ���������� �� ������� ����� �������������.
        {
            InteractSphereAndDoor(_doorSphere6);
        };

        _sphere7.OnInteractSphereActivated += (object sender, EventArgs e) => // ���������� �� ������� ����� �������������.
        {
            InteractSphereAndDoor(_doorSphere7);
        };
    }


    private void SetActiveGameObjectList(List<GameObject> gameObjectList, bool isActive) // ���������� �������� ��� �� �������� ������ �������� (� ����������� �� ���������� ��� ������� ����������) 
    {
        foreach (GameObject gameObject in gameObjectList)
        {
            gameObject.SetActive(isActive);
        }
    }

    private void InteractSphereAndDoor(DoorInteract doorInteract) // ��������������� ����� � �����
    {
        doorInteract.SetIsInteractable(true); // ������� �������������� � ������
        doorInteract.UpdateStateDoor(true); //������� ��������� ����� � ����������� �� ���������� ������� ����������
        OnInteractSphereAndDoor?.Invoke(this, doorInteract); // �������� ������� ����� � ����� ��������������� (��������� CameraManager ��� ������������ ������ �� �����) // � �������� ��������� ����� � ������� ���������� ��������������
    }



    

   /* private void LevelGrid_OnAnyUnitMovedGridPosition(object sender, LevelGrid.OnAnyUnitMovedGridPositionEventArgs e)
    {
        if (e.toGridPosition.z == 5 && !_hasShownFirstHider) // ���� ����� ������� ����� 5 ������(�����) � ��� �� ���� �������� ������ ����� ��...
        {
            _hasShownFirstHider = true; // ����� �������� ������ ������� ����� ������
            SetActiveGameObjectList(_hider1List, false); // �������� ������� ������� ��������
            SetActiveGameObjectList(_enemy1List, true); // ������� 1 ���� ������
        }
    }*/
}
