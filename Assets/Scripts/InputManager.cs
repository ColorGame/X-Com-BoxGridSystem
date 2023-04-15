#define USE_NEW_INPUT_SYSTEM // В C# определен ряд директив препроцессора, оказывающих влияние на интерпретацию исходного кода программы компилятором. 
//Эти директивы определяют порядок интерпретации текста программы перед ее трансляцией в объектный код в том исходном файле, где они появляются. 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour // Менеджер ввода // Все манипуляции внешних девайсов(джойстик, мышь, клавиатура...) должны проходить через этот класс
{

    public static InputManager Instance { get; private set; }   //(ПАТТЕРН SINGLETON) Это свойство которое может быть заданно (SET-присвоено) только этим классом, но может быть прочитан GET любым другим классом
                                                                // instance - экземпляр, У нас будет один экземпляр InputManager можно сдел его static. Instance нужен для того чтобы другие методы, через него, могли подписаться на Event.

    private PlayerInputActions _playerInpytActions; // Объявим - Действия, вводимые игроком (NEW INPUT SYSTEM)

    private void Awake()
    {
        // Если ты акуратем в инспекторе то проверка не нужна
        if (Instance != null) // Сделаем проверку что этот объект существует в еденичном екземпляре
        {
            Debug.LogError("There's more than one InputManager!(Там больше, чем один InputManager!) " + transform + " - " + Instance);
            Destroy(gameObject); // Уничтожим этот дубликат
            return; // т.к. у нас уже есть экземпляр InputManager прекратим выполнение, что бы не выполнить строку ниже
        }
        Instance = this;

        _playerInpytActions =  new PlayerInputActions(); //Создадим и сохраним новый экземпляр Ввода 
        _playerInpytActions.Player.Enable(); // Откроем карту действий (мы назвали ее Player в InputActions) и включим ее, что бы использовать этот актив
    }


    public Vector2 GetMouseScreenPosition() // Вернуть положение мыши на экране
    {
#if USE_NEW_INPUT_SYSTEM //Если используем Новую Систему Ввода то компилировать код ниже (если закоментировать #define USE_NEW_INPUT_SYSTEM то код ниже не будет компилироваться)
        return Mouse.current.position.ReadValue(); 
#else //в противном случае компилировать 
        return Input.mousePosition;
#endif
    }

    public bool IsMouseButtonDownThisFrame() // Нажата кнопка мыши в этот кадр
    {
#if USE_NEW_INPUT_SYSTEM //Если используем Новую Систему Ввода то компилировать код ниже (если закоментировать #define USE_NEW_INPUT_SYSTEM то код ниже не будет компилироваться)
        return _playerInpytActions.Player.Click.WasPressedThisFrame(); //Был Нажат Этот Кадр возвращает true если на этом кадре была нажата левая кнопка мыши
#else
        return Input.GetMouseButtonDown(0); // При нажатии лев кнопки мыши вернется true в противном случае false
#endif
    }

    public Vector2 GetCameraMoveVector() // Получить Вектор Движения Камеры
    {
#if USE_NEW_INPUT_SYSTEM //Если используем Новую Систему Ввода то компилировать код ниже (если закоментировать #define USE_NEW_INPUT_SYSTEM то код ниже не будет компилироваться)
       return _playerInpytActions.Player.CameraMovement.ReadValue<Vector2>(); //Зайдем в менеджер ввода, в Карту действий (Player), в действие Движение камеры (CameraMovement), и прочитаем преданные значения определив их тип <Vector2>
#else //в противном случае компилировать 
        Vector2 inputMoveDirection = new Vector2(0, 0); // Направление вводимого движенияи (обнуляем перед каждой трансформащией)

        if (Input.GetKey(KeyCode.W))
        {
            inputMoveDirection.y = +1f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            inputMoveDirection.y = -1f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            inputMoveDirection.x = -1f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            inputMoveDirection.x = +1f;
        }

        return inputMoveDirection;
#endif
    }

    public float GetCameraRotateAmount() //Получить величину поворота камеры
    {
#if USE_NEW_INPUT_SYSTEM //Если используем Новую Систему Ввода то компилировать код ниже (если закоментировать #define USE_NEW_INPUT_SYSTEM то код ниже не будет компилироваться)
        return _playerInpytActions.Player.CameraRotate.ReadValue<float>();//Зайдем в менеджер ввода, в Карту действий (Player), в действие поворота камеры (CameraRotate), и прочитаем преданные значения определив их тип <float>
#else
        float rotateAmount = 0f; // Величина поворота

        if (Input.GetKey(KeyCode.Q))
        {
            rotateAmount = +1f;
        }
        if (Input.GetKey(KeyCode.E))
        {
            rotateAmount = -1f;
        }

        return rotateAmount;
#endif
    }

    public float GetCameraZoomAmount() // Получить величину увеличения камеры
    {
#if USE_NEW_INPUT_SYSTEM //Если используем Новую Систему Ввода то компилировать код ниже (если закоментировать #define USE_NEW_INPUT_SYSTEM то код ниже не будет компилироваться)
        return _playerInpytActions.Player.CameraZoom.ReadValue<float>();//Зайдем в менеджер ввода, в Карту действий (Player), в действие увеличения камеры (CameraZoom), и прочитаем преданные значения определив их тип <float>
#else
        float zoomAmount = 0f; // Величина увеличения

        // Мы фиксируем лиш изминение величины прокрутки колесика и не учитываем ее величину (тоже самое что и фиксировать лиш нажатие клавиши например Input.GetKeyDown)
        if (Input.mouseScrollDelta.y > 0)
        {
            zoomAmount = -1f;
        }
        if (Input.mouseScrollDelta.y < 0)
        {
            zoomAmount = +1f;
        }

        return zoomAmount;
#endif
    }
}
