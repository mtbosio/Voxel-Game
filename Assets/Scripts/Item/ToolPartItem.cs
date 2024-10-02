using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[CreateAssetMenu(menuName = "Scriptable object/ToolPartItem")]
public class ToolPartItem : Item {
    public ToolPartType toolPartType;
    public BreakLevel breakLevel;
    public int addedDurability;
    public float speedMultiplier;
    
}
