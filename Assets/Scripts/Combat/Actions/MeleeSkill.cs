using Combat.Units;
using UnityEngine;
using Worlds;

namespace Combat.Actions
{
    public class MeleeSkill : MonoBehaviour, IAction
    {
        public string Name => "Sneaky Striker";
        public string Description => $"Dash to behind the target in {range} tile range, deal {_ATK * multiplier} damage and gain a 15% crit chance for 1 rounds";
        private float _ATK => transform.parent.GetComponent<Unit>().CurrentStats.Attack;

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