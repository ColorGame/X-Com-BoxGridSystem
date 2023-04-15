using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitRagdoll : MonoBehaviour //  Юнит Тряпичная кукла// Висит на Тряпичной кукле . Отвечает за преобразовании куклы из положения Т в нормальное. Откидывание из за взрыва
{
    [SerializeField] private Transform _ragdollRootBone; // Тряпичная кукла Корневая Кость

    private float _explosionForce = 300; // Сила взрыва. У каждого экшн она будет настраиваться в Setup() //НУЖНО НАСТРОИТЬ//

    public void Setup(Transform originalRootBone, Unit keelerUnit) // Настройка тряпичной куклы (в аргумент передаем Оригинальную Корневую Кость(юнита) и Юнита который хочет нас убить - Киллер)
    {
        MatchAllChildrenTransform(originalRootBone, _ragdollRootBone);

        Vector3 explosionPosition; //Иниацилизируем переменную Позиция Взрыва

        BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction(); // Получим Выбранное Действие
                
        switch (selectedAction)
        {
            default:// Этот кейс будет выполняться по умолчанию если нет соответствующих selectedAction
            case GrenadeAction grenadeAction:
                
                Vector3 randomDir = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)); // Для рандомности точки взрыва по Х и Z что бы рагдолл падал по разному
                explosionPosition = transform.position + randomDir;
                _explosionForce = 400f; //НУЖНО НАСТРОИТЬ//
                break;

            case ShootAction shootAction:
                
                Vector3 directionKeeler = (keelerUnit.transform.position - transform.position).normalized; //Направление к киллеру , что бы узнать с какой стороны нанесен урон
                explosionPosition = transform.position + directionKeeler; // Сместим точку взрыва в сторону киллера            
                _explosionForce = 300f; //НУЖНО НАСТРОИТЬ//
                break;

            case SwordAction swordAction:

                directionKeeler = (keelerUnit.transform.position - transform.position).normalized; //Направление к киллеру , что бы узнать с какой стороны нанесен урон
                explosionPosition = transform.position + directionKeeler; // Сместим точку взрыва в сторону киллера    
                _explosionForce = 500f; //НУЖНО НАСТРОИТЬ//
                break;

        }

        ApplyExplosionToRagdoll(_ragdollRootBone, _explosionForce, explosionPosition, 10f); // Применить Взрыв к Тряпичной кукле
        //Вы могли бы изменить функцию Damage, чтобы получить объект Damager, затем, когда урона будет достаточно, чтобы умереть, спросите у этого объекта его положение, чтобы применить силу к тряпичной кукле
        //В качестве альтернативы вы могли бы создать  GrenadeDamageUnits, а после этого создать другую сферу перекрытия и на этот раз искать тряпичных кукол и, если вы найдете какие - либо, применить к ним силу.

    }

    private void MatchAllChildrenTransform(Transform root, Transform clone) // Сопоставьте все дочерние преобразования// Скопируем трансформ корня на трансформ клона с помощью рекурсивной циклической функции
    {
        foreach (Transform child in root) // В цикле переберем дочернии объекты корня(оригинала)
        {
            Transform cloneChild = clone.Find(child.name); // Заходим в клон и ищем в нем дочернии объект с именем как у дочернего объекта оригинала(root) и сохраним в cloneChild(Ребенок клона)
            if (cloneChild != null)
            {
                cloneChild.position = child.position;// Позиция и поворот ребенка клона устанавливаем как у дочернего объекта оригинала(root)
                cloneChild.rotation = child.rotation;

                MatchAllChildrenTransform(child, cloneChild); // Рекурсивная функция (вызывает саму себя) нужна для следующих уровней дочерних объектов, она будет вызывать саму себя пока не доберется до дна
            }
        }
    }

    private void ApplyExplosionToRagdoll(Transform root, float explosionForce, Vector3 explosionPosition, float explosionRange) // Применить Взрыв к Тряпичной кукле (explosionRange Диапазон взрыва)
    {
        foreach (Transform child in root)
        {
            if (child.TryGetComponent<Rigidbody>(out Rigidbody childrigidbody)) // Попробуем получить риджибоди дочерних объектов 
            {
                childrigidbody.AddExplosionForce(explosionForce, explosionPosition, explosionRange);
            }

            ApplyExplosionToRagdoll(child, explosionForce, explosionPosition, explosionRange);  // Рекурсивная функция
        }
    }
}
