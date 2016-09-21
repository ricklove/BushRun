using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TileSprites : MonoBehaviour
{
    public TileDirection TileDirection = TileDirection.Horizontal;
    public Sprite[] tileSprites = null;
    private List<GameObject> tiles = null;
    private List<GameObject> clones = new List<GameObject>();
    private List<GameObject> remaining = new List<GameObject>();

    void LateUpdate()
    {
        remaining = clones.ToList();

        TileForPreCull(MainModel.Instance.CameraModel.GameObject.GetComponent<Camera>());

        foreach (var r in remaining)
        {
            r.SetActive(false);
        }
    }

    void TileForPreCull(Camera cam)
    {
        // Get camera frustum in local coords
        var distance = Mathf.Abs(cam.transform.position.z);

        var frustumHeight = 2.0f * distance * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
        var frustumWidth = frustumHeight * cam.aspect;

        if (cam.orthographic)
        {
            var minSize = cam.orthographicSize * 2;
            if (cam.aspect > 1)
            {
                frustumHeight = minSize;
                frustumWidth = frustumHeight * cam.aspect;
            }
            else
            {
                frustumWidth = minSize;
                frustumHeight = frustumHeight / cam.aspect;
            }
        }

        var fx = cam.transform.position.x - frustumWidth * 0.5f;
        var fy = cam.transform.position.y - frustumHeight * 0.5f;

        // Go past the edges a bit
        var min = transform.InverseTransformPoint(new Vector3(fx - frustumWidth * 0.1f, fy - frustumHeight * 0.1f, 0));
        var max = transform.InverseTransformPoint(new Vector3(fx + frustumWidth * 1.1f, fy + frustumHeight * 1.1f, 0));

        // Get sprite local coords
        var sprite = GetComponent<SpriteRenderer>();
        var spriteWorldMin = sprite.bounds.min;
        var spriteWorldMax = sprite.bounds.max;

        var localMin = transform.InverseTransformPoint(spriteWorldMin);
        var localMax = transform.InverseTransformPoint(spriteWorldMax);
        var localSize = localMax - localMin;

        // Repeat tiles inside camera using local coords

        // Horizontal
        if (TileDirection == TileDirection.Horizontal)
        {
            var width = localSize.x;
            var x = localMin.x;

            var minX = min.x;
            var maxX = max.x;

            PositionTiles(x, width, new Vector3(width, 0, 0), minX, maxX, remaining);
        }
    }

    void PositionTiles(float val, float change, Vector3 vectChange, float min, float max, List<GameObject> remaining)
    {
        // Create prefab if needed
        if (tiles == null)
        {
            tiles = new List<GameObject>();
            if (tileSprites == null
                || tileSprites.Length == 0)
            {

                var sprite = GetComponent<SpriteRenderer>();
                tileSprites = new Sprite[] { sprite.sprite };
            }

            foreach (var tileSprite in tileSprites)
            {
                var prefab = new GameObject();
                SpriteRenderer childSprite = prefab.AddComponent<SpriteRenderer>();
                childSprite.sprite = tileSprite;

                var sprite = GetComponent<SpriteRenderer>();
                childSprite.sortingOrder = sprite.sortingOrder;
                prefab.layer = transform.gameObject.layer;

                prefab.name = "Tile" + tiles.Count;

                prefab.transform.parent = transform;
                prefab.transform.localPosition = new Vector3();
                prefab.transform.localScale = new Vector3(1, 1, 1);

                prefab.SetActive(false);
                sprite.enabled = false;

                tiles.Add(prefab);
            }

        }

        // Get positions
        var positions = GetTilePositions(val, change, tileSprites.Length, 0, min, max);

        foreach (var p in positions)
        {
            var targetTile = tiles[p.TileIndex];
            var targetPos = new Vector3(p.Position, 0, 0);

            // Check for one already displayed here
            var matches = clones.Where(cl => cl.transform.localPosition == targetPos && targetTile.name == cl.name);
            if (matches.Any())
            {
                var m = matches.First();
                m.SetActive(true);
                remaining.Remove(m);
                continue;
            }


            var c = remaining.LastOrDefault(cl => cl.name == targetTile.name);

            if (c == null)
            {
                // Generate a child prefab of the sprite renderer
                c = ((Transform)Instantiate(targetTile.transform)).gameObject;
                c.name = targetTile.name;
                clones.Add(c);
                c.transform.parent = transform;
                c.transform.localScale = new Vector3(1, 1, 1);
            }
            else
            {
                remaining.Remove(c);
            }

            c.transform.localPosition = targetPos;
            c.SetActive(true);
        }
    }

    public static IList<TilePosition> GetTilePositions(float tileOrigin, float tileSize, int tileCount, float overlap, float min, float max)
    {
        // Go back enough 
        var d = tileSize - overlap;

        var p = tileOrigin;
        var i = 0;

        // Go forward enough
        while (p <= (max + tileSize))
        {
            p += d;
            i++;
            i %= tileCount;
        }

        // Go back behind
        while (p >= (min - tileSize))
        {
            p -= d;
            i--;
            i += tileCount;
            i %= tileCount;
        }

        // Start
        var positions = new List<TilePosition>();

        while (p <= (max + tileSize))
        {
            // Add position
            positions.Add(new TilePosition() { Position = p, Size = tileSize, TileIndex = i });

            p += d;
            i++;
            i %= tileCount;
        }

        return positions;
    }

    public class TilePosition
    {
        public float Position;
        public float Size;
        public int TileIndex;

        public override string ToString()
        {
            return string.Format("{0} @ {1}", TileIndex, Position);
        }
    }
}

public enum TileDirection
{
    None = 0,
    Horizontal = 1,
    Vertical = 2,
    //Both=3, 
}