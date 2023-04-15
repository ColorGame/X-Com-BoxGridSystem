using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private GameObject _actionCameraGameObject; // В инспекторе закинем ActionVirtualCamera и НАСТРОИМ на ней в инспекторе приоритет Priority = 20// На основной камере в разделе Cinemachine настроить время перехода Defauld Blend = 0.2


    [SerializeField] LayerMask _actionCameraCullingMaskNoUI;//Отключить слой "пользовательский интерфейс" на маске экшн-камеры остальные оставить. //НАСТРОИТЬ в сцене объект CANVAS - Render mode - Screen space Camera

    private LayerMask _normalCameraCullingMask;

    private void Start()
    {
        BaseAction.OnAnyActionStart += BaseAction_OnAnyActionStart; // Подпишемся на событие будет выполняться при старте любого действия
        BaseAction.OnAnyActionCompleted += BaseAction_OnAnyActionCompleted; //Подпишемся на событие будет выполняться при завершении любого действия
        LevelScripting.Instance.OnInteractSphereAndDoor += Instance_OnInteractSphereAndDoor; // Подпишемся на событие будет выполняться когда Сфера и Дверь Взаимодействуют 

        _normalCameraCullingMask = Camera.main.cullingMask; // Сохраним нормальные параметры маски рендеринга камеры

        HideActionCamera(); //На всякий случай скроем камеру , если забудем вручную отключить ActionVirtualCamera в сцене 

    }

    private void Instance_OnInteractSphereAndDoor(object sender, DoorInteract e)
    {
        Transform transformDoor = e.gameObject.GetComponent<Transform>(); // Получим Transform двери которую открываем

        Vector3 cameraCharacterHeight = Vector3.up * 10f; // Поднимем камеру на высоту

        float ZAxisOffsetAmount = 5f; // Величина смещения относительно оси Z 
        Vector3 ZAxisOffset = Quaternion.Euler(0, -90, 0) * transformDoor.forward * ZAxisOffsetAmount; // берем направление локальную ось Z двери, поворачиваем ее на -90 градусов по оси Y (влево) и двигаемся ZAxisOffsetAmount в этом направлении, чтобы получить точку, в которой должна находиться камера.)

        Vector3 actionCameraDoorPosition =
                   transformDoor.position +        //позиция двери
                   cameraCharacterHeight +         //поднимем на высоту
                   ZAxisOffset +                    //сместим в влево относительно локальной оси Z 
                   (transformDoor.forward * -10);          //сдвинем камеру назад

        _actionCameraGameObject.transform.position = actionCameraDoorPosition;// Переместим нашу экшен камеру в настроенную позицию
        _actionCameraGameObject.transform.LookAt(transformDoor.position + Vector3.up * 2f); // Посмотрим навысоту 2 в сторону двери 

        ShowActionCamera();
    }

    private void ShowActionCamera() // показать камеру действия
    {
        Camera.main.cullingMask = _actionCameraCullingMaskNoUI; // Настроим Отборочная маска камеры что бы не показывать UI
        _actionCameraGameObject.SetActive(true);
    }

    private void HideActionCamera() // Скрыть камеру действия
    {
        Camera.main.cullingMask = _normalCameraCullingMask; // Вернем стандартные параметры рендеринга слоев маски
        _actionCameraGameObject.SetActive(false);
    }

    private void BaseAction_OnAnyActionStart(object sender, System.EventArgs e)
    {
        // Можно проверить отправителя что бы отсеять лишние события и оставить события отправляемые классом ShootAction
        // if (sender is ShootAction) { }
        // Или использовать переключатель который В ДАЛЬНЕЙШЕМ МОЖНО РАСШИРИТЬ ДРУГОЙ ЛОГИКОЙ
        switch (sender)
        {
            case ShootAction shootAction:
                Unit shooterUnit = shootAction.GetUnit(); // Вернем Стрелка
                Unit targetUnit = shootAction.GetTargetUnit(); // Вернем в кого стреляем

                Vector3 cameraCharacterHeight = Vector3.up * 1.7f; // Поднимем камеру на уровень плеча

                Vector3 shootDirection = (targetUnit.GetWorldPosition() - shooterUnit.GetWorldPosition()).normalized; //Еденичный вектор Направления стрельбы

                float shoulderOffsetAmount = 0.5f; // Величина смещения относительно плеча
                Vector3 shoulderOffset = Quaternion.Euler(0, 90, 0) * shootDirection * shoulderOffsetAmount; // Смещение относительно плеча (Мы берем направление на цель (shootDirection), поворачиваем ее на 90 градусов по оси Y (вправо) и двигаемся shoulderOffsetAmount в этом направлении, чтобы получить точку, в которой должна находиться камера.)

                Vector3 actionCameraPosition =
                    shooterUnit.GetWorldPosition() +    //позиция у основания ног стрелка
                    cameraCharacterHeight +             //поднимем на высоту
                    shoulderOffset +                    //сместим в право относительно плеча 
                    (shootDirection * -1);              //сдвинем камеру назад

                _actionCameraGameObject.transform.position = actionCameraPosition; // Переместим нашу экшен камеру в настроенную позицию
                _actionCameraGameObject.transform.LookAt(targetUnit.GetWorldPosition() + cameraCharacterHeight); // Посмотрим в сторону врага на высоту cameraCharacterHeight (targetUnit.GetWorldPosition() вернет позицию у основания ног)

                ShowActionCamera();
                break;

            case SwordAction swordAction:
                Unit swordUnit = swordAction.GetUnit(); // Вернем Юнита который атакует                
                targetUnit = swordAction.GetTargetUnit(); // Вернем Юнита которого атакуем

                cameraCharacterHeight = Vector3.up * 5f; // Поднимем камеру 

                Vector3 swordDirection = (targetUnit.GetWorldPosition() - swordUnit.GetWorldPosition()).normalized; //Еденичный вектор Направления атаки Мечом

                float shoulderOffsetSwordAmount = 6f; // Величина смещения относительно плеча
                Vector3 shoulderSwordOffset = Quaternion.Euler(0, 90, 0) * swordDirection * shoulderOffsetSwordAmount;

                Vector3 actionCameraSwordPosition =
                    swordUnit.GetWorldPosition() +  //позиция у основания ног стрелка
                    cameraCharacterHeight +         //поднимем на высоту
                    shoulderSwordOffset +         //сместим в право относительно плеча
                    (swordDirection * -5);          //сдвинем камеру назад

                _actionCameraGameObject.transform.position = actionCameraSwordPosition;
                _actionCameraGameObject.transform.LookAt(targetUnit.GetWorldPosition() + Vector3.up * 1.7f); // Посмотрим в сторону врага на высоту плеча (targetUnit.GetWorldPosition() вернет позицию у основания ног)

                ShowActionCamera();
                break;

        }
    }
    private void BaseAction_OnAnyActionCompleted(object sender, System.EventArgs e)
    {
        //Будем использовать переключатель который В ДАЛЬНЕЙШЕМ МОЖНО РАСШИРИТЬ ДРУГОЙ ЛОГИКОЙ
        switch (sender)
        {
            case ShootAction shootAction:
                HideActionCamera();
                break;

            case SwordAction swordAction:
                HideActionCamera();
                break;

            case InteractAction interactAction:
                HideActionCamera();
                break;
        }
    }
}
