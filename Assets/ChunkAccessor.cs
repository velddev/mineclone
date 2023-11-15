using System;
using UnityEngine;

public class ChunkAccessor : MonoBehaviour
{
    private Chunk chunk;

    private bool isInitialized;
    private bool isStarted;

    private MeshFilter meshFilter;
    private MeshCollider meshCollider;
    private MeshRenderer meshRenderer;
    
    private void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
        meshRenderer = GetComponent<MeshRenderer>();

        isStarted = true;
        UpdateMesh(null, null);
    }
    
    public void Init(Chunk chunk)
    {
        if (isInitialized)
        {
            Debug.LogError($"ChunkAccessor on {gameObject.name} already initialized!");
            return;
        }
        
        this.chunk = chunk;
        chunk.OnChunkChanged += UpdateMesh;
        isInitialized = true;
    }

    public Block GetBlock(Vector3 position)
    {
        var localizedPosition = position - transform.position;
        return chunk.GetBlock(new Vector3Int
        {
            x = (int)Math.Floor(localizedPosition.x),
            y = (int)Math.Floor(localizedPosition.y),
            z = (int)Math.Floor(localizedPosition.z)
        });
    }
    
    public void SetBlock(Vector3 position, BlockProperties block)
    {
        var localizedPosition = position - transform.position;

        if (Math.Abs(localizedPosition.x - 16f) < 0.01f)
        {
            localizedPosition.x = 15.99f;
        }
        
        if (Math.Abs(localizedPosition.y - 16f) < 0.01f)
        {
            localizedPosition.y = 15.99f;
        }
        
        if (Math.Abs(localizedPosition.z - 16f) < 0.01f)
        {
            localizedPosition.z = 15.99f;
        }
        
        chunk.SetBlock(
            (int)Math.Floor(localizedPosition.x), 
            (int)Math.Floor(localizedPosition.y), 
            (int)Math.Floor(localizedPosition.z), block);
    }

    private void UpdateMesh(object caller, EventArgs args)
    {
        if (!isStarted)
        {
            Debug.Log("Ignored mesh update request, chunk has not started yet.");
            return;
        }
        
        Debug.Log("Chunk " + chunk.Position + " updated!");
        var newMesh = chunk.GetMesh();
        if (meshFilter.mesh == newMesh)
        {
            return;
        }
        
        meshFilter.mesh = chunk.GetMesh();
        meshCollider.sharedMesh = meshFilter.mesh;
    }
    
    public Chunk GetChunk() => chunk;
}