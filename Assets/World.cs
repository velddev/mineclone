using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

public class World
{
    public ConcurrentQueue<Chunk> DirtyChunks { get; } = new();

    private ConcurrentDictionary<Vector2Int, Chunk> Chunks { get; } = new();

    public Chunk GetChunk(int x, int y)
    {
        return Chunks.GetValueOrDefault(new Vector2Int(x, y));
    }

    public Chunk RequestChunk(int x, int y)
    {
        return Chunks.GetOrAdd(new Vector2Int(x, y), key => GenerateChunk(key.x, key.y));
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