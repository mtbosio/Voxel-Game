using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// a class that can be extended by special blocks such as chests, crafting tables etc.
public class ToolAssembler : SpecialBlock {
    public GameObject ToolAssemblerBlock;
    
    public override void Initialize(Vector3Int position, BlockType blockType)
    {
        this.blockType = blockType;
        this.position = position;
        blockDataManager = GameObject.Find("BlockDataManager").GetComponent<BlockDataManager>();
        ToolAssemblerBlock = blockDataManager.SpawnSpecialBlock(this);
    }
    public override GameObject GetSpecialBlock()
    {
        return ToolAssemblerBlock;
    }
    public override void Interact(){
        ToolAssemblerBlock.SetActive(true);
        InventoryManager.instance.otherInventoryOpen = ToolAssemblerBlock;
        InventoryManager.instance.ToggleInventoryOpen();
        InventoryManager.instance.inventoryGroup.SetActive(false);
    }
}