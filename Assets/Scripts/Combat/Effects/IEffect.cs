using Combat.Units;

namespace Combat.Effects
{
    public interface IEffect
    {
        public int Duration { get; }

        public Stats CalculateBonus(Stats baseStats)
        {
            return new Stats(); 
        }

        public void OnNewTurn(Unit unit) {}
    }
}