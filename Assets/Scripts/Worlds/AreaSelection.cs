using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Tilemaps;

namespace Worlds
{
    public class AreaSelection : MonoBehaviour
    {
        [SerializeField] private TileBase[] unpassableTiles;
        [SerializeField] private GameObject areaEffectPrefab;
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private Transform areas;
        [SerializeField] private Camera cam;

        private IObjectPool<GameObject> _squares;
        private Action<Vector3Int> _callback;
        private List<GameObject> _activatedSquares;
        private Vector2Int _size;
        private int _range;
        private Vector2Int _origin;

        private void Start()
        {
            _squares = new ObjectPool<GameObject>(
                () => Instantiate(areaEffectPrefab, areas),
                (go) => go.gameObject.SetActive(true),
                (go) => go.gameObject.SetActive(false),
                Destroy,
                defaultCapacity: 25
            );

            _activatedSquares = new List<GameObject>();
        }

        private void Update()
        {
            if (_callback == null) return;
            
            var mp = Input.mousePosition;
            mp = cam.ScreenToWorldPoint(mp);

            if ((areas.transform.position - mp).sqrMagnitude < 1)
            {
                Debug.Log((areas.transform.position - mp).sqrMagnitude < 1);
                return;
            }

            var pos = World.Current.ClosestGridLocation(mp, _size);
            
            pos.x -= _size.x * 0.5f;
            pos.y -= _size.y * 0.5f;
            
            areas.transform.DOMove(pos, 0.2f)
                .SetEase(Ease.OutCubic);

            if (Input.GetMouseButtonDown(0))
            {
                var gridPos = World.Current.GetGridPosOfObject(mp, _size);

                if (((Vector2Int) gridPos - _origin).magnitude > _range)
                {
                    return;
                }
                
                _callback(gridPos);
                _callback = null;

                foreach (var sq in _activatedSquares)
                {
                    _squares.Release(sq);
                }
                
                _activatedSquares.Clear();
            }
        }

        public void Select(Vector3 origin, Vector2Int size, int range, Action<Vector3Int> callback)
        {
            Select((Vector2Int) World.Current.GetGridPosOfObject(origin, size), size, range, callback);
        }

        private void Select(Vector2Int origin, Vector2Int size, int range, Action<Vector3Int> callback)
        {
            areas.transform.position = Vector3.zero;
            
            for (int i = 0; i < size.x; i++)
            {
                for (int j = 0; j < size.y; j++)
                {
                    var pos = new Vector2(0.5f + i, 0.5f + j);
                    var sq = _squares.Get();
                    sq.transform.position = pos;
                    _activatedSquares.Add(sq);
                }
            }

            _size = size;
            _callback = callback;
            _range = range;
            _origin = origin;
        }
    }
}