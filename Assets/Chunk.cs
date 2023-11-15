using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class Chunk
{
    public static Vector3Int ChunkSize = new (WIDTH, HEIGHT, DEPTH);
    
    const int WIDTH = 16;
    const int HEIGHT = 128;
    const int DEPTH = 16;

    Block[][][] Blocks { get; }
    private MeshBuffer buffer;
    public bool Dirty { get; private set; }
    public bool Generating { get; set; }
    private World world;
    public Vector2Int Position { get; private set; }
    [CanBeNull] private Mesh mesh = null;
    public ChunkAccessor ChunkAccessor { get; set; }

    public Chunk()
    {
        Blocks = new Block[WIDTH][][];
        for (var i = 0; i < WIDTH; i++)
        {
            Blocks[i] = new Block[HEIGHT][];
            for (var j = 0; j < HEIGHT; j++)
            {
                Blocks[i][j] = new Block[DEPTH];
                for (var b = 0; b < DEPTH; b++)
                {
                    Blocks[i][j][b] = new Block();
                    Blocks[i][j][b].AttachToChunk(this, new Vector3Int(i, j, b));
                }
            }
        }

        buffer = new MeshBuffer();
    }

    public void AttachToWorld(World world, int x, int y)
    {
        if (this.world != null)
        {
            throw new Exception("Already attached.");
        }

        this.world = world;
        Position = new Vector2Int(x, y);
    }
    
    public Mesh GetMesh()
    {
        if (mesh == null || Dirty)
        {
            mesh = CreateMesh();
            Dirty = false;
        }

        return mesh;
    }

    public Block GetBlock(int x, int y, int z) 
        => GetBlock(new Vector3Int(x, y, z));
    public Block GetBlock(Vector3Int position)
    {
        if (position.y < 0 || position.y >= HEIGHT)
            return null;

        if (position.x < 0 || position.x >= 16)
        {
            return null;
        }
        
        if (position.z < 0 || position.z >= 16)
        {
            return null;
        }

        return Blocks[position.x][position.y][position.z];
    }

    public void SetBlock(Vector3Int position, [CanBeNull] BlockProperties block)
        => SetBlock(position.x, position.y, position.z, block);

    public void SetBlock(int x, int y, int z, [CanBeNull] BlockProperties properties)
    {
        var block = Blocks[x][y][z];
        if (block.Properties == properties)
        {
            // ignore if the properties are the same
            return;
        }
        
        block.SetProperties(properties);

        if (x == 15)
        {
            world.GetChunk(Position.x + 1, Position.y)?.SetDirty();
        }
        
        if (x == 0)
        {
            world.GetChunk(Position.x - 1, Position.y)?.SetDirty();
        }
        
        if (z == 15)
        {
            world.GetChunk(Position.x, Position.y + 1)?.SetDirty();
        }
        
        if (z == 0)
        {
            world.GetChunk(Position.x, Position.y - 1)?.SetDirty();
        }
        
    }

    private Mesh CreateMesh()
    {
        buffer = new MeshBuffer();
        for (var x = 0; x < WIDTH; x++)
        {
            for (var y = 0; y < HEIGHT; y++)
            {
                for (var z = 0; z < DEPTH; z++)
                {
                    var block = Blocks[x][y][z];
                    if (block?.Properties == null)
                    {
                        continue;
                    }

                    var offset = new Vector3(x, y, z);

                    if (GetBlock(x, y + 1, z)?.Properties == null)
                    {
                        buffer.AddTopQuad(offset, block.Properties.TileTop);
                    }

                    if (GetBlock(x, y - 1, z)?.Properties == null)
                    {
                        buffer.AddBottomQuad(offset, block.Properties.TileBottom);
                    }

                    if (GetBlock(x + 1, y, z)?.Properties == null)
                    {
                        if (x == 15)
                        {
                            var neighbor = world.GetChunk(Position.x + 1, Position.y)?
                                .GetBlock(0, y, z);
                            if (neighbor?.Properties == null)
                            {
                                buffer.AddBackQuad(offset, block.Properties.TileBack);
                            }
                        }
                        else
                        {
                            buffer.AddBackQuad(offset, block.Properties.TileBack);
                        }
                    }

                    if (GetBlock(x - 1, y, z)?.Properties == null)
                    {
                        if (x == 0)
                        {
                            var neighbor = world.GetChunk(Position.x - 1, Position.y)?
                                .GetBlock(15, y, z);
                            if (neighbor?.Properties == null)
                            {
                                buffer.AddFrontQuad(offset, block.Properties.TileFront);
                            }
                        }
                        else
                        {
                            buffer.AddFrontQuad(offset, block.Properties.TileFront);
                        }
                    }

                    if (GetBlock(x, y, z + 1)?.Properties == null)
                    {
                        if (z == 15)
                        {
                            var neighbor = world.GetChunk(Position.x, Position.y + 1)?
                                .GetBlock(x, y, 0);
                            if (neighbor?.Properties == null)
                            {
                                buffer.AddLeftQuad(offset, block.Properties.TileLeft);
                            }
                        }
                        else
                        {
                            buffer.AddLeftQuad(offset, block.Properties.TileLeft);
                        }
                    }

                    if (GetBlock(x, y, z - 1)?.Properties == null)
                    {
                        if (z == 0)
                        {
                            var neighbor = world.GetChunk(Position.x, Position.y - 1)?
                                .GetBlock(x, y, 15);
                            if (neighbor?.Properties == null)
                            {
                                buffer.AddRightQuad(offset, block.Properties.TileRight);
                            }
                        }
                        else
                        {
                            buffer.AddRightQuad(offset, block.Properties.TileRight);
                        }
                    }
                }
            }
        }

        var newMesh = new Mesh();
        newMesh.vertices = buffer.vertices.ToArray();
        newMesh.triangles = buffer.triangles.ToArray();
        newMesh.uv = buffer.uv.ToArray();
        newMesh.RecalculateNormals();
        return newMesh;
    }

    public void SetDirty()
    {
        if (Dirty || Generating)
        {
            return;
        }

        Dirty = true;
        world.DirtyChunks.Enqueue(this);
    }
}