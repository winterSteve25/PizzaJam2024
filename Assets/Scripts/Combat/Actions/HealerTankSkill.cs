using Combat.Units;
using Worlds;

namespace Combat.Actions
{
    public class HealerTankSkill : IAction
    {
        public string Name => "Heal Self";
        public string Description => "";
        
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