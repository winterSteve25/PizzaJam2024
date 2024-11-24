using Combat.Units;
using UnityEngine;

namespace Combat.Effects
{
    public interface IEffect
    {
        public int Duration { get; }
        public bool Stackable => false;

        public Stats CalculateBonus(Stats baseStats)
        {
            return new Stats(); 
        }

        public void OnNewTurn(Unit unit) {}

        public void OnAttack(Unit unit, Unit target, bool didCrit)
        {
        }
        
        public void OnDamaged(Unit unit) {}
    }
}