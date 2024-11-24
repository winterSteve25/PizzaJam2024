using Combat.Effects;
using Combat.Units;
using UnityEngine;
using Worlds;

namespace Combat.Actions
{
    public class DebufferSkill : MonoBehaviour, IAction
    {
        public string Name => "Depression";

        public string Description =>
            $"Reduce all Enemy DEF by {defDebuff * 100}%, Enemy ATK by {atkDebuff * 100}% for {duration} turns";

        [SerializeField] private int range;
        [SerializeField] private float defDebuff = 0.2f;
        [SerializeField] private float atkDebuff = 0.3f;
        [SerializeField] private int duration = 3;

        public void Act(World world, Unit unit, CombatManager combatManager)
        {
            var pred = AreaSelection.Circle(range);

            var halfRange = Mathf.FloorToInt(range * 0.5f);

            for (int i = -halfRange; i <= halfRange; i++)
            {
                for (int j = -halfRange; j <= halfRange; j++)
                {
                    if (i == 0 && j == 0) continue;
                    if (!pred(new Vector2Int(i, j) + unit.gridPosition, unit.gridPosition)) continue;
                    if (!world.GetUnitAt(new Vector2Int(i, j) + unit.gridPosition, out var other)) continue;
                    if (other.IsAlly()) continue;
                    other.AddEffect(new Debuff(duration, defDebuff, atkDebuff));
                }
            }

            combatManager.NextTurn();
        }

        public void PreviewArea(World world, Unit unit)
        {
            world.AreaSelection.PreviewArea(range, unit.transform.position, AreaSelection.Circle(range));
        }

        private class Debuff : IEffect
        {
            private int _duration;
            public int Duration => _duration;

            private float _defDebuff;
            private float _atkDebuff;

            public Debuff(int duration, float defDebuff, float atkDebuff)
            {
                _duration = duration;
                _defDebuff = defDebuff;
                _atkDebuff = atkDebuff;
            }

            public Stats CalculateBonus(Stats baseStats)
            {
                return new Stats(
                    -_atkDebuff * baseStats.Attack,
                    -_defDebuff * baseStats.Defence,
                    0,
                    0,
                    0,
                    0,
                    0
                );
            }
        }
    }
}