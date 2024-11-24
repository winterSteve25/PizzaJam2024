using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Worlds
{
    [Serializable]
    public class Rectangle
    {
        public Vector2Int offset;
        public Vector2Int size;

        public void DrawGizmos()
        {
            var world = Object.FindFirstObjectByType<World>();
            World.DrawBound(world.CellToWorld(offset, size), size);
        }

        public Vector2Int GetRandomPoint(Vector2Int minSize)
        {
            var lst = new List<Vector2Int>();
            
            for (int i = 0; i < size.x; i++)
            {
                for (int j = 0; j < size.y; j++)
                {
                    var p = new Vector2Int(i, j) + offset;
                    var ok = true;

                    for (int k = 0; k < minSize.x; k++)
                    {
                        for (int l = 0; l < minSize.y; l++)
                        {
                            if (World.Current.AreaSelection.PassableAndEmpty(p + new Vector2Int(k, l),
                                    Vector2Int.zero)) continue;
                            ok = false;
                            goto next;
                        }
                    }
                    
                    next:
                    if (ok)
                    {
                        lst.Add(p);
                    }
                }
            }

            return lst[Random.Range(0, lst.Count)];
        }
    }

    [Serializable]
    public class RectangleCollection
    {
        public Rectangle[] rects;

        public bool IsIn(Vector2Int position)
        {
            return rects.Any(rect =>
                position.x >= rect.offset.x && position.y >= rect.offset.y &&
                position.x < rect.offset.x + rect.size.x && position.y < rect.offset.y + rect.size.y);
        }

        public Vector2Int GetRandomPoint(Vector2Int size)
        {
            return rects[Random.Range(0, rects.Length)].GetRandomPoint(size);
        }

        public void DrawGizmos()
        {
            foreach (var rect in rects)
            {
                rect.DrawGizmos();
            }
        }
    }
}