using Combat.Units;
using UnityEngine;
using Worlds;

namespace Combat.Actions
{
    public class MeleeSkill : MonoBehaviour, IAction
    {
        public string Name => "Sneaky Striker";
        public string Description => "";

        [SerializeField] private int range;
        [SerializeField] private float multiplier;
        
        public void Act(World world, Unit unit, CombatManager combatManager)
        {
            world.AreaSelection.SelectUnit(range, unit.transform.position, false, (u) =>
            {

            }, AreaSelection.Circle(range), (p, origin) =>
            {
                var dir = p - origin;
                return p + dir;
            });
        }

        public void PreviewArea(World world, Unit unit)
        {
            world.AreaSelection.PreviewArea(range, unit.transform.position, AreaSelection.Circle(range));
        }
    }
}