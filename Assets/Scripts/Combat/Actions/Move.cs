using Combat.Units;
using DG.Tweening;
using UnityEngine;
using Worlds;

namespace Combat.Actions
{
    public class Move : MonoBehaviour, IAction
    {
        public string Name => "Move";

        [SerializeField] private int range;
        
        public void Act(World world, Unit unit)
        {
            world.AreaSelection.Select(range, unit.transform.position, Vector2Int.one, (p) =>
            {
                world.MoveUnit(unit.gridPosition, (Vector2Int)p);
                unit.transform.DOMove(p, 0.4f).SetEase(Ease.InOutCubic);
            }, AreaSelection.Circle(range), world.AreaSelection.Passable);
        }

        public void PreviewArea(World world, Unit unit)
        {
            world.AreaSelection.PreviewArea(range, unit.transform.position, AreaSelection.Circle(range));
        }

        public void RemovePreview(World world)
        {
            world.AreaSelection.RemovePreview();
        }
    }
}