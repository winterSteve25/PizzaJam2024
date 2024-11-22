using Combat.Effects;
using Combat.Units;
using UnityEngine;
using Worlds;

namespace Combat.Actions
{
    public class ShieldTankSkill : MonoBehaviour, IAction
    {
        public string Name => "Defence Skill";
        public string Description => "";

        [SerializeField] private float defBufMultiplier = 1;
        [SerializeField] private int range = 3;

        public void Act(World world, Unit unit, CombatManager combatManager)
        {
            var halfRange = Mathf.FloorToInt(range * 0.5f);
            
            for (int i = -halfRange; i <= halfRange; i++)
            {
                for (int j = -halfRange; j <= halfRange; j++)
                {
                    if (i == 0 && j == 0) continue;
                    if (!world.GetUnitAt(new Vector2Int(i, j) + unit.gridPosition, out var other)) continue;
                    if (other == unit) continue;
                    other.AddEffect(new DefBuff(unit.CurrentStats.Defence * defBufMultiplier));
                }
            }
            
            combatManager.NextTurn();
        }

        public void PreviewArea(World world, Unit unit)
        {
            world.AreaSelection.PreviewArea(range, unit.transform.position, AreaSelection.Rect());
        }

        private class DefBuff : IEffect
        {
            public int Duration => 3;

            private float _buff;
            
            public DefBuff(float buff)
            {
                _buff = buff;
            }
            
            public Stats CalculateBonus(Stats baseStats)
            {
                return new Stats(0, _buff, 0, 0, 0, 0);
            }

            public void OnNewTurn(Unit unit)
            {
            }
        }
    }
}