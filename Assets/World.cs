using System;
using System.Collections.Generic;
using UnityEngine;

public class World
{
    private Dictionary<Vector2Int, Chunk> Chunks { get; } = new();

    public Chunk GetChunk(int x, int y)
    {
        if (Chunks.TryGetValue(new Vector2Int(x, y), out var chunk))
        {
            return chunk;
        }

        return null;
    }
    public Chunk RequestChunk(int x, int y)
    {
        var chunk = GetChunk(x, y);
        if (chunk == null)
        {
            return GenerateChunk(x, y); 
        }

        return chunk;
    }

    public Block GetBlock(int x, int y, int z)
    {
        var chunkX = (int)Math.Floor((float)x / 16);
        var chunkY = (int)Math.Floor((float)y / 16);
        var chunk = GetChunk(chunkX, chunkY);
        if (chunk == null)
        {
            return null;
        }
        
        return chunk.GetBlock(new Vector3Int(x % 16, y % 16, z));
    }
    
    private Chunk GenerateChunk(int x, int y)
    {
        var chunk = new Chunk();
        chunk.AttachToWorld(this, x, y);
        
        Chunks[new Vector2Int(x, y)] = chunk;
        return chunk;
    }
}