using System;
using System.Collections.Generic;
using UnityEngine;

public class MeshBuffer
{
    public readonly List<Vector3> vertices = new();
    public readonly List<int> triangles = new();
    public readonly List<Vector2> uv = new();

    public void AddTopQuad(Vector3 offset, int tileId)
    {
        var tr = AddVertex(new Vector3(1, 1, 0) + offset);
        var tl = AddVertex(new Vector3(1, 1, 1) + offset);
        var br = AddVertex(new Vector3(0, 1, 1) + offset);
        var bl = AddVertex(new Vector3(0, 1, 0) + offset);
        
        triangles.AddRange(new []
        {
            tl, tr, bl,
            tl, bl, br
        });
        
        var y = (int)Math.Floor((float)tileId / 8);
        var x = tileId % 8;
        
        uv.AddRange(SelectTile(x, y));
    }

    public void AddBottomQuad(Vector3 offset, int tileId)
    {
        var tl = AddVertex(new Vector3(1, 0, 0) + offset);
        var tr = AddVertex(new Vector3(1, 0, 1) + offset);
        var bl = AddVertex(new Vector3(0, 0, 1) + offset);
        var br = AddVertex(new Vector3(0, 0, 0) + offset);
         
        triangles.AddRange(new []
        {
            tl, tr, bl,
            tl, bl, br
        });
        
        var y = (int)Math.Floor((float)tileId / 8);
        var x = tileId % 8;
        
        uv.AddRange(SelectTile(x, y));
    }
    
    /// <summary>
    /// -x
    /// </summary>
    /// <param name="offset"></param>
    /// <param name="tileId"></param>
    public void AddFrontQuad(Vector3 offset, int tileId)
    {
        var tl = AddVertex(new Vector3(0, 0, 1) + offset);
        var tr = AddVertex(new Vector3(0, 1, 1) + offset);
        var bl = AddVertex(new Vector3(0, 1, 0) + offset);
        var br = AddVertex(new Vector3(0, 0, 0) + offset);
         
        triangles.AddRange(new []
        {
            tl, tr, bl,
            tl, bl, br
        });
        
        var y = (int)Math.Floor((float)tileId / 8);
        var x = tileId % 8;
        
        uv.AddRange(SelectTile(x, y));
    }
    
    /// <summary>
    /// +x
    /// </summary>
    /// <param name="offset"></param>
    /// <param name="tileId"></param>
    public void AddBackQuad(Vector3 offset, int tileId)
    {
        var tl = AddVertex(new Vector3(1, 0, 0) + offset);
        var tr = AddVertex(new Vector3(1, 1, 0) + offset);
        var bl = AddVertex(new Vector3(1, 1, 1) + offset);
        var br = AddVertex(new Vector3(1, 0, 1) + offset);
         
        triangles.AddRange(new []
        {
            tl, tr, bl,
            tl, bl, br
        });

        var y = (int)Math.Floor((float)tileId / 8);
        var x = tileId % 8;
        
        uv.AddRange(SelectTile(x, y));
    }
    
    /// <summary>
    /// -z
    /// </summary>
    /// <param name="offset"></param>
    /// <param name="tileId"></param>
    public void AddRightQuad(Vector3 offset, int tileId)
    {
        var tl = AddVertex(new Vector3(0, 0, 0) + offset);
        var tr = AddVertex(new Vector3(0, 1, 0) + offset);
        var bl = AddVertex(new Vector3(1, 1, 0) + offset);
        var br = AddVertex(new Vector3(1, 0, 0) + offset);
         
        triangles.AddRange(new []
        {
            tl, tr, bl,
            tl, bl, br
        });
        
        var y = (int)Math.Floor((float)tileId / 8);
        var x = tileId % 8;
        
        uv.AddRange(SelectTile(x, y));
    }
    
    /// <summary>
    /// +z
    /// </summary>
    /// <param name="offset"></param>
    /// <param name="tileId"></param>
    public void AddLeftQuad(Vector3 offset, int tileId)
    {
        var tl = AddVertex(new Vector3(1, 0, 1) + offset);
        var tr = AddVertex(new Vector3(1, 1, 1) + offset);
        var bl = AddVertex(new Vector3(0, 1, 1) + offset);
        var br = AddVertex(new Vector3(0, 0, 1) + offset);
         
        triangles.AddRange(new []
        {
            tl, tr, bl,
            tl, bl, br
        });

        var y = (int)Math.Floor((float)tileId / 8);
        var x = tileId % 8;
        
        uv.AddRange(SelectTile(x, y));
    }
    
    private Vector2[] SelectTile(int x, int y)
    {
        var tileX = 1f / 8;
        var tileY = 1f / 8;

        var tile = new Vector2[4];
        tile[1] = new Vector2(tileX * x, 1 - tileY * y); // bottom left
        tile[0] = new Vector2(tileX * x, 1 - tileY * (y + 1)); // top left
        tile[3] = new Vector2(tileX * (x + 1), 1 - tileY * (y + 1)); // top right
        tile[2] = new Vector2(tileX * (x + 1), 1 - tileY * y); // bottom right
        return tile;
    }

    private int AddVertex(Vector3 v)
    {
        // oops doesnt work yet
        // if (vertexSet.Contains(v))
        //     return vertices.IndexOf(v);
        
        vertices.Add(v);
        //vertexSet.Add(v);
        return vertices.Count - 1; 
    }
}