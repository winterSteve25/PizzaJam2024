namespace Combat.Units
{
    public class EnemyUnit : Unit
    {
        public override bool IsAlly()
        {
            return false;
        }
    }
}