using System;
using Combat;
using Combat.Units;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Worlds;

namespace UI
{
    public class SelectedUnitUI : MonoBehaviour
    {
        [SerializeField] private CanvasGroup mainPanel;

        [Header("Info")] 
        [SerializeField] private TMP_Text title;
        [SerializeField] private TMP_Text hp;
        [SerializeField] private TMP_Text attack;
        [SerializeField] private TMP_Text defence;
        [SerializeField] private TMP_Text speed;
        [SerializeField] private TMP_Text critChance;
        [SerializeField] private TMP_Text critMultiplier;

        [Header("Actions")] 
        [SerializeField] private RectTransform actionsList;
        [SerializeField] private ActionButton actionPrefab;
        [SerializeField] private CanvasGroup arrow;
        [SerializeField] private RectTransform actionsPanel;

        private void OnEnable()
        {
            UnitSelectionManager.UnitSelected += UpdateVisual;
        }

        private void OnDisable()
        {
            UnitSelectionManager.UnitSelected -= UpdateVisual;
        }

        private void Start()
        {
            mainPanel.alpha = 0;
            ((RectTransform)mainPanel.transform).pivot = new Vector2(0, 1.5f);
            mainPanel.gameObject.SetActive(false);
            arrow.alpha = 0;
        }

        private void UpdateVisual(Unit unit)
        {
            if (unit == null)
            {
                HidePanel(null);
                World.Current.AreaSelection.RemovePreview();
                return;
            }

            if (mainPanel.gameObject.activeSelf)
            {
                HidePanel(() => UpdateVisual(unit));
                return;
            }

            title.text = $"Lv{unit.UnitLevel} - {unit.UnitName}";
            hp.text = $"HP: {unit.CurrentHpPercentage * unit.CurrentStats.MaxHp}/{unit.CurrentStats.MaxHp}";
            attack.text = $"ATK: {unit.CurrentStats.Attack}";
            defence.text = $"DEF: {unit.CurrentStats.Defence}";
            speed.text = $"SPD: {unit.CurrentStats.Speed}";
            critChance.text = $"Crit Chance: {unit.CurrentStats.CritChance}";
            critMultiplier.text = $"Crit Multiplier: {unit.CurrentStats.CritMultiplier}";

            var isTurnOf = CombatManager.Current.IsTurnOf(unit);
            if (isTurnOf)
            {
                actionsPanel.gameObject.SetActive(true);
                ActionButton first = null;

                for (int i = 0; i < actionsList.transform.childCount; i++)
                {
                    Destroy(actionsList.transform.GetChild(i).gameObject);
                }

                foreach (var action in unit.Actions)
                {
                    var btn = Instantiate(actionPrefab, actionsList.transform);
                    btn.Init(action.Name, (RectTransform)arrow.transform);
                    btn.OnClick += () =>
                    {
                        HidePanel(null);
                        action.Act(World.Current, unit, CombatManager.Current);
                    };
                    btn.OnHover += () => action.PreviewArea(World.Current, unit);
                    first ??= btn;
                }

                LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)actionsList.transform);
                LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)mainPanel.transform);

                if (first != null)
                {
                    var firstTransform = (RectTransform)first.transform;
                    LayoutRebuilder.ForceRebuildLayoutImmediate(firstTransform);
                    var pos = arrow.transform.position;
                    pos.x = firstTransform.position.x;
                    pos.y = firstTransform.position.y;
                    arrow.transform.position = pos;
                }
            }
            else
            {
                actionsPanel.gameObject.SetActive(false);
            }

            arrow.alpha = 0;
            ShowPanel(isTurnOf);
        }

        private void ShowPanel(bool showArrow)
        {
            mainPanel.gameObject.SetActive(true);
            mainPanel.blocksRaycasts = true;
            mainPanel.interactable = true;
            
            mainPanel.DOFade(1, 0.2f);
            ((RectTransform)mainPanel.transform).DOPivotY(0, 0.2f)
                .SetEase(Ease.OutExpo);

            if (showArrow)
            {
                arrow.DOFade(1, 0.05f);
            }
        }

        private void HidePanel(Action callback)
        {
            mainPanel.blocksRaycasts = false;
            mainPanel.interactable = false;
            
            mainPanel.DOFade(0, 0.2f);
            arrow.DOFade(0, 0.05f);

            ((RectTransform)mainPanel.transform).DOPivotY(1.5f, 0.2f)
                .SetEase(Ease.OutCubic)
                .OnComplete(() =>
                {
                    mainPanel.gameObject.SetActive(false);
                    callback?.Invoke();
                });
        }
    }
}