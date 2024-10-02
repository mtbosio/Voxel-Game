using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using TMPro;

public class InventoryItem : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI")]
    public Image image;
    public Text countText;
    [SerializeField] public Item item;
    [HideInInspector] public int count = 1;
    [HideInInspector] public Transform parentAfterDrag;
    private GameObject hoverableInfo;
    public RectTransform hoverableInfoRect;
    public void InitializeItem(Item item){
        this.item = item;
        image.sprite = item.Image;
        hoverableInfo = GameObject.Find("InventoryCanvas").transform.Find("HoverableInfoContainer").gameObject;
        RefreshCount();
    }
    public void RefreshCount() {
        countText.text = count.ToString();
        bool textActive = count > 1;
        countText.gameObject.SetActive(textActive);
    }
    public void SetCount(int count) {
        this.count = count;
        RefreshCount();
    }
    // Handles InventoryItem being clicked
    public void OnPointerClick(PointerEventData eventData){
        // if there is a hoverable info then hide it
        hoverableInfo.SetActive(false);
        
        Transform currentSelectedItemTransform = InventoryManager.instance.currentSelectedItem;
        // if the right mouse button was pressed
        if(eventData.button == PointerEventData.InputButton.Right){
            // if there is an item already selected
            if(currentSelectedItemTransform){
                // if the selected item and this item match deposit one item into this item
                if(item.IsStackable && currentSelectedItemTransform.GetComponent<InventoryItem>().item.ItemId == item.ItemId){
                    // check if adding one to count would be > maxStackedItems
                    int result = count + 1;
                    if(result <= InventoryManager.instance.maxStackedItems) {
                        count++;
                        currentSelectedItemTransform.GetComponent<InventoryItem>().SetCount(currentSelectedItemTransform.GetComponent<InventoryItem>().count-1);
                        if(currentSelectedItemTransform.GetComponent<InventoryItem>().count <= 0) {
                            Destroy(currentSelectedItemTransform.gameObject);
                            InventoryManager.instance.currentSelectedItem = null;
                        } 

                    }
                }
                // if they don't match do nothing
            }
            // if there is no item selected, grab half the stack of this item 
            else{
                // make sure stack is splittable
                if(count > 1){
                    int newCount = count / 2;
                    InventoryItem newItem = InventoryManager.instance.SpawnNewItem(item);
                    if(count % 2 == 0) {
                        newItem.SetCount(newCount);
                    } else {
                        newItem.SetCount(newCount + 1);
                    }
                    newItem.transform.SetParent(GameObject.Find("InventoryCanvas").transform);
                    newItem.image.raycastTarget = false;
                    count /= 2;
                } else {
                    // select this item
                    InventoryManager.instance.currentSelectedItem = transform;
                    image.raycastTarget = false;
                    transform.SetParent(GameObject.Find("InventoryCanvas").transform);
                }
            }
        } 
        // if the left mouse button was pressed 
        else if (eventData.button == PointerEventData.InputButton.Left) { 
                // if there is no current item selected
                if(currentSelectedItemTransform == null){
                    // select this item
                    InventoryManager.instance.currentSelectedItem = transform;
                    image.raycastTarget = false;
                    transform.SetParent(GameObject.Find("InventoryCanvas").transform);
                }
                // if there is an item already selected
                else {
                    // if the item already selected and this item are the same
                    if(currentSelectedItemTransform.GetComponent<InventoryItem>().item.ItemId == item.ItemId){
                        // add them and check if the result > max stack size
                        int result = currentSelectedItemTransform.GetComponent<InventoryItem>().count + count;
                        if(result > InventoryManager.instance.maxStackedItems){
                            count = InventoryManager.instance.maxStackedItems;
                            // set current selected items count of result - maxStackedItems and set it as currentSelectedItem
                            InventoryManager.instance.currentSelectedItem.GetComponent<InventoryItem>().SetCount( result - InventoryManager.instance.maxStackedItems);
                        } else {
                            count = result;
                            Destroy(currentSelectedItemTransform.gameObject);
                            InventoryManager.instance.currentSelectedItem = null;
                        }
                        image.raycastTarget = true;
                    } 
                    // if the item already selected and this item are not the same
                    else {
                        // switch the items and set current selected item to this
                        currentSelectedItemTransform.SetParent(transform.parent);
                        InventoryManager.instance.currentSelectedItem.GetComponent<InventoryItem>().image.raycastTarget = true;
                        InventoryManager.instance.currentSelectedItem = transform;
                        image.raycastTarget = false;
                        transform.SetParent(GameObject.Find("InventoryCanvas").transform);
                    }  
                }
        }
        RefreshCount();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(InventoryManager.instance.currentSelectedItem == null){
            hoverableInfo.SetActive(true);
            hoverableInfo.GetComponent<HoverableInfo>().UpdateText(item);
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
