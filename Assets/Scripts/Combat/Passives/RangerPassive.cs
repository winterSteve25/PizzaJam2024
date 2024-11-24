using Combat.Units;
using UnityEngine;

namespace Combat.Passives
{
    public class RangerPassive : MonoBehaviour, IPassive
    {
        [SerializeField] private float extraDmgMultiplier;
        
        public void OnAttack(Unit unit, Unit target, bool didCrit)
        {
            if (!didCrit) return;
            unit.DealDamageTo(target, unit.CurrentStats.Attack * extraDmgMultiplier);
        }
    }
}