using System.Collections.Generic;
using Combat.Actions;
using Combat.Effects;
using Combat.Passives;
using DG.Tweening;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Worlds;

namespace Combat
{
    public class Unit : MonoBehaviour
    {
        [Header("Stats")] [SerializeField] private string unitName;
        [SerializeField] private int unitLevel;
        [SerializeField] private Stats baseStats;
        [SerializeField] private Stats currentStats;
        [SerializeField] private Vector2Int size;
        [SerializeField] private float currentHpPercentage = 1;

        [Header("References")] 
        [SerializeField] private GameObject actions;
        [SerializeField] private GameObject passives;

        [Header("UI")] 
        [SerializeField] private CanvasGroup actionsSelector;
        [SerializeField] private CanvasGroup actionsPanel;
        [SerializeField] private ActionButton actionButtonPrefab;
        [SerializeField] private TMP_Text title;
        [SerializeField] private Slider hpSlider;
        [SerializeField] private RectTransform info;

        public Vector2Int Size => size;
        public float Hp => currentHpPercentage * currentStats.MaxHp;

        private List<(IEffect, float)> _effects;
        private List<IAction> _actions;
        private List<IPassive> _passives;

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.magenta;

            var gridPos = World.Current.ClosestGridLocation(transform.position, new Vector3(size.x, size.y, 0));
            gridPos.x -= size.x * 0.5f;
            gridPos.y -= size.y * 0.5f;

            var bl = new Vector2(gridPos.x, gridPos.y);
            var br = new Vector2(gridPos.x + size.x, gridPos.y);
            var tl = new Vector2(gridPos.x, gridPos.y + size.y);
            var tr = new Vector2(gridPos.x + size.x, gridPos.y + size.y);

            Gizmos.DrawLine(bl, br);
            Gizmos.DrawLine(bl, tl);
            Gizmos.DrawLine(br, tr);
            Gizmos.DrawLine(tl, tr);
        }

        private void Start()
        {
            _effects = new List<(IEffect, float)>();
            _actions = new List<IAction>();
            _passives = new List<IPassive>();
            currentHpPercentage = 1;
            actions.GetComponents(_actions);
            passives.GetComponents(_passives);
            transform.position = World.Current.ClosestGridLocation(transform.position, size);
            CalculateStats();
            
            ActionButton first = null;

            foreach (var action in _actions)
            {
                var btn = Instantiate(actionButtonPrefab, actionsPanel.transform);
                btn.Init(action.Name, (RectTransform)actionsSelector.transform);
                btn.OnClick += () => action.Act(World.Current, this);
                first ??= btn;
            }
            
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)actionsPanel.transform);

            if (first != null)
            {
                var pos = actionsSelector.transform.position;
                pos.x = first.transform.position.x;
                pos.y = first.transform.position.y;
                actionsSelector.transform.position = pos;
            }

            actionsPanel.alpha = 0;
            actionsPanel.gameObject.SetActive(false);
            actionsSelector.alpha = 0;
            actionsSelector.gameObject.SetActive(false);

            title.text = $"Lv {unitLevel} - {unitName}";
            hpSlider.value = currentHpPercentage;

            var infoPos = info.transform.localPosition;
            infoPos.y = 0.5f * size.y;
            info.transform.localPosition = infoPos;
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(info);
            World.Current.AddUnit(transform.position, this);
        }

        private void Update()
        {
            for (var i = 0; i < _effects.Count; i++)
            {
                var (effect, durLeft) = _effects[i];
                durLeft -= Time.deltaTime;

                if (durLeft <= 0)
                {
                    _effects.RemoveAt(i);
                    i--;
                    CalculateStats();
                    continue;
                }

                _effects[i] = (effect, durLeft);
            }
        }

        public void AddEffect(IEffect effect)
        {
            _effects.Add((effect, effect.Duration));
            CalculateStats();
        }

        public IReadOnlyList<(IEffect, float)> GetEffects()
        {
            return _effects;
        }

        public void Select()
        {
            if (_actions.Count <= 0) return;
            actionsPanel.gameObject.SetActive(true);
            actionsPanel.DOFade(1, 0.1f);
            actionsSelector.gameObject.SetActive(true);
            actionsSelector.DOFade(1, 0.1f);
        }

        public void Unselect()
        {
            actionsPanel.DOFade(0, 0.1f)
                .OnComplete(() => actionsPanel.gameObject.SetActive(true));
            actionsSelector.DOFade(0, 0.1f)
                .OnComplete(() => actionsSelector.gameObject.SetActive(true));
        }

        private void CalculateStats()
        {
            currentStats = baseStats;

            foreach (var effect in _effects)
            {
                currentStats += effect.Item1.CalculateBonus(baseStats);
            }
        }
    }
}