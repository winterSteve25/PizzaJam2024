using System.Linq;
using Combat.Effects;
using Combat.Units;
using UnityEngine;
using Worlds;

namespace Combat.Actions
{
    public class BufferNormalAttack : GappedAttack
    {
        [SerializeField] private float randomAtkBuff;
        [SerializeField] private int buffDuration;

        public override void Act(World world, Unit unit, CombatManager combatManager)
        {
            world.AreaSelection.Select(range, unit.transform.position, size, (p, s) =>
            {
                for (int i = 0; i < size.x; i++)
                {
                    for (int j = 0; j < size.y; j++)
                    {
                        if (!world.GetUnitAt(new Vector2Int(i, j) + p, out var u)) continue;
                        unit.DealDamageTo(u, unit.CurrentStats.Dot(multipliers));
                    }
                }

                var allies = combatManager.GetAllies().ToList();
                var k = Random.Range(0, allies.Count);
                allies[k].AddEffect(new RandomAttackBuff(buffDuration, randomAtkBuff));
                
                combatManager.NextTurn();
            }, Shape, (p, origin) => world.AreaSelection.Passable(p, origin) && Shape(p, origin));
        }

        private class RandomAttackBuff : IEffect
        {
            public int Duration { get; }
            public bool Stackable => true;

            private float _buff;

            public RandomAttackBuff(int duration, float buff)
            {
                Duration = duration;
                _buff = buff;
            }

            public Stats CalculateBonus(Stats baseStats)
            {
                return new Stats(
                    _buff * baseStats.Attack,
                    0,
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