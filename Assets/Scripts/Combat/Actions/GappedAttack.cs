using Combat.Units;
using UnityEngine;
using Worlds;

namespace Combat.Actions
{
    public class GappedAttack : MonoBehaviour, IAction
    {
        public string Name => "Normal Attack";

        [SerializeField] private int range = 5;
        [SerializeField] private int gap = 1;
        [SerializeField] private float multiplier;
        [SerializeField] private Vector2Int size;

        public void Act(World world, Unit unit, CombatManager combatManager)
        {
            world.AreaSelection.Select(range, unit.transform.position, size, (p, s) =>
            {
                for (int i = 0; i < size.x; i++)
                {
                    for (int j = 0; j < size.y; j++)
                    {
                        if (!world.GetUnitAt(new Vector2Int(i, j) + p, out var u)) continue;
                        u.Hurt(unit.CurrentStats.Attack * multiplier, unit.UnitLevel);
                    }
                }
            }, Shape, (p, origin) => world.AreaSelection.Passable(p, origin) && Shape(p, origin));
        }

        private bool Shape(Vector2Int p, Vector2Int origin)
        {
            var diff = p - origin;
            var dx = Mathf.Abs(diff.x) - 1;
            var dy = Mathf.Abs(diff.y) - 1;

            return (dx == gap || dy == gap) && (p.x == origin.x || p.y == origin.y);
        }

        public void PreviewArea(World world, Unit unit)
        {
            world.AreaSelection.PreviewArea(range, unit.transform.position, Shape);
        }
    }
}