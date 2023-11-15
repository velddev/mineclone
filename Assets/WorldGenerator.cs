using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class WorldGenerator : MonoBehaviour
{
    public int WorldSize = 10;

    private static readonly BlockProperties testBlock = new()
    {
        TileTop = 0,
        TileBottom = 1,
        TileLeft = 2,
        TileRight = 3,
        TileFront = 4,
        TileBack = 5
    };
    
    private static readonly BlockProperties stoneBlock = new()
    {
        TileTop = 3,
        TileBottom = 3,
        TileLeft = 3,
        TileRight = 3,
        TileFront = 3,
        TileBack = 3
    };
    
    private static readonly BlockProperties coalBlock = new()
    {
        TileTop = 4,
        TileBottom = 4,
        TileLeft = 4,
        TileRight = 4,
        TileFront = 4,
        TileBack = 4
    };

    private static readonly BlockProperties grassBlock = new()
    {
        TileTop = 0,
        TileBottom = 2,
        TileLeft = 1,
        TileRight = 1,
        TileFront = 1,
        TileBack = 1
    };

    private static readonly BlockProperties dirtBlock = new()
    {
        TileBack = 2,
        TileBottom = 2,
        TileFront = 2,
        TileLeft = 2,
        TileRight = 2,
        TileTop = 2
    };

    // Start is called before the first frame update
    void Start()
    {
        CreateMeshTest();
    }

    public void CreateMeshTest()
    {
        var chunksX = WorldSize;
        var chunksY = WorldSize;
        
        var world = new World();
        for (var chunkX = 0; chunkX < chunksX; chunkX++)
        {
            for (var chunkY = 0; chunkY < chunksY; chunkY++)
            {
                var chunk = world.RequestChunk(chunkX, chunkY);
                for (var blockX = 0; blockX < 16; blockX++)
                {
                    for (var blockZ = 0; blockZ < 16; blockZ++)
                    {
                        var height = 48f + Mathf.PerlinNoise(chunkX * 16 + blockX * 0.05333f, chunkY * 16 + blockZ * 0.05333f) * 8f;
                        var terrainHeight = Mathf.RoundToInt(height);

                        for (var blockY = 0; blockY < terrainHeight; blockY++)
                        {
                            chunk.SetBlock(blockX, blockY, blockZ,
                                blockY == terrainHeight - 1 
                                ? grassBlock 
                                : blockY < terrainHeight - (2 + Random.Range(1, 3))
                                    ? Random.Range(1, 12) == 10 ? coalBlock : stoneBlock  
                                    : dirtBlock);
                        }
                        
                        var treeChance = Random.Range(1, 100);
                        if (treeChance < 4)
                        {
                            var treeStemHeight = Random.Range(3, 8);
                            for (var treeStemY = 0; treeStemY < treeStemHeight; treeStemY++)
                            {
                                chunk.SetBlock(blockX, terrainHeight + treeStemY, blockZ, new BlockProperties
                                {
                                    TileTop = 5,
                                    TileBottom = 5,
                                    TileLeft = 6,
                                    TileRight = 6,
                                    TileFront = 6,
                                    TileBack = 6
                                });
                            }
                                
                            var treeTopHeight = Random.Range(2, 4);
                            // create a spherical tree top
                            for (var treeTopX = -treeTopHeight; treeTopX <= treeTopHeight; treeTopX++)
                            {
                                for (var treeTopY = 0; treeTopY <= treeTopHeight; treeTopY++)
                                {
                                    for (var treeTopZ = -treeTopHeight; treeTopZ <= treeTopHeight; treeTopZ++)
                                    {
                                        if (treeTopX * treeTopX + treeTopY * treeTopY + treeTopZ * treeTopZ <= treeTopHeight * treeTopHeight)
                                        {
                                            var leafX = blockX + treeTopX;
                                            var leafZ = blockZ + treeTopZ;
                                            if (leafX < 0 || leafX >= 15 || leafZ < 0 || leafZ >= 15)
                                            {
                                                continue;
                                            }
                        
                                            chunk.SetBlock(leafX, terrainHeight + treeStemHeight + treeTopY, leafZ, new BlockProperties
                                            {
                                                TileTop = 7,
                                                TileBottom = 7,
                                                TileLeft = 7,
                                                TileRight = 7,
                                                TileFront = 7,
                                                TileBack = 7
                                            });
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                var gameObject = new GameObject();
                gameObject.transform.position = new Vector3(chunkX * 16, 0, chunkY * 16);
                var meshFilter = gameObject.AddComponent<MeshFilter>();
                var meshRenderer = gameObject.AddComponent<MeshRenderer>();
                var meshCollider = gameObject.AddComponent<MeshCollider>();
                var chunkAccessor = gameObject.AddComponent<ChunkAccessor>();

                chunkAccessor.Init(chunk);
                
                var mat = Resources.Load<Material>("Materials/Block");
                mat.mainTexture = Resources.Load<Texture2D>("Sprites/world");

                meshFilter.mesh = chunk.GetMesh();
                meshCollider.sharedMesh = meshFilter.mesh;
                meshRenderer.material = mat;
            }
        }
        
        for (var chunkX = 0; chunkX < chunksX; chunkX++)
        {
            for (var chunkY = 0; chunkY < chunksY; chunkY++)
            {
                var chunk = world.GetChunk(chunkX, chunkY);
                chunk.SetDirty();
            }
        }
    }
}

