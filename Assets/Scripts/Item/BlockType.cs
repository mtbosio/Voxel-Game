

using System.Collections.Generic;

public enum BlockType
{
    Nothing,
    Air,
    Grass_Dirt,
    Dirt,
    Grass_Stone,
    Stone,
    Oak_Log,
    TreeLeafesTransparent,
    TreeLeafsSolid,
    Water,
    Sand,
    Oak_Plank,
    ToolAssembler
}

public static class BlockTypeExtensions
{
    
    private static readonly Dictionary<BlockType, BlockData> blockDataMap = new Dictionary<BlockType, BlockData>
    {
        { BlockType.Nothing, new BlockData(0f, ToolType.None, BreakLevel.None) },
        { BlockType.Air, new BlockData(0f, ToolType.None, BreakLevel.None) },
        { BlockType.Grass_Dirt, new BlockData(0.9f, ToolType.Shovel, BreakLevel.None) },
        { BlockType.Dirt, new BlockData(0.75f, ToolType.Shovel, BreakLevel.None) },
        { BlockType.Grass_Stone, new BlockData(2.0f, ToolType.Pickaxe, BreakLevel.None) },
        { BlockType.Stone, new BlockData(1.5f, ToolType.Pickaxe, BreakLevel.Wood) },
        { BlockType.Oak_Log, new BlockData(2.5f, ToolType.Axe, BreakLevel.None) },
        { BlockType.TreeLeafesTransparent, new BlockData(0.5f, ToolType.None, BreakLevel.None) },
        { BlockType.TreeLeafsSolid, new BlockData(0.5f, ToolType.None, BreakLevel.None) },
        { BlockType.Water, new BlockData(0f, ToolType.None, BreakLevel.None) },
        { BlockType.Sand, new BlockData(0.75f, ToolType.Shovel, BreakLevel.None) },
        { BlockType.Oak_Plank, new BlockData(3.0f, ToolType.Axe, BreakLevel.None) },
        { BlockType.ToolAssembler, new BlockData(4.0f, ToolType.Pickaxe, BreakLevel.None) }
    };

    public static BlockData GetBlockData(this BlockType blockType)
    {
        return blockDataMap.TryGetValue(blockType, out var blockData) ? blockData : new BlockData(1.0f, ToolType.None, BreakLevel.None); // Default block data
    }
}

public class BlockData
{
    public float BaseHardness { get; }
    public ToolType EffectiveTool { get; }
    public BreakLevel RequiredBreakLevel { get; }    
    public BlockData(float baseHardness, ToolType effectiveTool, BreakLevel requiredBreakLevel)
    {
        BaseHardness = baseHardness;
        EffectiveTool = effectiveTool;
        RequiredBreakLevel = requiredBreakLevel;
    }
}