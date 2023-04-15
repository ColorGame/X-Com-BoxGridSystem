using Cinemachine; // добавим для работы с Cinemachine
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ПРЕДВАРИТЕЛЬНАЯ НАСТРОИКА - у CinemachineVirtualCamera во вкладке Add Extension добавили Cinemachine Impulse Listener //https://docs.unity3d.com/Packages/com.unity.cinemachine@2.9/manual/CinemachineImpulseListener.html
// Secondary Noise  -6DShake    Вторичный шум
// Amplituda Gain   -10         Коэффициент усиления по амплитуде
// Frequency Gain   -5          Усиление частоты
// Duration         -0.5        Продолжительность

public class ScreenShake : MonoBehaviour //Дрожание Экрана //Генерирует настраевымый импульс который будет слушать CinemachineVirtualCamera
{
    public static ScreenShake Instance { get; private set; }   //(ПАТТЕРН SINGLETON) Это свойство которое может быть заданно (SET-присвоено) только этим классом, но может быть прочитан GET любым другим классом
                                                               // instance - экземпляр, У нас будет один экземпляр ScreenShake можно сдел его static. Instance нужен для того чтобы другие методы, через него, могли подписаться на Event.


    private CinemachineImpulseSource _cinemachineImpulseSource; // Источник кинематографических импульсов

    private void Awake()
    {
        // Если ты акуратем в инспекторе то проверка не нужна
        if (Instance != null) // Сделаем проверку что этот объект существует в еденичном екземпляре
        {
            Debug.LogError("There's more than one ScreenShake!(Там больше, чем один ScreenShake!) " + transform + " - " + Instance);
            Destroy(gameObject); // Уничтожим этот дубликат
            return; // т.к. у нас уже есть экземпляр ScreenShake прекратим выполнение, что бы не выполнить строку ниже
        }
        Instance = this;

        _cinemachineImpulseSource = GetComponent<CinemachineImpulseSource>();
    }     

    public void Shake(float intensity = 1f) // Дрожание. В аргумент предадим величину интенсивности и по умолчанию установим ее 1
    {
        _cinemachineImpulseSource.GenerateImpulse(intensity); //Вызовим функцию Генерировать импульс
    }


    //ДЛЯ ТЕСТА И НАСТРОЙКИ СИЛЫ ТРЯСКИ
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            _cinemachineImpulseSource.GenerateImpulse(5);
        }
    }
}
