using Combat.Units;
using Worlds;

namespace Combat.Actions
{
    public class HealerTankSkill : IAction
    {
        public string Name => "Heal Self";
        
        public void Act(World world, Unit unit)
        {
            unit.HealPercentage(0.1f);
        }

        public void PreviewArea(World world, Unit unit)
        {
        }

        public void RemovePreview(World world)
        {
        }
    }
}