using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionBusyUI : MonoBehaviour // Действие занят. (будет обрабатывать логику вкл и выкл эмблемы "Busy", которая закрывает кнопки действий )
{
    //Заменил на 3 способ реализую в ActionButtonUI и UnitActionSystemUI
    /*private void Start()
    {
        UnitActionSystem.Instance.OnBusyChanged += UnitActionSystem_OnBusyChanged; // Подписываюсь на Event и выполним UnitActionSystem_OnBusyChanged, эта фунуция получит от события булевый аргумент

        gameObject.SetActive(false); // Скроем при старте
    }

    private void UnitActionSystem_OnBusyChanged(object sender, bool isBusy) // СОКРАЩЕННАЯ ЗАПИСЬ КОДА НИЖЕ
    {
        gameObject.SetActive(isBusy); //Если занять активировать(показать) если нет скрыть
    }*/

    
    
    
    
    /*private void Show() // Показать
    {
        gameObject.SetActive(true);
    }

    private void Hide() // Скрыть
    {
        gameObject.SetActive(false);
    }

    private void UnitActionSystem_OnBusyChanged(object sender, bool isBusy)
    {
        if (isBusy)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }*/        
}
