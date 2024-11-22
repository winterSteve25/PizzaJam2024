using Combat.Units;

namespace Combat.Passives
{
    public interface IPassive
    {
        void OnStartBattle(Unit unit) {}
        
        void OnNewTurn(Unit unit) {}

        void OnAttack(Unit unit, Unit target) {}

        void OnDamaged(Unit unit, Unit attacker) {}
    }
}