using System;
using Combat.Units;
using UnityEngine;
using UnityEngine.EventSystems;
using Worlds;

namespace Combat
{
    public class UnitSelectionManager : MonoBehaviour
    {
        [SerializeField] private Camera cam;

        public static UnitSelectionManager Current { get; private set; }
        public static event Action<Unit> UnitSelected; 

        private Unit _selectedUnit;
        public bool overriden;

        public Unit SelectedUnit
        {
            get => _selectedUnit;
            set
            {
                _selectedUnit = value;
                UnitSelected?.Invoke(_selectedUnit);
            }
        }

        private void Awake()
        {
            Current = this;
        }

        private void OnDestroy()
        {
            UnitSelected = null;
        }

        private void Update()
        {
            if (!Input.GetMouseButtonDown(0))
            {
                return;
            }
            
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            if (World.Current.AreaSelection.IsPicking)
            {
                return;
            }

            if (overriden)
            {
                overriden = false;
                return;
            }
            
            var pos = cam.ScreenToWorldPoint(Input.mousePosition);
            var gridPos = World.Current.TileMap.WorldToCell(pos);
            
            if (!World.Current.GetUnitAt(new Vector2Int(gridPos.x, gridPos.y), out var unit) ||
                !unit.TryGetComponent(out Unit selectable))
            {
                SelectedUnit = null;
                return;
            }
            
            if (SelectedUnit != null)
            {
                if (SelectedUnit == selectable)
                {
                    SelectedUnit = null;
                    return;
                }
            }
            
            SelectedUnit = selectable;
        }
    }
}