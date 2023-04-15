using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour // Шкала здоровья будет смотреть в сторону камеры
{
    [SerializeField] private bool _invert; // для 1 МЕТОДА поворота (ставим галочку если надо инвертировать)

    private Transform _cameraTransform;

    private void Awake()
    {
        _cameraTransform = Camera.main.transform; // Кэшируем трансформ камеры (особой необходимости нет но всеже немного сэкономим ресурсов)
    }

    private void LateUpdate() // будем выполнять после всех Update чтобы сначала переместились юниты а потом повернулась UI
    {
        /*// 1 МЕТОД//{ // в данном методе поворот UI игрока зависит от положения на сцене и требует доп проверки
        if (_invert) // Если текст требует инвертирование
        {
            Vector3 directionCamera = (_cameraTransform.position - transform.position).normalized; // Нормализованый вектор направленный в сторону камеры
            transform.LookAt(transform.position + directionCamera*(-1)); // Передадим в аргумент - смотреть относито своей позиции в противоположную сторону вектора directionCamera (он будет как бы смотреть на камеру но жопой)
        }
        else
        {
            transform.LookAt(_cameraTransform);
        }// 1 МЕТОД//}*/

        // 2 ЛУЧШИЙ МЕТОД//{  в данном методе UI будет паралелен рамкам экрана
        transform.rotation = _cameraTransform.rotation;
    }
}
