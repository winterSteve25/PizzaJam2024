using Combat.Units;
using DG.Tweening;
using UnityEngine;
using Worlds;

namespace Combat.Actions
{
    public class Move : MonoBehaviour, IAction
    {
        public string Name => "Move";
        public string Description => "";

        [SerializeField] private int range;
        
        public void Act(World world, Unit unit, CombatManager combatManager)
        {
            world.AreaSelection.Select(range, unit.transform.position, Vector2Int.one, (p, size) =>
            {
                unit.transform.DOMove(world.CellToWorld(p, unit.Size), 0.4f)
                    .SetEase(Ease.InOutCubic);
                world.MoveUnit(unit.gridPosition, p);
                combatManager.NextTurn();
            }, AreaSelection.Circle(range), world.AreaSelection.PassableAndEmpty);
        }

        public void PreviewArea(World world, Unit unit)
        {
            world.AreaSelection.PreviewArea(range, unit.transform.position, AreaSelection.Circle(range));
        }
    }
}