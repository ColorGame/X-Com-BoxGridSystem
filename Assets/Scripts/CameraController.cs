using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour // В дальнейшем можно сделать рефакторинг и добавить InputSystem
{

    private const float MIN_FOLLOW_Y_OFFSET = 2f;
    private const float MAX_FOLLOW_Y_OFFSET = 12f;

    [SerializeField] private CinemachineVirtualCamera _cinemachineVirtualCamera;

    private CinemachineTransposer _cinemachineTransposer;
    private Vector3 _targetFollowOffset; // Целевое Смещение следования


    private void Start()
    {
        _cinemachineTransposer = _cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>(); // Получим и сохраним компонент CinemachineTransposer из виртуальной камеры, чтобы в дальнейшем изменять ее параметры для ZOOM камеры
        _targetFollowOffset = _cinemachineTransposer.m_FollowOffset; // Смещение следования
    }
    
    private void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleZoom();
    }

    private void HandleMovement() // Ручное движение
    {
        Vector2 inputMoveDirection = InputManager.Instance.GetCameraMoveVector(); // Направление вводимого движенияи (обнуляем перед каждой трансформащией)
        
        float moveSpeed = 10f; // Скорость камеры

        //Чтобы Движение учитывало вращение преобразуем вектор inputMoveDirection в moveVector
        Vector3 moveVector = transform.forward * inputMoveDirection.y + transform.right * inputMoveDirection.x; // Применим локальное смещение. Локальным вектор forward(z) изменим на inputMoveDirection.y, а Локальным вектор right(x) изменим на inputMoveDirection.x
        transform.position += moveVector * moveSpeed * Time.deltaTime;
    }

    private void HandleRotation() // Ручной поворот
    {
        Vector3 rotationVector = new Vector3(0, 0, 0); // Вектор вращения // Будем вращать только вокруг оси Y (обнуляем перед каждой трансформащией)

        rotationVector.y = InputManager.Instance.GetCameraRotateAmount(); //Получить величину поворота камеры по ост У
        
        float rotationSpeed = 100f;
        transform.eulerAngles += rotationVector * rotationSpeed * Time.deltaTime;
        //Еще один способ
        //transform .Rotate(rotationVector, rotationSpeed * Time.deltaTime);
    }

    private void HandleZoom() // Ручное масштабирование
    {
        //Debug.Log(InputManager.Instance.GetCameraZoomAmount()); // Отладка чтобы вивдить вводимые данные

        float zoomIncreaseAmount = 1f; //Масштаб величины увеличение (скорость увеличения)

        _targetFollowOffset.y += InputManager.Instance.GetCameraZoomAmount() * zoomIncreaseAmount; // Получить величину увеличения камеры

        // Мы не используем Time.deltaTime т.к. фиксируем лиш изминение величины прокрутки колесика и не учитываем ее величину (тоже самое что и фиксировать лиш нажатие клавиши например Input.GetKeyDown)
       
        _targetFollowOffset.y = Mathf.Clamp(_targetFollowOffset.y, MIN_FOLLOW_Y_OFFSET, MAX_FOLLOW_Y_OFFSET);// ограничим значения масштабирования
        float zoomSpeed = 5f;
        _cinemachineTransposer.m_FollowOffset = Vector3.Lerp(_cinemachineTransposer.m_FollowOffset, _targetFollowOffset, Time.deltaTime * zoomSpeed); // Загружаем наши измененые значения, Для плавности используем Lerp
    }

}
