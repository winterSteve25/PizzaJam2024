using Combat.Units;
using UnityEngine;
using Worlds;

namespace Combat.Actions
{
    public class AttackAction : MonoBehaviour, IAction
    {
        public string Name => "Attack";
        
        public void Act(World world, Unit unit)
        {
            world.AreaSelection.Select(unit.transform.position, new Vector2Int(1, 1), 5, (p) =>
            {
                Debug.Log(p);
            });
        }
    }
}