using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// An abstract class for Slots such as crafting and inventory slots
public abstract class Slot : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData){
        // if there is a currently selected item
        if(InventoryManager.instance.currentSelectedItem) {
            if(transform.childCount <= 0){ // sometimes you can click on the very edge of the cell when an item is already in it
                if(eventData.button == PointerEventData.InputButton.Left){
                    // set the hoverable info back to active
                    GameObject hoverableInfo = GameObject.Find("InventoryCanvas").transform.Find("HoverableInfoContainer").gameObject;
                    hoverableInfo.SetActive(true);
                    hoverableInfo.GetComponent<HoverableInfo>().UpdateText(InventoryManager.instance.currentSelectedItem.GetComponent<InventoryItem>().item);

                    // if left click, set the item to this slot and set current selected item to null
                    InventoryManager.instance.currentSelectedItem.SetParent(transform);
                    InventoryManager.instance.currentSelectedItem.GetComponent<InventoryItem>().image.raycastTarget = true;
                    InventoryManager.instance.currentSelectedItem = null;
                } else if (eventData.button == PointerEventData.InputButton.Right){
                    // if right click deposit one into slot
                    InventoryItem newItem = InventoryManager.instance.SpawnNewItem(InventoryManager.instance.currentSelectedItem.GetComponent<InventoryItem>().item, GetComponent<InventorySlot>());
                    //newItem.transform.SetParent(transform);
                    InventoryManager.instance.currentSelectedItem.GetComponent<InventoryItem>().SetCount(InventoryManager.instance.currentSelectedItem.GetComponent<InventoryItem>().count - 1);
                    
                    if(InventoryManager.instance.currentSelectedItem.GetComponent<InventoryItem>().count <= 0){
                        Destroy(InventoryManager.instance.currentSelectedItem.gameObject);
                        InventoryManager.instance.currentSelectedItem = null;
                    }
                }
            }
        }
    }
}
