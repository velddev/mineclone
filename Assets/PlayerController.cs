using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class PlayerController : MonoBehaviour
{
    public GameObject BlockCursor;
    public Text DebugLabel;
    
    private RaycastHit hit;
    private Camera playerCamera;
    
    private void Start()
    {
        playerCamera = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    private static readonly BlockProperties stoneBlock = new()
    {
        TileTop = 4,
        TileBottom = 4,
        TileLeft = 4,
        TileRight = 4,
        TileFront = 4,
        TileBack = 4
    };

    
    private void Update()
    {
        // rotate camera on mouse y
        var mouseDelta = Input.GetAxis("Mouse Y");
        var cameraRotation = playerCamera.transform.rotation.eulerAngles;
        cameraRotation.x -= mouseDelta;
        playerCamera.transform.rotation = Quaternion.Euler(cameraRotation);
        
        // rotate character on mouse x
        var mouseDeltaX = Input.GetAxis("Mouse X");
        var characterRotation = transform.rotation.eulerAngles;
        characterRotation.y += mouseDeltaX;
        transform.rotation = Quaternion.Euler(characterRotation);

        var velX = Input.GetAxis("Horizontal");
        var velZ = Input.GetAxis("Vertical");

        transform.position += transform.forward * (velZ * (Time.deltaTime * 5f));
        transform.position += transform.right * (velX * (Time.deltaTime * 5f));

        if (Input.GetKeyDown(KeyCode.Space))
        {
            GetComponent<Rigidbody>().AddForce(Vector3.up * 500f);
        }
        
        BlockCursor.SetActive(false);

        Block block = null;
        ChunkAccessor chunk = null;
        var isHit = Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, 5f);
        if (isHit)
        {
            chunk = hit.collider.GetComponent<ChunkAccessor>();
            block = chunk.GetBlock(hit.point + playerCamera.transform.forward * 0.01f);
            if (block != null)
            {
                BlockCursor.SetActive(true);
                BlockCursor.transform.position = block.GetWorldPosition();
                BlockCursor.transform.position += Vector3.up / 2;
                BlockCursor.transform.position += Vector3.right / 2;
                BlockCursor.transform.position += Vector3.forward / 2;
            }
            
            if (Input.GetMouseButtonDown(0))
            {
                chunk.SetBlock(hit.point + playerCamera.transform.forward * 0.01f, null);
            }
        
            if (Input.GetMouseButtonDown(1))
            {
                chunk.SetBlock(hit.point - playerCamera.transform.forward * 0.01f, stoneBlock);
            }
        }

        // DebugLabel.text = 
        //     $"Mouse X: {mouseDeltaX}, Mouse Y: {mouseDelta}\n" +
        //     $"Camera Rotation: {cameraRotation}\n" +
        //     $"Character Rotation: {characterRotation}\n" +
        //     $"Block: {(block != null ? block.ToString() : "null")}";
    }
}
