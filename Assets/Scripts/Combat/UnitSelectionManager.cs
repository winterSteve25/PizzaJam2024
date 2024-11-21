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

        public static event Action<Unit> UnitSelected; 

        private Unit _selectedUnit;

        private Unit SelectedUnit
        {
            get => _selectedUnit;
            set
            {
                _selectedUnit = value;
                UnitSelected?.Invoke(_selectedUnit);
            }
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