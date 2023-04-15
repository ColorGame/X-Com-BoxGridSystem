using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// В игровм объекте ActionVirtualCamera лежит наша виртуальная камера для экшен стрельбы, настроим ее как и основную CinemachineVirtualCamera
// во вкладке Add Extension добавили Cinemachine Impulse Listener с такими же параметрами как и у основной (можно скопировать компонент у основной и вставить в камеру для экшен съемок)

public class ScreenShakeActions : MonoBehaviour //Независимый класс который реализует тряску экрана (связующее звено)
{


    private void Start()
    {
        ShootAction.OnAnyShoot += ShootAction_OnAnyShoot; // Подпишемся на событие Любой Начал стрелять
        GrenadeProjectile.OnAnyGrenadeExploded += GrenadeProjectile_OnAnyGrenadeExploded;// Подпишемся на событие Любая граната взорвалась
        SwordAction.OnAnySwordHit += SwordAction_OnAnySwordHit;// Подпишемся на событие  Любой Начал удар мечом
    }

    private void SwordAction_OnAnySwordHit(object sender, SwordAction.OnSwordEventArgs e)
    {
        ScreenShake.Instance.Shake(2); // По умолчанию интенсивность тряски = 1
    }

    private void GrenadeProjectile_OnAnyGrenadeExploded(object sender, System.EventArgs e)
    {
        ScreenShake.Instance.Shake(4f); // Интенсивность тряски при взрыве гранаты установим 2 //НУЖНО НАСТРОИТЬ//
    }

    private void ShootAction_OnAnyShoot(object sender, ShootAction.OnShootEventArgs e)
    {
        ScreenShake.Instance.Shake(); // По умолчанию интенсивность тряски = 1
    }
   
}
