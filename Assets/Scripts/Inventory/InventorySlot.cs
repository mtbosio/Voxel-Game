using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : Slot
{
    public Image image;
    public Color selectedColor, notSelectedColor;

    public void Awake() {
        Deselect();
    }

    // used for setting the hotbar slots as selected or deselected
    public void Select() {
        image.color = selectedColor;
    }
    public void Deselect() {
        image.color = notSelectedColor;
    }
}
