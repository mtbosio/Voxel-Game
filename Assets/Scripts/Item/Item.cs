using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
[CreateAssetMenu(menuName = "Scriptable object/Item")]
public class Item : ScriptableObject {
    public int ItemId;
    public String ItemName;
    public Sprite Image;
    public bool IsStackable = true;
}
