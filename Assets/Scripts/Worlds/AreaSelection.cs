using System;
using System.Collections.Generic;
using System.Linq;
using Combat.Units;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace Worlds
{
    public class AreaSelection : MonoBehaviour
    {
        public delegate bool ShapePredicate(Vector2Int position, Vector2Int origin);

        public delegate Vector2Int LandingFunc(Vector2Int position, Vector2Int origin, Unit selectedUnit);

        [SerializeField] private TileBase[] unpassableTiles;
        [SerializeField] private Tilemap areaIndication;
        [SerializeField] private Tilemap selectedIndication;
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private Camera cam;

        [SerializeField] private TileBase rangeTile;
        [SerializeField] private TileBase failTile;
        [SerializeField] private TileBase pointerTile;
        [SerializeField] private TileBase killTile;
        [SerializeField] private TileBase supportTile;

        public bool IsPicking { get; private set; }

        private Action<Vector2Int, Vector2Int> _areaCallback;
        private Action<Unit, Vector2Int> _unitCallback;
        private LandingFunc _landingPosition;

        private Vector2Int _size;
        private Vector2Int _origin;
        private ShapePredicate _shape;
        private ShapePredicate _valid;
        private bool _rotatable;
        private bool _pickAlly;

        private void Update()
        {
            if (_areaCallback == null && _unitCallback == null)
            {
                IsPicking = false;
                return;
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                IsPicking = false;
                _areaCallback = null;
                _unitCallback = null;
                areaIndication.ClearAllTiles();
                selectedIndication.ClearAllTiles();
                return;
            }

            IsPicking = true;

            var mp = Input.mousePosition;
            mp = cam.ScreenToWorldPoint(mp);

            var dir = (Vector2Int)World.Current.GetGridPosOfObject(mp, Vector2.one) - _origin;
            var switchAxis = _rotatable && Mathf.Abs(dir.x) > Mathf.Abs(dir.y);
            var size = switchAxis ? new Vector2Int(_size.y, _size.x) : _size;

            var mpGridPos = World.Current.GetGridPosOfObject(mp, size);
            var pos = World.Current.SnapToGrid(mp, size);

            pos.x += size.x * -0.5f + 0.03f;
            pos.y += size.y * -0.5f + 0.03f;

            selectedIndication.transform.DOMove(pos, 0.2f)
                .SetEase(Ease.OutCubic);

            var gridPosAtMouse = (Vector2Int)mpGridPos;
            if (_areaCallback != null)
            {
                bool success = true;

                selectedIndication.ClearAllTiles();

                for (int i = 0; i < size.x; i++)
                {
                    for (int j = 0; j < size.y; j++)
                    {
                        var offsetted = new Vector2Int(i + gridPosAtMouse.x, j + gridPosAtMouse.y);
                        var tile = pointerTile;
                        if (!_shape(offsetted, _origin)) tile = failTile;
                        if (!_valid(offsetted, _origin)) tile = failTile;
                        selectedIndication.SetTile(new Vector3Int(i, j), tile);
                        if (tile == pointerTile) continue;
                        success = false;
                    }
                }

                if (!success)
                {
                    return;
                }

                if (Input.GetMouseButtonDown(0))
                {
                    _areaCallback(gridPosAtMouse, size);
                    _areaCallback = null;

                    areaIndication.ClearAllTiles();
                    selectedIndication.ClearAllTiles();
                }
            }

            if (_unitCallback == null) return;

            bool found = World.Current.GetUnitAt(gridPosAtMouse, out var unit);
            bool valid = found && (_pickAlly ? unit.IsAlly() : !unit.IsAlly());

            selectedIndication.ClearAllTiles();
            selectedIndication.SetTile(
                Vector3Int.zero,
                !found || !_shape(gridPosAtMouse, _origin)
                    ? failTile
                    : _pickAlly
                        ? unit.IsAlly() ? supportTile : failTile
                        : unit.IsAlly()
                            ? failTile
                            : killTile
            );

            if (!valid) return;
            if (_landingPosition != null)
            {
                selectedIndication.SetTile(
                    (Vector3Int)(_landingPosition(gridPosAtMouse, _origin, unit) - gridPosAtMouse),
                    pointerTile
                );
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (!_shape(gridPosAtMouse, _origin))
                {
                    return;
                }

                if (_landingPosition == null)
                {
                    _unitCallback(unit, _origin);
                }
                else
                {
                    _unitCallback(unit, _landingPosition(gridPosAtMouse, _origin, unit));
                }

                _unitCallback = null;

                areaIndication.ClearAllTiles();
                selectedIndication.ClearAllTiles();
            }
        }

        public void Select(
            int range,
            Vector3 origin,
            Vector2Int size,
            Action<Vector2Int, Vector2Int> callback,
            ShapePredicate shape,
            ShapePredicate valid,
            bool rotatable = true
        )
        {
            Select(range, (Vector2Int)World.Current.TileMap.WorldToCell(origin), size, callback, shape, valid,
                rotatable);
        }

        private void Select(
            int range,
            Vector2Int origin,
            Vector2Int size,
            Action<Vector2Int, Vector2Int> callback,
            ShapePredicate shape,
            ShapePredicate valid,
            bool rotatable = true
        )
        {
            SetUpSelect(range, origin, size, shape);

            _size = size;
            _areaCallback = callback;
            _origin = origin;
            _shape = shape;
            _valid = valid;
            _rotatable = rotatable;
        }

        public void SelectUnit(int range, Vector3 origin, bool ally, Action<Unit, Vector2Int> callback,
            ShapePredicate shape,
            LandingFunc landingPosition = null)
        {
            var og = (Vector2Int)World.Current.TileMap.WorldToCell(origin);
            SetUpSelect(range, og, Vector2Int.one, shape);

            _unitCallback = callback;
            _origin = og;
            _shape = shape;
            _pickAlly = ally;
            _landingPosition = landingPosition;
            _size = Vector2Int.one;
        }

        private void SetUpSelect(int range, Vector2Int origin, Vector2Int size, ShapePredicate shape)
        {
            areaIndication.ClearAllTiles();
            selectedIndication.ClearAllTiles();

            var halfRange = Mathf.FloorToInt(range * 0.5f);

            for (int i = -halfRange; i <= halfRange; i++)
            {
                for (int j = -halfRange; j <= halfRange; j++)
                {
                    var pos = new Vector2Int(i, j) + origin;
                    if (!shape(pos, origin)) continue;
                    if (!Passable(pos, origin)) continue;
                    areaIndication.SetTile(new Vector3Int(pos.x, pos.y, 0), rangeTile);
                }
            }

            for (int i = 0; i < size.x; i++)
            {
                for (int j = 0; j < size.y; j++)
                {
                    var pos = new Vector3Int(i, j, 0);
                    selectedIndication.SetTile(pos, failTile);
                }
            }
        }

        public void PreviewArea(int range, Vector3 origin, ShapePredicate shape)
        {
            SetUpSelect(range, (Vector2Int)World.Current.TileMap.WorldToCell(origin), Vector2Int.zero, shape);
        }

        public Vector2Int PickRandomPoint(int range, Vector2Int origin, Vector2Int size, ShapePredicate shape)
        {
            var halfRange = Mathf.FloorToInt(range * 0.5f);
            var lst = new List<Vector2Int>();
            
            for (int i = -halfRange; i <= halfRange; i++)
            {
                for (int j = -halfRange; j <= halfRange; j++)
                {
                    var pos = new Vector2Int(i, j) + origin;
                    if (!shape(pos, origin)) continue;
                    if (!Passable(pos, origin)) continue;
                    lst.Add(pos);
                }
            }

            return lst[Random.Range(0, lst.Count)];
        }

        public void RemovePreview()
        {
            areaIndication.ClearAllTiles();
        }

        public static ShapePredicate Circle(int range)
        {
            return (position, origin) => (origin - position).magnitude < (range * 0.5f);
        }

        public static ShapePredicate Rect()
        {
            return (_, _) => true;
        }

        public bool Passable(Vector2Int p, Vector2Int origin)
        {
            var tile = World.Current.TileMap.GetTile(new Vector3Int(p.x, p.y, 0));
            return tile == null || !unpassableTiles.Contains(tile) && World.Current.PlayableArea.IsIn(p);
        }

        public bool PassableAndEmpty(Vector2Int p, Vector2Int origin)
        {
            return Passable(p, origin) && !World.Current.GetUnitAt(p, out _);
        }
    }
}