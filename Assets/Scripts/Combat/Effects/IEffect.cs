namespace Combat.Effects
{
    public interface IEffect
    {
        public float Duration { get; protected set; }
        
        public Stats CalculateBonus(Stats baseStats);
    }
}