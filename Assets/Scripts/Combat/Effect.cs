using UnityEngine;

namespace Combat
{
    [CreateAssetMenu(menuName = "Game/New Effect", fileName = "New Effect")]
    public class Effect : ScriptableObject
    {
        [Tooltip("If 0 <= x <= 1 then it is a treated as a multiplier, otherwise as an additive value")]
        [field: SerializeField]
        public Stats BonusOrMultiplier { get; private set; }

        [field: SerializeField] public float Duration { get; private set; }

        public Stats CalculateBonus(Stats baseStats)
        {
            return new Stats(
                Calc(BonusOrMultiplier.Attack, baseStats.Attack),
                Calc(BonusOrMultiplier.Defence, baseStats.Defence),
                Calc(BonusOrMultiplier.MaxHp, baseStats.MaxHp),
                Calc(BonusOrMultiplier.Speed, baseStats.Speed),
                Calc(BonusOrMultiplier.CritRate, baseStats.CritRate),
                Calc(BonusOrMultiplier.CritMult, baseStats.CritMult)
            );
        }

        private float Calc(float f, float b)
        {
            return f > 1 ? f : f * b;
        }
    }
}