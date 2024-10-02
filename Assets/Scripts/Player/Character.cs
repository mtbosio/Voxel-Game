using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera;
    [SerializeField]
    private PlayerInput playerInput;
    [SerializeField]
    private PlayerMovement playerMovement;

    public float interactionRayLength = 5;

    public LayerMask groundMask;


    public bool fly = false;

    public Animator animator;

    bool isWaiting = false;

    public World world;
    public BlockDataManager blockDataManager;
    private BlockBreakData currentBlockBreakData;
    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
        playerInput = GetComponent<PlayerInput>();
        playerMovement = GetComponent<PlayerMovement>();
        world = FindObjectOfType<World>();
        blockDataManager = FindObjectOfType<BlockDataManager>();
    }

    private void Start()
    {
        playerInput.OnLeftMouseClick += HandleLeftMouseClick;
        playerInput.OnRightMouseClick += HandleRightMouseClick;
        playerInput.OnFly += HandleFlyClick;
        playerInput.OnInventoryButtonPressed += HandleInventoryButtonPress;
        playerInput.OnRightMouseUp += HandleLeftMouseUp;
    }
    

    private void HandleFlyClick()
    {
        fly = !fly;
    }

    void Update()
    {
        if (fly)
        {
            animator.SetFloat("speed", 0);
            animator.SetBool("isGrounded", false);
            animator.ResetTrigger("jump");
            playerMovement.Fly(playerInput.MovementInput, playerInput.IsJumping, playerInput.RunningPressed);

        }
        else
        {
            animator.SetBool("isGrounded", playerMovement.IsGrounded);
            if (playerMovement.IsGrounded && playerInput.IsJumping && isWaiting == false)
            {
                animator.SetTrigger("jump");
                isWaiting = true;
                StopAllCoroutines();
                StartCoroutine(ResetWaiting());
            }
            animator.SetFloat("speed", playerInput.MovementInput.magnitude);
            playerMovement.HandleGravity(playerInput.IsJumping);
            playerMovement.Walk(playerInput.MovementInput, playerInput.RunningPressed);


        }

    }
    IEnumerator ResetWaiting()
    {
        yield return new WaitForSeconds(0.1f);
        animator.ResetTrigger("jump");
        isWaiting = false;
    }

    private void HandleLeftMouseClick()
    {
        Ray playerRay = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(playerRay, out hit, interactionRayLength, groundMask))
        {
            ChunkRenderer chunk = hit.collider.GetComponent<ChunkRenderer>();
            if (chunk == null)
                return;
            
            Vector3Int pos = world.GetBlockPos(hit);
            BlockType hitBlock = world.GetBlockFromChunkCoordinates(chunk.ChunkData, pos.x, pos.y, pos.z);
            // useful for displaying overlay on the correct block pos
            Vector3Int localPos = world.GetPosFromChunkCoordinates(chunk.ChunkData, pos.x, pos.y, pos.z);
            if(hitBlock == BlockType.Air) return; // glitch need to figure this out

            float breakTime = BlockBreakData.GetBlockBreakTime(hitBlock, InventoryManager.instance.GetSelectedItem(false));
            
            if (currentBlockBreakData == null || currentBlockBreakData.blockPosition != localPos){
                // Start breaking a new block

                // first if there is already a block being broken reset it
                if(currentBlockBreakData != null){
                    currentBlockBreakData.chunk.ChunkData.BlocksBeingBroken.Remove(currentBlockBreakData.blockPosition);
                    currentBlockBreakData.chunk.UpdateChunk();
                }

                chunk.ChunkData.StartBreakingBlock(localPos, hitBlock, breakTime, chunk);
                currentBlockBreakData = chunk.ChunkData.BlocksBeingBroken[localPos];
            }else{
                // Continue breaking the block
                chunk.ChunkData.UpdateBreakingBlock(localPos, Time.deltaTime);
                currentBlockBreakData.breakProgress = chunk.ChunkData.BlocksBeingBroken[localPos].breakProgress;

                float progress = currentBlockBreakData.breakProgress / breakTime;

                // Update the breaking overlay on the block
                UpdateBreakingOverlay(chunk, localPos, progress);
                
                if (currentBlockBreakData.breakProgress >= currentBlockBreakData.breakTime) {
                    chunk.ChunkData.BlocksBeingBroken.Remove(localPos);
                    // Block is broken
                    ModifyTerrain(hit, BlockType.Air);
                    currentBlockBreakData = null; // Reset the break data

                    // if using a tool, reduce durability
                    if(InventoryManager.instance.GetSelectedItem(false) is ToolItem toolItem){
                        if(BlockTypeExtensions.GetBlockData(hitBlock).EffectiveTool == toolItem.toolType){
                            toolItem.durability -= 1;
                        } else {
                            toolItem.durability -= 2;
                        }
                        if(toolItem.durability <= 0){
                            Debug.Log("Break tool");
                        }
                    }
                } 
            }
        } else {
            currentBlockBreakData = null; // Reset if no block is hit
        }
    }

    private void HandleLeftMouseUp(){
        if(currentBlockBreakData != null){
            currentBlockBreakData.chunk.ChunkData.BlocksBeingBroken.Remove(currentBlockBreakData.blockPosition);
            currentBlockBreakData.chunk.UpdateChunk();
            currentBlockBreakData = null;
        }
    }

    private void UpdateBreakingOverlay(ChunkRenderer chunk, Vector3Int pos, float progress)
    {
        if(progress <= (1.0/6)){
            chunk.ChunkData.BlocksBeingBroken[pos].overlayProgress = 0;
            chunk.UpdateChunk();
        } else if(progress <= (2.0/6)){
            chunk.ChunkData.BlocksBeingBroken[pos].overlayProgress = 1;
            chunk.UpdateChunk();
        } else if(progress <= (3.0/6)){
            chunk.ChunkData.BlocksBeingBroken[pos].overlayProgress = 2;
            chunk.UpdateChunk();
        } else if(progress <= (4.0/6)){
            chunk.ChunkData.BlocksBeingBroken[pos].overlayProgress = 3;
            chunk.UpdateChunk();
        } else if(progress <= (5.0/6)){
            chunk.ChunkData.BlocksBeingBroken[pos].overlayProgress = 4;
            chunk.UpdateChunk();
        } else if(progress <= (5.9/6)){
            chunk.ChunkData.BlocksBeingBroken[pos].overlayProgress = 5;
            chunk.UpdateChunk();
        }
    }

    private void HandleRightMouseClick() {
        Ray playerRay = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(playerRay, out hit, interactionRayLength, groundMask))
        {
            ChunkRenderer chunk = hit.collider.GetComponent<ChunkRenderer>();
            if (chunk == null)
                return;

            Vector3Int pos = world.GetBlockPos(hit);
            BlockType hitBlock = world.GetBlockFromChunkCoordinates(chunk.ChunkData, pos.x, pos.y, pos.z);

            if(blockDataManager.blockMetaData.TryGetValue(pos, out SpecialBlock specialBlock)){
                    specialBlock.Interact();
                    return;
            }

            if (InventoryManager.instance.GetSelectedItem(false) is BlockItem blockItem)
            {
                if(hitBlock == BlockType.Air) return;
                if(ModifyTerrain(hit, blockItem.blockType)) InventoryManager.instance.GetSelectedItem(true);
            } 
        } 
    }

    private void HandleInventoryButtonPress() {
        InventoryManager.instance.ToggleInventoryOpen();
    }

    private bool ModifyTerrain(RaycastHit hit, BlockType block)
    {
        return world.SetBlock(hit, block);
          
    }


}

public class BlockBreakData
{
    public ChunkRenderer chunk;
    public Vector3Int blockPosition;
    public BlockType blockType;
    public float breakTime;
    public float breakProgress;
    public int overlayProgress;
    public BlockBreakData(Vector3Int pos, BlockType type, float time, ChunkRenderer chunkRenderer)
    {
        chunk = chunkRenderer;
        blockPosition = pos;
        blockType = type;
        breakTime = time;
        breakProgress = 0f;
    }

    public static float GetBlockBreakTime(BlockType blockType, Item tool)
    {
        BlockData blockData = BlockTypeExtensions.GetBlockData(blockType);
        float baseTime = blockData.BaseHardness; // Default base hardness
        float speedMultiplier = 1f;
        if (tool is ToolItem toolItem) {
            if(toolItem.breakLevel >= blockData.RequiredBreakLevel){ // if the tool is  the required tool level, mining time = 1.5 x base hardness
                    baseTime = 1.5f * baseTime;
                    if(blockData.EffectiveTool == toolItem.toolType){ // if the tool is the effective tool type of the block, speed is increased
                        speedMultiplier = toolItem.speedMultiplier;
                    }
            } else { // if the tool is  the required tool level, mining time = 5.0 x base hardness
                baseTime = 5.0f * baseTime;
            }
        } else {
            if(blockData.RequiredBreakLevel == BreakLevel.None){ // if the block doesnt require a higher level of mining
                baseTime = 1.5f * baseTime;
            } else {
                baseTime = 5.0f * baseTime;
            }
        }

        return baseTime / speedMultiplier;
    }
};


