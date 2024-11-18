using System;
using System.Collections;
using System.Collections.Generic;
using Combat.Units;
using UnityEngine;

namespace Combat
{
    public class CombatManager : MonoBehaviour
    {
        public static CombatManager Current { get; private set; }
        
        private Queue<int> _unitsTurns;
        [SerializeField] private List<Unit> _units;
        [SerializeField] private int _round;
        [SerializeField] private int _acting;

        private void Awake()
        {
            Current = this;
            _units = new List<Unit>();
        }

        private void Start()
        {
            _unitsTurns = new Queue<int>();
            _acting = 0;
            _round = 0;
            StartNewRound();
        }

        private void StartNewRound()
        {
            _round++;
            _units.Sort((a, b) => a.CurrentStats.Speed.CompareTo(b.CurrentStats.Speed));

            for (var i = 0; i < _units.Count; i++)
            {
                _unitsTurns.Enqueue(i);
            }
        }

        public void AddUnit(Unit unit)
        {
            _units.Add(unit);
        }

        public bool IsTurnOf(Unit unit)
        {
            return unit == _units[_acting];
        }
    }
}