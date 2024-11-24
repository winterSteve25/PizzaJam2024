using Combat.Effects;
using Combat.Units;
using UnityEngine;
using Worlds;

namespace Combat.Actions
{
    public class BufferSkill : MonoBehaviour, IAction
    {
        public string Name => "Let's GOOOO";
        public string Description => $"Grants an ally {speedBuff * 100}% speed bonus and {critChanceBuff * 100}% bonus CRIT Chance lasting for 2 turns";

        [SerializeField] private float speedBuff;
        [SerializeField] private float critChanceBuff;
        [SerializeField] private int range;
        [SerializeField] private int duration;

        public void Act(World world, Unit unit, CombatManager combatManager)
        {
            world.AreaSelection.SelectUnit(range, unit.transform.position, true, (u, p) =>
            {
                u.AddEffect(new Buff(duration, speedBuff, critChanceBuff));
            }, AreaSelection.Circle(range));
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
                    0,
                    0
                );
            }
        }
    }
}