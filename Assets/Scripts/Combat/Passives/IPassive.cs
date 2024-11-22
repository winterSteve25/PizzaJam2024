using Combat.Units;

namespace Combat.Passives
{
    public interface IPassive
    {
        void OnNewTurn(Unit unit);
    }
}