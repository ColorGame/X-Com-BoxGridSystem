using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystemVisualSingle : MonoBehaviour //Сеточная система визуализации еденицы сетки (ячейки) // Лежит на самом префабе (белая рамка)
{
    [SerializeField] private MeshRenderer _meshRenderer; // Будем включать и выкл. MeshRenderer что бы скрыть или показать наш визуальный объект

    public void Show(Material material) // Показать
    {
        _meshRenderer.enabled = true;
        _meshRenderer.material = material; // Установим переданный нам материал
    }

    public void Hide() // Скрыть
    {
        _meshRenderer.enabled = false;
    }

}
