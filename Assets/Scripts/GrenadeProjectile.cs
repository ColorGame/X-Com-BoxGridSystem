using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

// � ������� ������� � TRAIL �������� ��������� ������� Autodestruct
public class GrenadeProjectile : MonoBehaviour // ��������� ������
{

    public static event EventHandler OnAnyGrenadeExploded; // static - ���������� ��� event ����� ������������ ��� ����� ������ �� �������� �� ���� ������� � ��� �������� ������. ������� ��� ������������� ����� ������� ��������� �� ����� ������ �� �����-���� ���������� �������, ��� ����� �������� ������ � ������� ����� �����, ������� ����� ��������� ���� � �� �� ������� ��� ������ �������. 
                                                           // �� �������� ������� Event ����� ����� ������� ����������

    [SerializeField, Min(0.1f)] private float _moveSpeed = 15f; // �������� ����������� 
    [SerializeField, Min(0)] private int _damageAmount = 30; // �������� �����
    [SerializeField, Min(0)] private int _damageRadiusInCells = 1; // ������ ����������� � ������� ����� (������������� �� ������, ���� ����� ��� �� ����� ��������������� �� ���� ������ �� ����������� �� ������ ������ = 1,5 (0,5 ��� �������� ����������� ������ halfCentralCell - ����� ���������� ��������) (���� ����� �������������� ����� �� 2 ������ �� ������ ������ �� ������ = 2,5 ������. ��� 3 ����� ������ 3,5)
    [SerializeField] private AnimationCurve _damageMultiplierAnimationCurve; //������������ ������ ��������� �����������

    [SerializeField] private Transform _grenadeExplosionVfxPrefab; // � ���������� �������� ������� ������ (����� �� �������) //�������� ��������� ������� � TRAIL ���������������(Destroy) ����� ������������
    [SerializeField] private TrailRenderer _trailRenderer; // � ���������� �������� ����� ������� �� ����� � ����� ���� // � TRAIL �������� ��������� ������� Autodestruct
    [SerializeField] private AnimationCurve _arcYAnimationCurve; // ������������ ������ ��� ��������� ���� ������ �������

    private Vector3 _targetPosition;//������� ����
    private Vector3 _moveDirection; //������ ����������� �������� �������. ��� ����������� �������� ���� ��� �.�. ��� �� �������� � ����� ������������ � Update()
    private float _totalDistance;   //��� ���������. ��������� �� ���� (����� �������� � �����). ��� ����������� �������� ���� ���, � � Update() ��� ���������� �������� ��������� �� ���� ����� �������� �� _totalDistance ��������� �� ���� ��� moveStep (Vector3.Distance-��������� �����)
    private float _currentDistance; //������� ���������� �� ����
    private Vector3 _positionXZ;    //���������� ������� ������ ������� �� ��� X (Y-����� ������ ������������ ������)


    private Action _onGrenadeBehaviorComplete;  //(�� ������� �������� ���������)// �������� ������� � ������������ ���� - using System;
                                                //�������� ��� ������� ��� ������������ ���������� (� ��� ����� ��������� ������ ������� �� ���������).
                                                //Action- ���������� �������. ���� ��� �������. ������� Func<>. 
                                                //�������� ��������. ����� ���������� ������� � ������� �� �������� ��������, ����� �����, � ������������ ����� ����, ����������� ����������� �������.
                                                //�������� ��������. ����� �������� �������� ������� �� ������� ������

    private void Update()
    {
        float moveStep = _moveSpeed * Time.deltaTime; // ��� ����������� �� ����

        _positionXZ += _moveDirection * moveStep; // ���������� ������ �� ��� � �� ���� ���

        _currentDistance -= moveStep; // ������� ��������� �� ����. �� ����������� ��������� ������ ���� ����� �������� ��������� ���
        float currentDistanceNormalized = 1 - _currentDistance / _totalDistance;//����� ������� AnimationCurve (�������������� ���) ������� �� ��������������� ��������� �� ���� � ������� ��������(�� 1 ������� ���������� ��������). _currentDistance<=_totalDistance ������� �������� ����� �� 0 �� 1.
                                                                                //� ������ ������ ������� _currentDistance = _totalDistance, ����� currentDistanceNormalized = 1(��� �������� ��� ������� � ������������ ������), ��� 1 ��� �������� �������� positionY ����� ������� � ��� ����� � ������ �������� � ������ ������� 0 ������� ������� ��������)
        float maxHeight = _totalDistance / 4f;// ������ ������ ������� ������� ��������� �� ��������� ������ (��� �� ��� �������� ������� ����� �������� �����������) //����� ���������//
        float positionY = _arcYAnimationCurve.Evaluate(currentDistanceNormalized) * maxHeight; // ������� ������� � �� ������������ ������  � ������� �� ���� ������ ������

        transform.position = new Vector3(_positionXZ.x, positionY, _positionXZ.z); //���������� ������� � ������ ������������ ������

        float reachedTargetDistance = 0.2f; // ����������� ������� ����������
        if (_currentDistance < reachedTargetDistance) // ���� ������� ���������� ������ �� �������� ��
        {
            float halfCentralCell = 0.5f; // �������� ����������� ������
            float damageRadius = (_damageRadiusInCells + halfCentralCell) * LevelGrid.Instance.GetCellSize(); // ������ ����������� �� ������� = ������ ����������� � ������� �����(� ������ ����������� ������) * ������ ������
            Collider[] colliderArray = Physics.OverlapSphere(_targetPosition, damageRadius); //� ���� ������ - �������� � �������� ������ �� ����� ������������, ���������������� �� ������ ��� ����������� ������ ���.

            foreach (Collider collider in colliderArray)  // ��������� ������ ����������
            {
                if (collider.TryGetComponent<Unit>(out Unit targetUnit))//� ������� � �������� ���������� collider ��������� �������� ��������� Unit // ���� �� ����������� �������� ����� "out", �� ������� ������ ���������� �������� ��� ���� ����������
                                                                        // TryGetComponent - ���������� true, ���� ���������< > ������.���������� ��������� ���������� ����, ���� �� ����������.
                {
                    /*//1// ������ ���� �� ������� �� ����������
                    targetUnit.Damage(_damageAmount);
                    //1//*/

                    //2// ������ ���� ������� �� ����������
                    float distanceToUnit = Vector3.Distance(targetUnit.GetWorldPosition(), _targetPosition); // ��������� �� ������ ������ �� ����� ������� ����� � ������ ������
                    float distanceToUnitNormalized = distanceToUnit / damageRadius; // ����� ������� AnimationCurve (�������������� ���) ������� �� ��������������� ��������� �� ����� (distanceToUnit<=damageRadius ������� �������� ����� �� 0 �� 1. ���� ���� ���������� ������ ������ �� distanceToUnit =0 ����� distanceToUnitNormalized ���� = 0, ����� ������������ ������ ������ �������� ������������ ��� � ������� ������ ������� ��� �������� ����� =1)
                    int damageAmountFromDistance = Mathf.RoundToInt(_damageAmount * _damageMultiplierAnimationCurve.Evaluate(distanceToUnitNormalized)); //�������� ����������� �� ���������. �������� �� ������ � ��������� � int �.�. Damage() ��������� ����� �����

                    targetUnit.Damage(damageAmountFromDistance); // �������� ���� � ����� ��������� � ������ ������
                    //2//
                }

                if (collider.TryGetComponent<DestructibleCrate>(out DestructibleCrate destructibleCrate))   //� ������� � �������� ���������� collider ��������� �������� ��������� DestructibleCrate // ���� �� ����������� �������� ����� "out", �� ������� ������ ���������� �������� ��� ���� ����������
                                                                                                            // TryGetComponent - ���������� true, ���� ���������< > ������.���������� ��������� ���������� ����, ���� �� ����������.
                {
                    destructibleCrate.Damage(); // ���� ���� ���� �������� ��� // ����� ����� ����������� ��������� ���������� ��� �� ������� ����� ��������� ��� ������� ������� ��������� ���� ���������
                }

            }

            OnAnyGrenadeExploded?.Invoke(this, EventArgs.Empty);// ������� �������

            _trailRenderer.transform.parent = null; // ���������� ����� �� �������� ��� �� �� ��� ���. � � ���������� �������� ������� Autodestruct - ����������� ����� ���������� ����������

            Instantiate(_grenadeExplosionVfxPrefab, _targetPosition, Quaternion.LookRotation(Vector3.up)); //�������� ��������� ������ . ��������� ��� �� ��� Z �������� ����� �.�. � ��� ������� ������ ��� ���������

            Destroy(gameObject);

            _onGrenadeBehaviorComplete(); // ������� ����������� ������� ������� ��� �������� ������� Setup(). � ����� ������ ��� ActionComplete() �� ������� ��������� � ������ UI
        }
    }

    public void Setup(GridPosition targetGridPosition, Action onGrenadeBehaviorComplete) // ��������� �������. � �������� �������� ������� �������  � �������� ����� ���������� ������� ���� Action (onGrenadeBehaviorComplete - �� ������� �������� ���������)
    {
        _onGrenadeBehaviorComplete = onGrenadeBehaviorComplete; // �������� ��������� �������

        _targetPosition = LevelGrid.Instance.GetWorldPosition(targetGridPosition); // ������� ������� ������� �� ���������� ��� ������� �����

        // ��������������� ���������� ��� ����������� 

        _positionXZ = transform.position; // �������� ������� ������� �� ��� � ��� ���� ������� � ������������
        _positionXZ.y = 0;

        _totalDistance = Vector3.Distance(_positionXZ, _targetPosition);  //�������� ��������� ����� �������� � ����� (����� �� ��������� ������ ���� � update)
        _currentDistance = _totalDistance; // ������� ���������� � ������ ����� ����� ����������

        _moveDirection = (_targetPosition - _positionXZ).normalized; //�������� ������ ����������� �������� ������� (����� �� ��������� ������ ���� � update �.�. ��� �� ��������)
    }
    public int GetDamageRadiusInCells() //�������� _damageRadiusInCells
    {
        return _damageRadiusInCells;
    }

    /*#if UNITY_EDITOR //��������� �� ��������� ����������. ��������� ��������� ��� ������ �� ����� ��� ���������� � ���������� ����� ���� ������������� ��� ����� �� �������������� ��������.
        private void OnDrawGizmos() // ��� ��������� ������������� �������� � �����, � ����� ������ ���� �������� �������
        {
            Handles.color = Color.red;
            Handles.DrawWireDisc(_targetPosition, Vector3.up , _damageRadiusInCells * LevelGrid.Instance.GetCellSize(), 4f);
        }
    #endif // ��� �������� ����� ���� ����� ���� �� ����� � ���� ���������� � ����� �������� ������ � EDITOR(��������)*/

}
