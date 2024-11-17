using System.Collections.Generic;
using Combat;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Worlds
{
    public class World : MonoBehaviour
    {
        public static World Current { get; private set; }

        [field: SerializeField] public Tilemap TileMap { get; private set; }

        private Dictionary<Vector2Int, Unit> _units;

        private void OnValidate()
        {
            Current = this;
        }

        private void Awake()
        {
            Current = this;
            _units = new Dictionary<Vector2Int, Unit>();
        }

        public bool GetUnitAt(Vector2Int pos, out Unit unit)
        {
            return _units.TryGetValue(pos, out unit);
        }

        public bool MoveUnit(Vector2Int was, Vector2Int now)
        {
            if (!_units.TryGetValue(was, out var unit))
            {
                return false;
            }

            for (int i = 0; i < unit.Size.x; i++)
            {
                for (int j = 0; j < unit.Size.y; j++)
                {
                    _units.Remove(new Vector2Int(was.x + i, was.y + j));
                }
            }
            
            if (_units.ContainsKey(now))
            {
                return false;
            }

            AddUnit(now, unit);
            return true;
        }

        public void AddUnit(Vector2Int pos, Unit unit)
        {
            for (int i = 0; i < unit.Size.x; i++)
            {
                for (int j = 0; j < unit.Size.y; j++)
                {
                    _units.Add(new Vector2Int(pos.x + i, pos.y + j), unit);
                }
            }
        }
        
        public void AddUnit(Vector3 worldPos, Unit unit)
        {
            var pos = GetGridPosOfObject(worldPos, unit.Size);
            
            for (int i = 0; i < unit.Size.x; i++)
            {
                for (int j = 0; j < unit.Size.y; j++)
                {
                    _units.Add(new Vector2Int(pos.x + i, pos.y + j), unit);
                }
            }
        }

        public Vector2 ClosestGridLocation(Vector3 position, Vector2 size)
        {
            var worldPos = TileMap.CellToWorld(GetGridPosOfObject(position, size));

            worldPos.x += size.x * 0.5f;
            worldPos.y += size.y * 0.5f;
            
            return worldPos;
        }

        public Vector3Int GetGridPosOfObject(Vector3 position, Vector2 size)
        {
            var cellSize = TileMap.cellSize;
            var corner = position;

            corner.x -= size.x * 0.5f;
            corner.y -= size.y * 0.5f;
            corner.x += cellSize.x * 0.5f;
            corner.y += cellSize.y * 0.5f;

            return TileMap.WorldToCell(corner);
        }
    }
}