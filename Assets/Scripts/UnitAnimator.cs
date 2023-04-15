using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimator : MonoBehaviour // Анимация юнита и создание пули (ПЛАНЫ: в дальнейшем можно создать класс оружие Arms и расширять его, тогда в этих классах и будем создавать пули)
{
    [SerializeField] private Animator _animator;
    [SerializeField] private Transform _bulletProjectilePrefab; // в инспекторе закинуть префаб пули
    [SerializeField] private Transform _shootPointTransform; // в инспекторе закинуть точку выстрела лежит на автомате

    // Если сложная система оружий то лучше создать отдельный скрипт который отвечает за смену оружия
    // Менеджер по оружию описание логики https://community.gamedev.tv/t/weapon-manager/213840
    [SerializeField] private Transform _rifleTransform; //в инспекторе закинуть Трансформ Винтовки
    [SerializeField] private Transform _swordTransform; //в инспекторе закинуть Трансформ Мечя

    private void Awake()
    {
        if (TryGetComponent<MoveAction>(out MoveAction moveAction)) // Попробуем получить компонент MoveAction и если получиться сохраним в moveAction
        {
            moveAction.OnStartMoving += MoveAction_OnStartMoving; // Подпишемся на событие
            moveAction.OnStopMoving += MoveAction_OnStopMoving; // Подпишемся на событие
        }

        if (TryGetComponent<ShootAction>(out ShootAction shootAction)) // Попробуем получить компонент ShootAction и если получиться сохраним в shootAction
        {
            shootAction.OnShoot += ShootAction_OnShoot; // Подпишемся на событие
        }
        
        if (TryGetComponent<SwordAction>(out SwordAction swordAction)) // Попробуем получить компонент SwordAction и если получиться сохраним в swordAction
        {
            swordAction.OnSwordActionStarted += SwordAction_OnSwordActionStarted; // Подпишемся на событие
            swordAction.OnSwordActionCompleted += SwordAction_OnSwordActionCompleted;
        }

    }

    private void Start()
    {
        EquipRifle(); // Включим винтовку
    }

    private void SwordAction_OnSwordActionCompleted(object sender, EventArgs e) // Когда действие завершено поменяем меч на винтовку
    {
        EquipRifle(); // Включим винтовку
    }

    private void SwordAction_OnSwordActionStarted(object sender, EventArgs e)
    {
        EquipSword();   // Включим МЕЧ
        _animator.SetTrigger("SwordSlash");// Установить тригер( нажать спусковой крючок)
    }

    private void MoveAction_OnStartMoving(object sender, EventArgs empty)
    {
        _animator.SetBool("IsWalking", true); // Включить анимацию Хотьбыw
    }
    private void MoveAction_OnStopMoving(object sender, EventArgs empty)
    {
        _animator.SetBool("IsWalking", false); // Выключить анимацию Хотьбы
    }

    private void ShootAction_OnShoot(object sender, ShootAction.OnShootEventArgs e) 
    {
        _animator.SetTrigger("Shoot"); // Установить тригер( нажать спусковой крючок)

        Transform bulletProjectilePrefabTransform = Instantiate(_bulletProjectilePrefab, _shootPointTransform.position, Quaternion.identity); // Создадим префаб пули в точке выстрела
        
        BulletProjectile bulletProjectile  = bulletProjectilePrefabTransform.GetComponent<BulletProjectile>(); // Вернем компонент BulletProjectile созданной пули

        Vector3 targetUnitShootAtPosition = e.targetUnit.GetWorldPosition(); // Мировая позиция целевого юнита. (Целевой Юнит Стрельба По Позиции)

        targetUnitShootAtPosition.y = _shootPointTransform.position.y; // В результате ПУЛЯ будет выпущенна горизонтально
        bulletProjectile.Setup(targetUnitShootAtPosition); // В аргумент предали Мировую позицию целевого юнита. с преобразовоной координатой по У
    }

    private void EquipSword() // Экипировка меч
    {
        _swordTransform.gameObject.SetActive(true);
        _rifleTransform.gameObject.SetActive(false);
    }

    private void EquipRifle() // Экипировка вмнтовка
    {
        _swordTransform.gameObject.SetActive(false);
        _rifleTransform.gameObject.SetActive(true);
    }

}
