using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class DestructibleCrate : MonoBehaviour // Разрушаемый ящик
{
    public static event EventHandler OnAnyDestroyed; // static - обозначает что event будет существовать для всего класса, а не для оттдельного ящика. Поэтому для прослушивания этого события слушателю не нужна ссылка на конкретный объект, они могут получить доступ к событию через класс, который затем запускает одно и то же событие для каждого объекта. 
                                                     // Мы запустим событие Event ЛЮБЙ(Any) объект разрушен.
   
    [SerializeField] private Transform _crateDestroyedPrefab; // Префаб разрушенного ящика 

    private GridPosition _gridPosition; // Позиция сетки нашего ящика


    private void Start()
    {
        _gridPosition = LevelGrid.Instance.GetGridPosition(transform.position); //Получим сеточную позицию ящика
    }
    public void Damage()
    {
       Transform crateDestroyedTransform = Instantiate(_crateDestroyedPrefab, transform.position, transform.rotation); // Прежде чем уничтожить ящик создадим на его месте разрушенный

        ApplyExplosionToChildren(crateDestroyedTransform, 150f, transform.position, 10f); // Применим взрыв к разрушенному ящику, с силой 150, в той же позиции, и радиус действия 10

        Destroy(gameObject); //Уничтожим коробку

        OnAnyDestroyed?.Invoke(this, EventArgs.Empty);
    }

    public GridPosition GetGridPosition()
    {
        return _gridPosition;
    }

    private void ApplyExplosionToChildren(Transform root, float explosionForce, Vector3 explosionPosition, float explosionRange) // Применить Взрыв к Детям (explosionRange Диапазон взрыва)
    {
        foreach (Transform child in root)
        {
            if (child.TryGetComponent<Rigidbody>(out Rigidbody childrigidbody)) // Попробуем получить риджибоди дочерних объектов 
            {
                childrigidbody.AddExplosionForce(explosionForce, explosionPosition, explosionRange);
            }

            ApplyExplosionToChildren(child, explosionForce, explosionPosition, explosionRange);  // Рекурсивная функция
        }
    }
}
