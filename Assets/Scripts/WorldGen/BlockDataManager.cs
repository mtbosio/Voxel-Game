using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockDataManager : MonoBehaviour
{
    public static float textureOffset = 0.001f;
    public static float tileSizeX, tileSizeY;
    public static Dictionary<BlockType, TextureData> blockTextureDataDictionary = new Dictionary<BlockType, TextureData>();
    public BlockDataSO textureData;
    public Dictionary<Vector3Int, SpecialBlock> blockMetaData;
    public GameObject toolAssembler;
    public Transform specialBlocksGO;
    private void Awake()
    {
        foreach (var item in textureData.textureDataList)
        {
            if (blockTextureDataDictionary.ContainsKey(item.blockType) == false)
            {
                blockTextureDataDictionary.Add(item.blockType, item);
            };
        }
        tileSizeX = textureData.textureSizeX;
        tileSizeY = textureData.textureSizeY;

        blockMetaData = new Dictionary<Vector3Int, SpecialBlock>();
    }
    public GameObject SpawnSpecialBlock(SpecialBlock block){
        switch(block.blockType){
            case BlockType.ToolAssembler: 
                return Instantiate(toolAssembler, specialBlocksGO);
            default: 
                return null;
        }
    }
}
