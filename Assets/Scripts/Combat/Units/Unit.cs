using System;
using System.Collections.Generic;
using System.Linq;
using Combat.Actions;
using Combat.DmgNumber;
using Combat.Effects;
using Combat.Passives;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Worlds;
using Random = UnityEngine.Random;

namespace Combat.Units
{
    public class Unit : MonoBehaviour
    {
        [Header("Stats")] public Vector2Int gridPosition;
        [field: SerializeField] public string UnitName { get; protected set; }
        [field: SerializeField] public int UnitLevel { get; protected set; }
        [field: SerializeField] public Stats BaseStats { get; protected set; }
        [field: SerializeField] public Stats CurrentStats { get; protected set; }
        [field: SerializeField] public Vector2Int Size { get; protected set; }
        [field: SerializeField] public float CurrentHpPercentage { get; protected set; } = 1;

        [Header("References")] [SerializeField]
        protected GameObject actions;

        [SerializeField] protected GameObject passives;
        protected Material _spriteMaterial;

        [Header("UI")] [SerializeField] protected Slider hpSlider;
        [SerializeField] protected RectTransform info;
        protected static readonly int HurtProgressKey = Shader.PropertyToID("_Progress");

        public List<(IEffect, int)> Effects { get; protected set; }
        public List<IAction> Actions { get; protected set; }
        public List<IPassive> Passives { get; protected set; }

        private void OnDrawGizmosSelected()
        {
            World.DrawBound(transform.position, Size);
        }

        protected virtual void Awake()
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

            hpSlider.value = CurrentHpPercentage;

            var infoPos = info.transform.localPosition;
            infoPos.y = 0.6f * Size.y;
            info.transform.localPosition = infoPos;

            LayoutRebuilder.ForceRebuildLayoutImmediate(info);
        }

        protected virtual void Start()
        {
            _spriteMaterial = GetComponentInChildren<SpriteRenderer>().material;
        }

        public void AddEffect(IEffect effect)
        {
            var existing = Effects.FindIndex(x => x.Item1.GetType() == effect.GetType() && !x.Item1.Stackable);
            if (existing == -1)
            {
                Effects.Add((effect, effect.Duration));
                CalculateStats();
                return;
            }
            
            var e = Effects[existing];
            e.Item2 = effect.Duration;
            Effects[existing] = e;
        }

        protected void CalculateStats()
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

        private float CalculateDmg(float raw, out bool crit)
        {
            if (Random.Range(0f, 1f) < Mathf.Clamp01(CurrentStats.CritChance))
            {
                crit = true;
                return raw * (1 + CurrentStats.CritMultiplier);
            }

            crit = false;
            return raw;
        }

        public virtual void Hurt(float raw, Unit attacker, out bool crit)
        {
            var currHp = CurrentHpPercentage * CurrentStats.MaxHp;
            var dmg = DamageReceived(CalculateDmg(raw, out crit), attacker.UnitLevel);
            var newPerc = (currHp - dmg) / CurrentStats.MaxHp;

            DamageNumberManager.Current.CreateNumber(dmg, transform.position, false, crit);

            if (newPerc <= 0)
            {
                Die();
                CurrentHpPercentage = 0;
                return;
            }

            foreach (var passive in Passives)
            {
                passive.OnDamaged(this);
            }

            foreach (var passive in Effects)
            {
                passive.Item1.OnDamaged(this);
            }

            DOTween.To(
                () => _spriteMaterial.GetFloat(HurtProgressKey),
                x => _spriteMaterial.SetFloat(HurtProgressKey, x),
                1f,
                0.05f
            ).OnComplete(() =>
            {
                DOTween.To(
                    () => _spriteMaterial.GetFloat(HurtProgressKey),
                    x => _spriteMaterial.SetFloat(HurtProgressKey, x),
                    0f,
                    0.05f
                ).SetDelay(0.1f);
            });

            hpSlider.DOValue(CurrentHpPercentage, 0.1f)
                .SetEase(Ease.OutCubic);
            
            CurrentHpPercentage = newPerc;
        }

        public virtual void Heal(float raw)
        {
            HealPercentage(raw / CurrentStats.MaxHp);
        }

        public virtual void HealPercentage(float percentage)
        {
            DamageNumberManager.Current.CreateNumber(percentage * CurrentStats.MaxHp, transform.position, true, false);
            CurrentHpPercentage += percentage;
            CurrentHpPercentage = Mathf.Clamp01(CurrentHpPercentage);
        }

        public virtual void DealDamageTo(Unit unit, float raw)
        {
            unit.Hurt(raw, this, out var crit);

            foreach (var passive in Passives)
            {
                passive.OnAttack(this, unit, crit);
            }

            foreach (var passive in Effects)
            {
                passive.Item1.OnAttack(this, unit, crit);
            }
        }

        protected virtual void Die()
        {
            World.Current.RemoveUnit(gridPosition);
            Destroy(gameObject);
        }

        public virtual bool IsAlly()
        {
            return true;
        }

        public virtual void TurnStarted()
        {
            foreach (var passive in Passives)
            {
                passive.OnNewTurn(this);
            }

            foreach (var passive in Effects)
            {
                passive.Item1.OnNewTurn(this);
            }

            for (var i = 0; i < Effects.Count; i++)
            {
                var (effect, duration) = Effects[i];
                effect.OnNewTurn(this);

                if (duration - 1 == 0)
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