using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "Scriptable object/ToolItem")]
public class ToolItem : Item {
    public ToolType toolType;
    public BreakLevel breakLevel;
    public float speedMultiplier;
    public List<ToolPartItem> toolPartItems;
    public int maxDurability;
    public int durability;

    public int CalculateMaxDurability(){
        return toolPartItems.Sum(part => part.addedDurability);
    }
    public float CalculateSpeedMultiplier(){
        float total = toolPartItems.Sum(part => part.speedMultiplier);
        float difference = Mathf.Ceil(total) - total;

    // If the difference is less than or equal to 0.2, round up
    if (difference <= 0.2f)
    {
        return Mathf.Ceil(total);
    }

    // Otherwise, return the original value without rounding
    return total;
    }
}
public enum ToolType
{
    None,
    Pickaxe,
    Axe,
    Sword,
    Sharp_Log,
    Shovel
}
public enum ToolPartType{
    // pickaxe parts
    PickaxeHead,
    // axe parts
    AxeHead,
    // sword parts
    SwordBlade,
    SwordCrossPiece,
    // multi
    Handle,
    Rod
    
}

public enum BreakLevel {
    None,
    Wood,
    Stone,
    Iron,
    Diamond
}