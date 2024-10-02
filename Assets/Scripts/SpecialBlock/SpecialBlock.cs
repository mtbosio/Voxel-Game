using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// a class that can be extended by special blocks such as chests, crafting tables etc.
public abstract class SpecialBlock {
    public Vector3Int position;
    public BlockType blockType;
    public BlockDataManager blockDataManager;
    public abstract void Initialize(Vector3Int position, BlockType blockType);
    public abstract void Interact();
    public abstract GameObject GetSpecialBlock();
}