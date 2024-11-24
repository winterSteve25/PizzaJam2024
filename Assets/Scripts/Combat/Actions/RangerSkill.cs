using Combat.Effects;
using Combat.Units;
using UnityEngine;
using Worlds;

namespace Combat.Actions
{
    public class RangerSkill : MonoBehaviour, IAction
    {
        public string Name => "Trust the Wind";

        public string Description =>
            $"Gains 'Swift Hunter' for {duration} turns. When 'Swift Hunter' is active, this unit turns invisible and can not be targeted and gains {critRateBonus * 100}% CRIT Chance." +
            $"In addition, during 'Swift Hunter' this unit will have an {chanceToActAgain * 100}% chance to act again after landing a normal attack." +
            $"After using this skill, the unit can immediately act again but will become unavailable while 'Swift Hunter' is active.";

        [SerializeField] private int duration;
        [SerializeField] private float chanceToActAgain;
        [SerializeField] private float critRateBonus;

        public void Act(World world, Unit unit, CombatManager combatManager)
        {
            unit.AddEffect(new SwiftHunter(duration, chanceToActAgain, critRateBonus));
            combatManager.PushToNext(unit);
            combatManager.NextTurn();
        }

        public void PreviewArea(World world, Unit unit)
        {
            world.AreaSelection.PreviewArea(1, unit.transform.position, AreaSelection.Rect());
        }

        private class SwiftHunter : IEffect
        {
            public int Duration { get; }

            private float _chance;
            private float _critRateBonus;

            public SwiftHunter(int duration, float chance, float critRateBonus)
            {
                Duration = duration;
                _chance = chance;
                _critRateBonus = critRateBonus;
            }

            public void OnAttack(Unit unit, Unit target, bool didCrit)
            {
                if (Random.Range(0, 1f) < _chance)
                    CombatManager.Current.PushToNext(unit);
            }

            public Stats CalculateBonus(Stats baseStats)
            {
                return new Stats(
                    0,
                    0,
                    0,
                    0,
                    _critRateBonus,
                    0,
                    0
                );
            }
        }
    }
}