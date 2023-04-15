using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingUpdater : MonoBehaviour //Обновление поиска пути
{
    private void Start()
    {
        DestructibleCrate.OnAnyDestroyed += DestructibleCrate_OnAnyDestroyed; //Подпишемся на событие ЛЮБЙ(Any) объект разрушен.
    }

    private void DestructibleCrate_OnAnyDestroyed(object sender, System.EventArgs e)
    {
        DestructibleCrate destructibleCrate = sender as DestructibleCrate; // Сохраним разрушаемы ящик (это наш отправитель сигнала))

        Pathfinding.Instance.SetIsWalkableGridPosition(destructibleCrate.GetGridPosition(), true); // Сеточную позицию где взорвался ящик сделаем пригодной для хотьбы
    }
}
