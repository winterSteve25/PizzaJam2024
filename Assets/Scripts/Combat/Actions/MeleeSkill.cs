using Combat.Units;
using DG.Tweening;
using UnityEngine;
using Worlds;

namespace Combat.Actions
{
    public class MeleeSkill : MonoBehaviour, IAction
    {
        public string Name => "Sneaky Striker";
        public string Description => $"Dash to behind the target in {range} tile range, deal {100 * multiplier}% damage and gain a 15% crit chance for {bufDuration} rounds";

        [SerializeField] private int range;
        [SerializeField] private float multiplier;
        [SerializeField] private float bufDuration = 1;
        
        public void Act(World world, Unit unit, CombatManager combatManager)
        {
            world.AreaSelection.SelectUnit(range, unit.transform.position, true, (u, landingPos) =>
            {
                u.transform.DOMove(new Vector3(landingPos.x, landingPos.y), 0.05f)
                    .SetEase(Ease.InCubic);
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