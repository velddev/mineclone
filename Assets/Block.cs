using System;
using UnityEngine;

[Serializable]
public class BlockProperties
{
    // tile ids for each face.
    public int TileTop { get; set; }
    public int TileBottom { get; set; }
    public int TileLeft { get; set; }
    public int TileRight { get; set; }
    public int TileFront { get; set; }
    public int TileBack { get; set; }
}

[Serializable]
public class Block
{
    public BlockProperties Properties { get; set; }
    // set by the chunk
    public Vector3Int Position { get; set; }
    public Chunk Chunk { get; set; }
    
    public void AttachToChunk(Chunk chunk, Vector3Int position)
    {
        Chunk = chunk;
        Position = position;
    }

    public void SetBlock(BlockProperties properties)
    {
        Properties = properties;
    }

    public Vector3Int GetWorldPosition()
    {
        return new Vector3Int
        {
            x = Chunk.Position.x * Chunk.ChunkSize.x + Position.x,
            y = Position.y,
            z = Chunk.Position.y * Chunk.ChunkSize.z + Position.z
        };
    }

    public void SetProperties(BlockProperties properties)
    {
        Properties = properties;
        Chunk.SetDirty();
    }
    
    public override string ToString()
    {
        return $"BLOCK(X={Position.x}, Y={Position.y}, Z={Position.z})::CHUNK(X={Chunk.Position.x}, Z={Chunk.Position.y})";
    }
}