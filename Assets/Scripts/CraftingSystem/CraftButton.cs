using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CraftButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{ 
    public CraftingSystem craftingSystem;
    public Transform outputSlot;
    public GameObject hoverableInfo;
    public RectTransform hoverableInfoRect;
    void Start()
    {
        hoverableInfo = GameObject.Find("InventoryCanvas").transform.Find("HoverableInfoContainer").gameObject;      
    }
    public void OnPointerClick(PointerEventData eventData){
            // try craft one
            if(eventData.button == PointerEventData.InputButton.Left){
                (Item item, int outputCount) = craftingSystem.TryCraftItem();
                if(item){
                    // if there is already an item in the output slot
                    if(outputSlot.childCount == 1){
                        // if the item to be crafted and the item in the slot are the same and the item is stackable
                        if(item.IsStackable && item.ItemId == outputSlot.GetChild(0).gameObject.GetComponent<InventoryItem>().item.ItemId){
                            int result = outputSlot.GetChild(0).gameObject.GetComponent<InventoryItem>().count + outputCount;
                            if(result <= InventoryManager.instance.maxStackedItems) {
                                outputSlot.GetChild(0).gameObject.GetComponent<InventoryItem>().SetCount(result);
                                craftingSystem.DecreaseItemCount(1);
                            }
                        }
                    }
                    // if there is no item in the output slot
                    else{
                        InventoryItem inventoryItem = InventoryManager.instance.SpawnNewItem(item);
                        inventoryItem.SetCount(outputCount);
                        inventoryItem.transform.SetParent(outputSlot);
                        InventoryManager.instance.currentSelectedItem = null;
                        craftingSystem.DecreaseItemCount(1);
                    }
                }
            }
            // try craft all
            else if(eventData.button == PointerEventData.InputButton.Right){
            }
    } 

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(InventoryManager.instance.currentSelectedItem == null){
            hoverableInfo.SetActive(true);
            hoverableInfo.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = "Left Click: Craft one\nRight Click: Craft all";
        } 
    }

    // Called when the pointer exits the item
    public void OnPointerExit(PointerEventData eventData)
    {
        // Check if the pointer is still over the hoverableInfo
        if (!IsPointerOverUIElement(hoverableInfoRect))
        {
            hoverableInfo.SetActive(false);
        }
    }

    // Checks if the pointer is over the specified RectTransform
    private bool IsPointerOverUIElement(RectTransform rectTransform)
    {
        Vector2 localMousePosition = rectTransform.InverseTransformPoint(Input.mousePosition);
        return rectTransform.rect.Contains(localMousePosition);
    }
}
