using Combat.Units;
using UnityEngine;
using Worlds;

namespace Combat.Actions
{
    public class HealerTankSkill : MonoBehaviour, IAction
    {
        public string Name => "Heal Self";
        public string Description => $"Heal self by {healPercentage * 100}";

        [SerializeField] private float healPercentage = 0.1f;
        
        public void Act(World world, Unit unit, CombatManager combatManager)
        {
            unit.HealPercentage(healPercentage);
            combatManager.NextTurn();
        }

        public void PreviewArea(World world, Unit unit)
        {
            world.AreaSelection.PreviewArea(1, unit.transform.position, AreaSelection.Rect());
        }
    }
}