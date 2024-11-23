using Combat.Units;
using DG.Tweening;
using UnityEngine;
using Worlds;

namespace Combat.Actions
{
    public class MeleeSkill : MonoBehaviour, IAction
    {
        public string Name => "Sneaky Striker";

        public string Description =>
            $"Dash to behind the target in {range} tile range, deal {100 * multiplier}% damage and gain a 15% crit chance for {bufDuration} rounds";

        [SerializeField] private int range;
        [SerializeField] private float multiplier;
        [SerializeField] private float bufDuration = 1;

        public void Act(World world, Unit unit, CombatManager combatManager)
        {
            world.AreaSelection.SelectUnit(range, unit.transform.position, true, (u, landingPos) =>
            {
                var damaged = false;
                var landingPosWorldSpace = world.CellToWorld(landingPos, unit.Size);

                var t = unit.transform.DOMove(landingPosWorldSpace, 0.05f)
                    .SetEase(Ease.InCubic)
                    .OnComplete(() => world.MoveUnit(unit.gridPosition, landingPos));

                t.OnUpdate(() =>
                {
                    if (!((unit.transform.position - u.transform.position).magnitude < 1) || damaged) return;
                    u.Hurt(multiplier * unit.CurrentStats.Attack, unit.UnitLevel);
                    damaged = true;
                });
            }, AreaSelection.Circle(range), (p, origin) =>
            {
                var dir = p - origin;

                dir.x = Mathf.Clamp(dir.x, 0, 1);
                dir.y = Mathf.Clamp(dir.y, 0, 1);
                
                return p + dir;
            });
        }

        public void PreviewArea(World world, Unit unit)
        {
            world.AreaSelection.PreviewArea(range, unit.transform.position, AreaSelection.Circle(range));
        }
    }
}