using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CreateAssetMenu(menuName = "Scriptable object/BlockItem")]
public class BlockItem : Item {
    public BlockType blockType;
    public BlockItem GetBlock(){
        return this;
    }
}