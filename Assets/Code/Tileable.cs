using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Tileable : MonoBehaviour
{
    public TileDirection TileDirection = TileDirection.Horizontal;
    public Sprite[] tileSprites = null;
    private List<GameObject> tiles = null;
    private List<GameObject> clones = new List<GameObject>();
    private List<GameObject> remaining = new List<GameObject>();

    void Update()
    {
        remaining = clones.ToList();

        var cameras = CameraHelper.Cameras;

        foreach (var cam in cameras)
        {
            // If visible in camera layer
            if ((cam.cullingMask & (1 << gameObject.layer)) > 0)
            {
                TileForPreCull(cam);
            }
        }

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
        
        if (cam.isOrthoGraphic)
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

        // Go forward as much as needed
        var iVal = 0;
        
        while (val < min)
        {
            val += change;
            iVal++;
        }
        
        // Go behind as much as needed
        while (val > min)
        {
            val -= change;
            iVal--;
        }
        
        // Move forward, laying children
        while (val < max)
        {
            var iTile = ((iVal % tiles.Count) + tiles.Count) % tiles.Count;
            var targetTile = tiles [iTile];
            var targetPos = vectChange * iVal;
            
            // Check for one already displayed here
            var matches = clones.Where(cl => cl.transform.localPosition == targetPos && targetTile.name == cl.name);
            if (matches.Any())
            {
                var m = matches.First();
                m.SetActive(true);
                remaining.Remove(m);
                
                val += change;
                iVal++;
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
                remaining.RemoveAt(remaining.Count - 1);
            }
            
            c.transform.localPosition = targetPos;
            c.SetActive(true);
            
            val += change;
            iVal++;
        }
        
        foreach (var r in remaining)
        {
            r.SetActive(false);
        }
        
    }

//    void TileForPreCull(Camera cam)
//    {
//        Debug.DrawLine(new Vector3(fx, fy, 0), new Vector3(fx + frustumWidth, fy + frustumHeight, 0));
//
//
//        // TODO: Implement logic for both
////        if (TileDirection == TileDirection.Both)
////        {
////            throw new UnityException("Both Not Implemented");
////        }
////        
//
//        var sprite = GetComponent<SpriteRenderer>();
//        var worldSize = new Vector2(sprite.bounds.size.x, sprite.bounds.size.y);
//        var worldPos = sprite.bounds.min;
//
//        var localSize = new Vector2(worldSize.x / transform.localScale.x, worldSize.y / transform.localScale.y);
//        var unscaledLocalPos = worldPos - transform.position;
//        var localPos = new Vector2(unscaledLocalPos.x / transform.localScale.x, unscaledLocalPos.y / transform.localScale.y);
//
//        // Horizontal
//        if (TileDirection == TileDirection.Horizontal)
//        {
//            // Fill frustum with children clones
//            var width = worldSize.x;
//            var x = worldPos.x;
//
//            var min = fx;
//            var max = fx + frustumWidth;
//
//            PositionTiles(x, width, new Vector3(width, 0, 0), min, max, remaining);
//            sprite.enabled = false;
//        }
//
////        // Vertical
////        if (TileDirection == TileDirection.Vertical)
////        {
////            // Fill frustum with children clones
////            var height = renderer.bounds.size.y;
////            var y = renderer.bounds.center.y - height * 0.5f;
////            
////            PositionTiles(y, height, new Vector3(0, height, 0), fy, fy + frustumHeight, remaining);
////        }
       
//    }

//    void PositionTiles(float val, float change, Vector3 vectChange, float min, float max, List<GameObject> remaining)
//    {
//        // Go forward as much as needed
//        var iVal = 0;
//
//        while (val < min)
//        {
//            val += change;
//            iVal++;
//        }
//
//        // Go behind as much as needed
//        while (val > min)
//        {
//            val -= change;
//            iVal--;
//        }
//        
//        // Move forward, laying children
//        while (val < max)
//        {
//            // Check for one already displayed here
//            var target = transform.localPosition + vectChange * iVal;
//            var matches = clones.Where(cl => cl.transform.localPosition == target && cl.activeSelf);
//            if (matches.Any())
//            {
//                var m = matches.First();
//                m.SetActive(true);
//                remaining.Remove(m);
//
//                val += change;
//                iVal++;
//                continue;
//            }
//
//
//            var c = remaining.LastOrDefault();
//            
//            if (c == null)
//            {
//                // Generate a child prefab of the sprite renderer
//                c = new GameObject();
//                SpriteRenderer childSprite = c.AddComponent<SpriteRenderer>();
//                c.transform.position = transform.position;
//
//                var sprite = GetComponent<SpriteRenderer>();
//                childSprite.sprite = sprite.sprite;
//                childSprite.sortingOrder = sprite.sortingOrder;
//                c.layer = transform.gameObject.layer;
//                c.name = name + "(" + clones.Count + ")";
//
//                clones.Add(c);
//                //c.transform.parent = transform;
//            } else
//            {
//                remaining.RemoveAt(remaining.Count - 1);
//            }
//
//            c.SetActive(true);
//
//            //c.transform.localScale = new Vector3(1, 1, 1);
//            //c.transform.localPosition = vectChange * iVal;
//            c.transform.localPosition = transform.localPosition + vectChange * iVal;
//            c.transform.localScale = transform.localScale;
//            
//            val += change;
//            iVal++;
//        }
//
//        foreach (var r in remaining)
//        {
//            r.SetActive(false);
//        }
//
//    }

}

public enum TileDirection
{
    None=0,
    Horizontal=1,
    Vertical=2,
    //Both=3, 
}