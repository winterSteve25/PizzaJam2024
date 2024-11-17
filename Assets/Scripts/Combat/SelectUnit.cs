using UnityEngine;
using UnityEngine.EventSystems;
using Worlds;

namespace Combat
{
    public class SelectUnit : MonoBehaviour
    {
        [SerializeField] private Camera cam;

        private Unit _selectedUnit;
        
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

            if (!World.Current.GetUnitAt(new Vector2Int(gridPos.x, gridPos.y), out var unit))
            {
                if (_selectedUnit != null) _selectedUnit.Unselect();
                _selectedUnit = null;
                return;
            }

            if (_selectedUnit != null)
            {
                _selectedUnit.Unselect();

                if (_selectedUnit == unit)
                {
                    _selectedUnit = null;
                    return;
                }
            }
            
            unit.Select();
            _selectedUnit = unit;
        }
    }
}