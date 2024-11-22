using System.Collections.Generic;
using Combat.Actions;
using Combat.Effects;
using Combat.Passives;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Worlds;
using Random = UnityEngine.Random;

namespace Combat.Units
{
    public class Unit : MonoBehaviour, IDamagable
    {
        [Header("Stats")]
        [field: SerializeField]
        public string UnitName { get; private set; }

        [field: SerializeField] public int UnitLevel { get; private set; }
        [field: SerializeField] public Stats BaseStats { get; private set; }
        [field: SerializeField] public Stats CurrentStats { get; private set; }
        [field: SerializeField] public Vector2Int Size { get; private set; }
        [field: SerializeField] public float CurrentHpPercentage { get; private set; } = 1;

        [Header("References")] 
        [SerializeField] private GameObject actions;
        [SerializeField] private GameObject passives;

        [Header("UI")] [SerializeField] private TMP_Text title;
        [SerializeField] private Slider hpSlider;
        [SerializeField] private RectTransform info;

        public List<(IEffect, int)> Effects { get; private set; }
        public List<IAction> Actions { get; private set; }
        public List<IPassive> Passives { get; private set; }
        
        [FormerlySerializedAs("worldPosition")] public Vector2Int gridPosition;

        private void OnDrawGizmosSelected()
        {
            World.DrawBound(transform.position, Size);
        }

        private void Awake()
        {
            Effects = new List<(IEffect, int)>();
            Actions = new List<IAction>();
            Passives = new List<IPassive>();
            CurrentHpPercentage = 1;
            actions.GetComponents(Actions);
            passives.GetComponents(Passives);
            transform.position = World.Current.SnapToGrid(transform.position, Size);

            CalculateStats();
            
            // NOTE: THIS REQUIRES World.Awake TO RUN BEFORE THIS
            World.Current.AddUnit(transform.position, this);
            CombatManager.Current.AddUnit(this);

            title.text = $"Lv {UnitLevel} - {UnitName}";
            hpSlider.value = CurrentHpPercentage;

            var infoPos = info.transform.localPosition;
            infoPos.y = 0.5f * Size.y;
            info.transform.localPosition = infoPos;

            LayoutRebuilder.ForceRebuildLayoutImmediate(info);
        }

        public void AddEffect(IEffect effect)
        {
            Effects.Add((effect, effect.Duration));
            CalculateStats();
        }

        private void CalculateStats()
        {
            CurrentStats = BaseStats;

            foreach (var effect in Effects)
            {
                CurrentStats += effect.Item1.CalculateBonus(BaseStats);
            }
        }

        private float DamageReceived(float raw, int level)
        {
            float levelMultiplier = L(UnitLevel - level);
            float defenceMultiplier = 1 / L(CurrentStats.Defence * 0.05f);

            return raw * levelMultiplier * defenceMultiplier;
        }

        private float L(float x)
        {
            return Mathf.Pow(2, x * 0.1f);
        }

        public float CalculateDmg(float raw)
        {
            if (Random.Range(0f, 1f) < Mathf.Clamp01(CurrentStats.CritChance))
            {
                // crit
                return raw * (1 + CurrentStats.CritMultiplier);
            }

            return raw;
        }

        public void Hurt(float raw, int level)
        {
            var currHp = CurrentHpPercentage * CurrentStats.MaxHp;
            var dmg = DamageReceived(raw, level);
            var newPerc = (currHp - dmg) / CurrentStats.MaxHp;

            if (newPerc <= 0)
            {
                Die();
            }
            
            CurrentHpPercentage = newPerc;
        }

        public void Heal(float raw)
        {
            HealPercentage(raw / CurrentStats.MaxHp);
        }

        public void HealPercentage(float percentage)
        {
            CurrentHpPercentage += percentage;
            CurrentHpPercentage = Mathf.Clamp01(CurrentHpPercentage);
        }

        private void Die()
        {
        }

        public bool IsAlly()
        {
            return true;
        }

        public void TurnStarted()
        {
            foreach (var passive in Passives)
            {
                passive.OnNewTurn(this);
            }

            for (var i = 0; i < Effects.Count; i++)
            {
                var (effect, duration) = Effects[i];
                effect.OnNewTurn(this);

                if (duration - 1 <= 0)
                {
                    Effects.RemoveAt(i);
                    i--;
                    continue;
                }

                Effects[i] = (effect, duration - 1);
            }
        }
    }
}