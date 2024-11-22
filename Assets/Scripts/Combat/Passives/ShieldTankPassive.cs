using System;
using Combat.Units;
using UnityEngine;

namespace Combat.Passives
{
    public class ShieldTankPassive : MonoBehaviour, IPassive
    {
        private int _cooldown;

        private void Start()
        {
            _cooldown = 0;
        }

        public void OnNewTurn(Unit unit)
        {
            if (_cooldown <= 0) return;
            _cooldown--;
        }

        public void OnDamaged(Unit unit, Unit attacker)
        {
            if (_cooldown > 0) return;
            if (!(unit.CurrentHpPercentage < 0.1)) return;
            unit.HealPercentage(0.35f);
            _cooldown = 3;
        }
    }
}