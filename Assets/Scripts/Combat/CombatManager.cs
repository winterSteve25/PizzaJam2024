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

        public event Action<Unit> OnTurnChanged;
        public event Action<int> OnRoundChanged; 

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
                _unitsTurns.Enqueue(i);
            }

            OnRoundChanged?.Invoke(_round);
            NextTurn();
        }

        public void NextTurn()
        {
            if (!_unitsTurns.TryDequeue(out var unit))
            {
                _acting = -1;
                StartNewRound();
                return;
            }
            
            Debug.Log($"{_units[unit].UnitName}({unit})'s turn has started");
            
            _acting = unit;
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
    }
}