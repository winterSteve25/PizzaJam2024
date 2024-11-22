using DG.Tweening;
using UnityEngine;
using Worlds;

namespace UI
{
    public class CameraMovement : MonoBehaviour
    {
        [SerializeField] private Camera cam;
        [SerializeField] private float targetZoom;
        [SerializeField] private float sensitivity = 0.1f;
        [SerializeField] private float movementSpeed = 5;

        private float _boundLeft;
        private float _boundRight;
        private float _boundUp;
        private float _boundDown;

        private void Start()
        {
            targetZoom = cam.orthographicSize;

            var pos = World.Current.SnapToGrid(Vector3.zero, Vector2.one);
            transform.position = new Vector3(pos.x, pos.y, -10);

            var cellsX = Mathf.FloorToInt(World.Current.Size.x * 0.5f);
            var cellsY = Mathf.FloorToInt(World.Current.Size.y * 0.5f);

            if (cellsX == 0)
            {
                _boundLeft = 0.5f;
                _boundRight = 0.5f;
            }
            else
            {
                _boundLeft = World.Current.TileMap.CellToWorld(new Vector3Int(-cellsX, 0)).x;
                _boundRight = World.Current.TileMap.CellToWorld(new Vector3Int(cellsX, 0)).x +
                              World.Current.TileMap.cellSize.x;
            }

            if (cellsY == 0)
            {
                _boundUp = 0.5f;
                _boundDown = 0.5f;
            }
            else
            {
                _boundUp = World.Current.TileMap.CellToWorld(new Vector3Int(0, cellsY)).y +
                           World.Current.TileMap.cellSize.y;
                _boundDown = World.Current.TileMap.CellToWorld(new Vector3Int(0, -cellsY)).y;
            }
        }

        private void Update()
        {
            if (Mathf.Abs(targetZoom - cam.orthographicSize) > 0.1)
            {
                DOTween.To(() => cam.orthographicSize, x => cam.orthographicSize = x, targetZoom, 0.1f)
                    .SetEase(Ease.OutCubic);
            }

            if (Input.mouseScrollDelta.y != 0)
            {
                targetZoom -= Input.mouseScrollDelta.y * sensitivity;
                targetZoom = Mathf.Clamp(targetZoom, 5, 12);
            }

            var transformPosition = transform.position;
            var mov = new Vector3(
                Input.GetAxisRaw("Horizontal"),
                Input.GetAxisRaw("Vertical"),
                0
            );

            mov.Normalize();

            if (mov.sqrMagnitude <= 0)
            {
                return;
            }

            var pos = transformPosition + mov * (Time.deltaTime * movementSpeed);
            pos.x = Mathf.Clamp(pos.x, _boundLeft, _boundRight);
            pos.y = Mathf.Clamp(pos.y, _boundDown, _boundUp);
            transform.DOMove(pos, 0.2f)
                .SetEase(Ease.OutCubic);
        }
    }
}