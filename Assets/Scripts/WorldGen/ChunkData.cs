using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkData
{
    public BlockType[] blocks;
    public Dictionary<Vector3Int, BlockBreakData> BlocksBeingBroken { get; private set; } = new Dictionary<Vector3Int, BlockBreakData>();

    public int chunkSize = 16;
    public int chunkHeight = 100;
    public World worldReference;
    public Vector3Int worldPosition;
    public bool modifiedByThePlayer = false;
    public TreeData treeData;

    public ChunkData(int chunkSize, int chunkHeight, World world, Vector3Int worldPosition)
    {
        this.chunkHeight = chunkHeight;
        this.chunkSize = chunkSize;
        this.worldReference = world;
        this.worldPosition = worldPosition;
        blocks = new BlockType[chunkSize * chunkHeight * chunkSize];
    }

    public void StartBreakingBlock(Vector3Int position, BlockType blockType, float breakTime, ChunkRenderer chunkRenderer)
    {
        if (!BlocksBeingBroken.ContainsKey(position))
        {
            BlocksBeingBroken[position] = new BlockBreakData(position, blockType, breakTime, chunkRenderer);
        }
    }

    public void UpdateBreakingBlock(Vector3Int position, float deltaTime)
    {
        if (BlocksBeingBroken.ContainsKey(position))
        {
            var breakData = BlocksBeingBroken[position];
            breakData.breakProgress += deltaTime;

           
            BlocksBeingBroken[position] = breakData;
            
        }
    }

    public bool IsBlockBeingBroken(Vector3Int position)
    {
        return BlocksBeingBroken.ContainsKey(position);
    }

}
