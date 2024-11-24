using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Combat.Units;
using Combat.Units.Shaman;
using UnityEngine;

namespace Combat
{
    public class CombatManager : MonoBehaviour
    {
        public static CombatManager Current { get; private set; }

        public event Action<Unit> OnTurnChanged;
        public event Action<int> OnRoundChanged; 

        private LinkedList<int> _unitsTurns;
        private List<Unit> _units;
        private int _round;
        private int _acting;

        private void Awake()
        {
            Current = this;
            _units = new List<Unit>();
        }

        private void Start()
        {
            _unitsTurns = new LinkedList<int>();
            _acting = -1;
            _round = 0;
        }

        private void OnDestroy()
        {
            OnRoundChanged = null;
            OnTurnChanged = null;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StartNewRound();
            }
        }

        private void StartNewRound()
        {
            if (_units.Count <= 0) return;
            
            _round++;
            _units.Sort((a, b) => b.CurrentStats.Speed.CompareTo(a.CurrentStats.Speed));

            for (var i = 0; i < _units.Count; i++)
            {
                _unitsTurns.AddFirst(i);
            }

            OnRoundChanged?.Invoke(_round);
            NextTurn();
        }

        public void NextTurn()
        {
            if (_unitsTurns.Count <= 0)
            {
                _acting = -1;
                StartNewRound();
                return;
            }

            var first = _unitsTurns.First.Value;
            _unitsTurns.RemoveFirst();
            Act(first);
        }

        private void Act(int i)
        {
            _acting = i;
            OnTurnChanged?.Invoke(_units[_acting]);
            _units[_acting].TurnStarted();
            UnitSelectionManager.Current.SelectedUnit = _units[_acting];
            UnitSelectionManager.Current.overriden = true;
            StartCoroutine(RemoveOverride());
        }

        private IEnumerator RemoveOverride()
        {
            yield return new WaitForEndOfFrame();
            UnitSelectionManager.Current.overriden = false;
        }

        public void AddUnit(Unit unit)
        {
            _units.Add(unit);
        }

        public bool IsTurnOf(Unit unit)
        {
            if (_acting == -1) return false;
            return unit == _units[_acting];
        }

        public void PushToNext(Unit unit)
        {
            var i = _units.FindIndex(u => u == unit);
            _unitsTurns.AddFirst(i);
        }

        public IEnumerable<Unit> GetEnemies()
        {
            return _units.Where(u => !u.IsAlly());
        }

        public IEnumerable<Unit> GetAllies()
        {
            return _units.Where(u => u.IsAlly());
        }

        public void TakeOver(Unit unit)
        {
            var i = _units.FindIndex(u => u == unit);

            _unitsTurns.AddFirst(_acting);
            Act(i);
        }
    }
}