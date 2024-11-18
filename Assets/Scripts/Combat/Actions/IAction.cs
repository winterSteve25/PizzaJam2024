using Combat.Units;
using Worlds;

namespace Combat.Actions
{
    public interface IAction
    {
        string Name { get; }
        
        void Act(World world, Unit unit);
    }
}