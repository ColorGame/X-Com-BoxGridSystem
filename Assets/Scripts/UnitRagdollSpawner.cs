using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitRagdollSpawner : MonoBehaviour // Юнит Тряпичная кукла Зарождение// Висит на юните
{
    [SerializeField] private Transform _ragdollPrefab; // Префаб тряпичной куклы
    [SerializeField] private Transform _originalRootBone; // Оригинальная Корневая Кость(юнита) // В инспекторе закинуть кость юнита под названием Root 

    private HealthSystem _healthSystem;

    private Unit _keelerUnit; //Сохраним юнита который хочет нас убить Киллер.
    private Unit _unit; // Юнит на котором лежит этот скрипт

    private void Awake()
    {
        _healthSystem = GetComponent<HealthSystem>();       
        _unit= GetComponent<Unit>();
    }

    private void Start()
    {
        _healthSystem.OnDead += HealthSystem_OnDead; // подпишемся на событие Умер (запустим событие когда юнит умер)

        ShootAction.OnAnyShoot += ShootAction_OnAnyShoot; // Подпишемся на событие Любой Начал стрелять (на OnShoot не могу подписаться т.к. он не static и нужна ссылка на игровой объект)
        SwordAction.OnAnySwordHit += SwordAction_OnAnySwordHit; // Подпишемся на cобытие - Любой Начал удар мечом
    }

    private void SwordAction_OnAnySwordHit(object sender, SwordAction.OnSwordEventArgs e)
    {
        if (e.targetUnit == _unit) // Если целью является этот юнит , то сохраним того кто по нам стрелял
        {
            _keelerUnit = e.hittingUnit;
        }
    }

    private void ShootAction_OnAnyShoot(object sender, ShootAction.OnShootEventArgs e)
    {
        if(e.targetUnit== _unit) // Если целью является этот юнит , то сохраним того кто по нам стрелял
        {
            _keelerUnit = e.shootingUnit;
        }        
    }
        

    private void HealthSystem_OnDead(object sender, System.EventArgs e)
    {
        Transform ragdollTransform = Instantiate(_ragdollPrefab, transform.position, transform.rotation); // Создадим куклу из префаба в позиции юнита

        UnitRagdoll unitRagdoll = ragdollTransform.GetComponent<UnitRagdoll>(); // Найдем компонент UnitRagdoll на префабе
               
        unitRagdoll.Setup(_originalRootBone, _keelerUnit); // и передадим в метод Setup трансформ Оригинальнай Корневой Кости и СТРЕЛКА
    }
}
