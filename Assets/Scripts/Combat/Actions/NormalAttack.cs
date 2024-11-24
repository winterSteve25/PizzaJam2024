using Combat.Units;
using UnityEngine;
using Worlds;

namespace Combat.Actions
{
    public class NormalAttack : MonoBehaviour, IAction
    {
        public string Name => "Normal Attack";
        public string Description => "";

        [SerializeField] private int range = 3;
        [SerializeField] private Vector2Int size;
        [SerializeField] private Stats multipliers;
        
        public void Act(World world, Unit unit, CombatManager combatManager)
        {
            world.AreaSelection.Select(range, unit.transform.position, size, (p, size) =>
            {
                for (int i = 0; i < size.x; i++)
                {
                    for (int j = 0; j < size.y; j++)
                    {
                        if (!world.GetUnitAt(new Vector2Int(i, j) + p, out var u)) continue;
                        unit.DealDamageTo(u, unit.CurrentStats.Dot(multipliers));
                    }
                }
                
                combatManager.NextTurn();
            }, AreaSelection.Circle(range), world.AreaSelection.Passable);
        }

        public void PreviewArea(World world, Unit unit)
        {
            world.AreaSelection.PreviewArea(range, unit.transform.position, AreaSelection.Circle(range));
        }
    }
}