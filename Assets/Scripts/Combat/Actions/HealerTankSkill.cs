using Combat.Units;
using UnityEngine;
using Worlds;

namespace Combat.Actions
{
    public class HealerTankSkill : MonoBehaviour, IAction
    {
        public string Name => "Heal Self";
        public string Description => $"Heal self by {_HEALTH * (10/100)} Health";
        private float _HEALTH => transform.parent.GetComponent<Unit>().CurrentStats.MaxHp;

        
        public void Act(World world, Unit unit, CombatManager combatManager)
        {
            unit.HealPercentage(0.1f);
            combatManager.NextTurn();
        }

        public void PreviewArea(World world, Unit unit)
        {
        }
    }
}