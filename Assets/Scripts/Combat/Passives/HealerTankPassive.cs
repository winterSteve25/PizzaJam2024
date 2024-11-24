using Combat.Units;
using UnityEngine;

namespace Combat.Passives
{
    public class HealerTankPassive : MonoBehaviour, IPassive
    {
        [SerializeField] private float percentage;
        
        public void OnNewTurn(Unit unit)
        {
            unit.HealPercentage(percentage);
        }
    }
}