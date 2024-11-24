using System.Collections.Generic;
using Combat.Actions;
using Combat.Effects;
using Combat.Passives;
using UnityEngine.UI;
using Worlds;

namespace Combat.Units.Shaman
{
    public class Totem : EnemyUnit
    {
        private bool _blue;
        private GoblinShaman _master;
        
        protected override void Awake()
        {
            Effects = new List<(IEffect, int)>();
            Actions = new List<IAction>();
            Passives = new List<IPassive>();
            CurrentHpPercentage = 1;
            actions.GetComponents(Actions);
            passives.GetComponents(Passives);
            transform.position = World.Current.SnapToGrid(transform.position, Size);

            CalculateStats();
            hpSlider.value = CurrentHpPercentage;

            var infoPos = info.transform.localPosition;
            infoPos.y = 0.6f * Size.y;
            info.transform.localPosition = infoPos;

            LayoutRebuilder.ForceRebuildLayoutImmediate(info);
        }

        public void AddToWorld(GoblinShaman master)
        {
            World.Current.AddUnit(transform.position, this);
            CombatManager.Current.AddUnit(this);
            _master = master;
        }

        public void SetMode(bool blue)
        {
            _blue = blue;
        }

        public override void Hurt(float raw, Unit attacker, out bool crit)
        {
            switch (_blue)
            {
                case true when _master.BlueMode:
                case false when !_master.BlueMode:
                    _master.Hurt(raw, attacker, out crit);
                    return;
            }

            _master.CounterAttack(attacker);
            crit = false;
        }

        public override void TurnStarted()
        {
            base.TurnStarted();
            CombatManager.Current.NextTurn();
        }
    }
}