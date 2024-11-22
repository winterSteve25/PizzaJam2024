using Combat.Effects;
using Combat.Units;
using UnityEngine;
using Worlds;

namespace Combat.Actions
{
    public class DebufferSkill : MonoBehaviour, IAction
    {
        public string Name => "Debuff Skill";
        public string Description => "";

        [SerializeField] private int range;
        [SerializeField] private float defDebuff = 0.2f;
        [SerializeField] private float atkDebuff = 0.3f;
        [SerializeField] private int duration = 3;

        public void Act(World world, Unit unit, CombatManager combatManager)
        {
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
                    0
                );
            }

            public void OnNewTurn(Unit unit)
            {
            }
        }
    }
}