using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsLocked : MonoBehaviour // При взаимодействии с дверью, если ее нельзя открыть, появить надпись "Заперта"
{
    
    private void Start()
    {
        DoorInteract.OnAnyDoorIsLocked += DoorInteract_OnAnyDoorIsLocked; //Подпишемся на событие Любая Дверь Заперта(нельзя открыть вручную)
        
        UnitActionSystem.Instance.OnBusyChanged += UnitActionSystem_OnBusyChanged; // Подписываюсь на Event Занятость Изменена  и выполним UnitActionSystem_OnBusyChanged, эта фунуция получит от события булевый аргумент


        Hide(); // Скроем при старте
    }

    private void DoorInteract_OnAnyDoorIsLocked(object sender, System.EventArgs e)
    {
        Show(); 
    }
    
    private void UnitActionSystem_OnBusyChanged(object sender, bool isBusy)
    {
        if (!isBusy) // Когда игрок освобождается то скроем надпись
        {
            Hide();
        }
       
    }

    private void Show() // Показать
    {
        gameObject.SetActive(true);
    }

    private void Hide() // Скрыть
    {
        gameObject.SetActive(false);
    }
}
