using Combat.Units;
using UnityEngine;
using Worlds;

namespace Combat.Actions
{
    public class MeleeNormalAtk : MonoBehaviour, IAction
    {
        public string Name => "Normal Atk";

        [SerializeField] private int range = 3;
        [SerializeField] private float multiplier = 1.7f;

        public void Act(World world, Unit unit)
        {
            world.AreaSelection.Select(3, unit.transform.position, new Vector2Int(3, 1), (p, size) =>
            {
                for (int i = 0; i < size.x; i++)
                {
                    for (int j = 0; j < size.y; j++)
                    {
                        if (!world.GetUnitAt(new Vector2Int(i, j) + p, out var u)) continue;
                        u.Hurt(unit.CurrentStats.Attack * multiplier, unit.UnitLevel);
                    }
                }
            }, AreaSelection.Circle(3), world.AreaSelection.Passable);
        }

        public void PreviewArea(World world, Unit unit)
        {
        }

        public void RemovePreview(World world)
        {
        }
    }
}