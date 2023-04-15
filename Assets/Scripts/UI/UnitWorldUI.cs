using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitWorldUI : MonoBehaviour // Мировой пользовательский интерфейс юнита //Лежит в canvas на юните
{
    [SerializeField] private TextMeshProUGUI _actionPointsText; // Закинуть текс UI
    [SerializeField] private Unit _unit; // в инспекторе закинуть юнита
    [SerializeField] private Image _healthBarImage; // в инспекторе закинуть шкалу здоровья "Bar"
    [SerializeField] private HealthSystem _healthSystem; // Закинуть самого юнита тк скрипт висит на нем
    [SerializeField] private TextMeshProUGUI _healthPointsText; // Закинуть текст здоровья


    private void Start()
    {
        Unit.OnAnyActionPointsChanged += Unit_OnAnyActionPointsChanged; // Подпишемся на статическое событие (любое изминение очков действий) // Небольшой недостаток - это событие вызывается когда изменяется ActionPoints у любого юнита, а это немного расточительно но незначительно
        _healthSystem.OnDamage += HealthSystem_OnDamage; // Подпишемся на событие Получил повреждение.

        UpdateActionPointsText();
        UpdateHealthBar();

       
    }


    private void UpdateActionPointsText() // Обнавления текста Очков Действия
    {
        _actionPointsText.text = _unit.GetActionPoints().ToString(); // Вернем очки действия юнита преобразуеи в строку и передадим в текст который отображается над юнитом
    }

    private void Unit_OnAnyActionPointsChanged(object sender, EventArgs e)
    {
        UpdateActionPointsText();
    }

    /*//  Если вы хотите точно знать, какой юнит претерпел изменения в OnAnyActionPointsChanged, вам просто нужно указать отправителя как Юнита.
    private void Unit_OnAnyActionPointsChanged(object sender, EventArgs args)
    {
        Unit unit = sender as Unit;
        Debug.Log($"у {unit} очки деиствий изменились.");
    }*/
    private void UpdateHealthBar() // Обновления шкалы здоровья
    {
        _healthBarImage.fillAmount = _healthSystem.GetHealthNormalized();
        _healthPointsText.text = _healthSystem.GetHealth().ToString();

    }
    private void HealthSystem_OnDamage(object sender, EventArgs e) // при наступления события обновим шкалу жизни
    {
        UpdateHealthBar();
    }

}
