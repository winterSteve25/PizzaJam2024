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
            Debug.Log("WOAH");
        }
    }
}