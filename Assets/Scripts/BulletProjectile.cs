using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BulletProjectile : MonoBehaviour // —нар€д пули
{
    [SerializeField] private TrailRenderer _trailRenderer; // в инспекторе закинуть трэил пули он лежит в самой пули // у TRAIL незабудь поставить галочку Autodestruct
    [SerializeField] private Transform _bulletHitVfxPrefab; // в инспекторе закинуть систему частиц (искры от попадпни€)

    private Vector3 _targetPosition; // ÷елева€ позици€ пули
    public void Setup(Vector3 targetPosition) // Ќастроика пули
    {
        _targetPosition = targetPosition;
    }

    private void Update()
    {
        Vector3 moveDirection = (_targetPosition - transform.position).normalized; // Ќаправление движени€, еденичный вектор

        //float distanceBeforeMoving = Vector3.Distance(transform.position, _targetPosition); //(рассто€ние до движени€) Ќайдем и сохраним расто€ние до целевого юнита, прежде чем начнем двигатьс€
        //float moveSpead = 200f;
        //transform.position += moveDirection * moveSpead * Time.deltaTime; // переместим пулю
        //float distanceAfterMoving = Vector3.Distance(transform.position, _targetPosition); //(рассто€ние после движени€) Ќайдем и сохраним расто€ние до целевого юнита, после начала движени€

        // ≈сли вам нужно сравнить длины только некоторых векторов, вы можете сравнить их квадратную длину, использу€ sqrMagnitude .
        // ¬ычисление квадратов величин происходит намного быстрее:
        float sqrMagnitudeBeforeMoving = (transform.position - _targetPosition).sqrMagnitude; //(квадрат рассто€ни€ до движени€) Ќайдем и сохраним квадрат расто€ние до целевого юнита, прежде чем начнем двигатьс€
        float moveSpead = 200f;
        transform.position += moveDirection * moveSpead * Time.deltaTime; // переместим пулю
        float sqrMagnitudeAfterMoving = (transform.position - _targetPosition).sqrMagnitude; //(квадрат рассто€ни€ после движени€) Ќайдем и сохраним квадрат расто€ние до целевого юнита, после начала движени€


        if (sqrMagnitudeBeforeMoving < sqrMagnitudeAfterMoving) // if сработает, когда мы перелетим цель, и отдалимс€ от нее, на рассто€ние больше, чем когда мы были до цели. Ѕлиже к цели эти рассто€ни€ становитьс€ мальнекими. ≈сли ввести stoppingDistance как в MoveAction из за большой скорости пул€ будет долго колебатьс€ возле цели пролета€ дистанцию остановки.
        {
            transform.position = _targetPosition; // мы пролетели цель поэтому вернем пулю в целевую позицию. (дл€ визуала)

            _trailRenderer.transform.parent = null; // ќтсоеденим трэйл от родител€ что бы он еще жил. ј в инсепкторе поставим галочку Autodestruct - уничтожение после завершени€ ортрисовки

            Destroy(gameObject);

            Instantiate(_bulletHitVfxPrefab, _targetPosition, Quaternion.identity); // —оздадим префаб частиц (Ќе забудь в инспекторе включить у частиц Stop Action - Destroy)
        }
    }
}
