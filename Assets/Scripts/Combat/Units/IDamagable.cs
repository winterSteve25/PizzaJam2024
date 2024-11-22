namespace Combat.Units
{
    public interface IDamagable
    {
        void Hurt(float raw, int level);

        void Heal(float raw);

        void HealPercentage(float percentage);
    }
}