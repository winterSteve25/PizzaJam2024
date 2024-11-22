using Combat.Effects;
using Combat.Units;
using UnityEngine;
using Worlds;

namespace Combat.Actions
{
    public class BufferSkill : MonoBehaviour, IAction
    {
        public string Name => "Let's GOOOO";
        public string Description => "Grants an ally 10% speed bonus and 25% bonus CRIT Chance lasting for 2 turns";

        public void Act(World world, Unit unit, CombatManager combatManager)
        {
        }

        public void PreviewArea(World world, Unit unit)
        {
        }

        private class Buff : IEffect
        {
            private int _duration;
            private float _speedBuff;
            private float _critBuff;

            public int Duration => _duration;

            public Buff(int duration, float speedBuff, float critBuff)
            {
                _duration = duration;
                _speedBuff = speedBuff;
                _critBuff = critBuff;
            }

            public Stats CalculateBonus(Stats baseStats)
            {
                return new Stats(
                    0,
                    0,
                    0,
                    _speedBuff * baseStats.Speed,
                    _critBuff,
                    0
                );
            }

            public void OnNewTurn(Unit unit)
            {
            }
        }
    }
}