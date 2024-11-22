using Combat.Effects;
using Combat.Units;
using UnityEngine;

namespace Combat.Passives
{
    public class MeleePassive : MonoBehaviour, IPassive
    {
        [SerializeField] private float critMulBuff;
        [SerializeField] private float critChanceBuff;
        
        public void OnStartBattle(Unit unit)
        {
            unit.AddEffect(new StartBattleBuff(critMulBuff));
        }

        public void OnDamaged(Unit unit, Unit attacker)
        {
            unit.AddEffect(new ChanceBuff(critChanceBuff));
        }

        private class StartBattleBuff : IEffect
        {
            public int Duration => -1;

            private float _buff;

            public StartBattleBuff(float buff)
            {
                _buff = buff;
            }

            public Stats CalculateBonus(Stats baseStats)
            {
                return new Stats(
                    0,
                    0,
                    0,
                    0,
                    0,
                    _buff
                );
            }

            public void OnNewTurn(Unit unit)
            {
            }
        }

        private class ChanceBuff : IEffect
        {
            public int Duration => 2;

            private float _chance;

            public ChanceBuff(float chance)
            {
                _chance = chance;
            }

            public Stats CalculateBonus(Stats baseStats)
            {
                return new Stats(
                    0,
                    0,
                    0,
                    0,
                    _chance,
                    0
                );
            }
        }
    }
}