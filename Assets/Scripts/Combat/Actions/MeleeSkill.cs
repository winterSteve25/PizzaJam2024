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
            world.AreaSelection.SelectUnit(range, unit.transform.position, false, (u, landingPos) =>
            {
                var damaged = false;
                var landingPosWorldSpace = world.CellToWorld(landingPos, unit.Size);

                var t = unit.transform.DOMove(landingPosWorldSpace, 0.05f)
                    .SetEase(Ease.InCubic)
                    .OnComplete(() =>
                    {
                        world.MoveUnit(unit.gridPosition, landingPos);
                        combatManager.NextTurn();
                    });

                t.OnUpdate(() =>
                {
                    if (!((unit.transform.position - u.transform.position).magnitude < 1) || damaged) return;
                    unit.DealDamageTo(u, multiplier * unit.CurrentStats.Attack);
                    damaged = true;
                });
            }, AreaSelection.Circle(range), (p, origin, u) =>
            {
                var dir = p - origin;

                dir.x = dir.x > 0 ? u.Size.x : 0;
                dir.y = dir.y > 0 ? u.Size.y : 0;
                
                return p + dir;
            });
        }

        public void PreviewArea(World world, Unit unit)
        {
            world.AreaSelection.PreviewArea(range, unit.transform.position, AreaSelection.Circle(range));
        }
    }
}