using Combat.Effects;
using Combat.Units;
using UnityEngine;

namespace Combat.Passives
{
    public class DebufferPassive : MonoBehaviour, IPassive
    {
        [SerializeField] private float chance;
        [SerializeField] private float defReduction;
        [SerializeField] private float atkReduction;
        [SerializeField] private int duration;
        
        public void OnAttack(Unit unit, Unit target)
        {
            if (!(Random.Range(0f, 1f) < chance)) return;
            target.AddEffect(new Debuff(duration, defReduction, atkReduction));
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