using Combat.Units;

namespace Combat.Passives
{
    public interface IPassive
    {
        void OnStartBattle(Unit unit) {}
        
        void OnNewTurn(Unit unit) {}

        void OnAttack(Unit unit, Unit target, bool didCrit) {}

        void OnDamaged(Unit unit) {}
    }
}