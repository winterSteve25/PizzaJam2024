using Combat.Units;

namespace Combat.Effects
{
    public interface IEffect
    {
        public int Duration { get; }
        
        public Stats CalculateBonus(Stats baseStats);

        public void OnNewTurn(Unit unit);
    }
}