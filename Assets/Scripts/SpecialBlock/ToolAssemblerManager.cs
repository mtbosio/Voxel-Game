using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class ToolAssemblerManager : MonoBehaviour{
    public AssembleButton assembleButton;
    public List<ToolPartItem> tool;
    public GameObject currentContainer;
    public Transform outputSlot;
    public ToolItem TryAssembleTool(){
        if(outputSlot.childCount == 0){
            // loop through the current container to see if all the inventorySlots contain an inventory item
            for(int i = 0; i < currentContainer.transform.childCount - 1; i++){ 
                if(currentContainer.transform.GetChild(i).childCount == 1){ 
                    if(currentContainer.transform.GetChild(i).GetChild(0).GetComponent<InventoryItem>().item is ToolPartItem toolPartItem){
                        tool.Add(toolPartItem);
                    } else{
                        tool.Clear();
                        return null;
                    }
                } else {
                    tool.Clear();
                    return null;
                }
            }
            return CalculateToolStats(tool);
        } else {
            return null;
        }
        
    }   
    public void DecreasePartCounts(){
        // loop through the current container to decrease count of each item
        for(int i = 0; i < currentContainer.transform.childCount - 1; i++){ 
            if(currentContainer.transform.GetChild(i).childCount == 1){ 
                currentContainer.transform.GetChild(i).GetChild(0).GetComponent<InventoryItem>().SetCount(currentContainer.transform.GetChild(i).GetChild(0).GetComponent<InventoryItem>().count - 1);
                if(currentContainer.transform.GetChild(i).GetChild(0).GetComponent<InventoryItem>().count <= 0){
                    Destroy(currentContainer.transform.GetChild(i).GetChild(0).gameObject);
                }
            }
        }
    }

    public ToolItem CalculateToolStats(List<ToolPartItem> toolPartItems){
        ToolItem toolItem = ScriptableObject.CreateInstance<ToolItem>();
        toolItem.ItemId = -2;

        if (currentContainer.name == "PickaxeContainer") {
            if(toolPartItems[0].toolPartType != ToolPartType.PickaxeHead){
                return null;
            }
            if(toolPartItems[1].toolPartType != ToolPartType.Rod){
                return null;
            }
            if(toolPartItems[2].toolPartType != ToolPartType.Handle){
                return null;
            }
            
            toolItem.ItemName = toolPartItems[0].breakLevel.ToString() + " Pickaxe";
            toolItem.toolType = ToolType.Pickaxe;
            toolItem.Image = ToolPartSpriteCombiner(toolPartItems, ToolType.Pickaxe);
        } else if (currentContainer.name == "SwordContainer") {
            if(toolPartItems[0].toolPartType != ToolPartType.SwordBlade){
                return null;
            }
            if(toolPartItems[1].toolPartType != ToolPartType.SwordCrossPiece){
                return null;
            }
            if(toolPartItems[2].toolPartType != ToolPartType.Handle){
                return null;
            }

            toolItem.ItemName = toolPartItems[0].breakLevel.ToString() + " Sword";
            toolItem.toolType = ToolType.Sword;
            toolItem.Image = ToolPartSpriteCombiner(toolPartItems, ToolType.Sword);
        } else if (currentContainer.name == "AxeContainer") {
            if(toolPartItems[0].toolPartType != ToolPartType.AxeHead){
                return null;
            }
            if(toolPartItems[1].toolPartType != ToolPartType.Rod){
                return null;
            }
            if(toolPartItems[2].toolPartType != ToolPartType.Handle){
                return null;
            }

            toolItem.ItemName = toolPartItems[0].breakLevel.ToString() + " Axe";
            toolItem.toolType = ToolType.Axe;
            toolItem.Image = ToolPartSpriteCombiner(toolPartItems, ToolType.Axe);
        } else {
            return null;
        }

        toolItem.toolPartItems = toolPartItems;
        toolItem.IsStackable = false;
        
        toolItem.durability = toolItem.CalculateMaxDurability();
        toolItem.maxDurability = toolItem.durability;
        toolItem.speedMultiplier = toolItem.CalculateSpeedMultiplier();
        toolItem.breakLevel = toolPartItems[0].breakLevel;

        InventoryItem inventoryItem = InventoryManager.instance.SpawnNewItem(toolItem);
        inventoryItem.SetCount(1);
        inventoryItem.transform.SetParent(outputSlot);
        InventoryManager.instance.currentSelectedItem = null;
        
        DecreasePartCounts();
        return toolItem;
    }
    public void ChangeCurrentContainer(GameObject newContainer){
        if(newContainer != currentContainer){
            newContainer.SetActive(true);
            currentContainer.SetActive(false);
            currentContainer = newContainer;
        }
    }
    public Sprite ToolPartSpriteCombiner(List<ToolPartItem> toolPartItems, ToolType toolType){
        Resources.UnloadUnusedAssets();
        var newImage = new Texture2D(16, 16, TextureFormat.ARGB32, false);
        newImage.filterMode = FilterMode.Point;

        // Set the new image background to transparent
        for (int x = 0; x < newImage.width; x++) {
            for (int y = 0; y < newImage.height; y++) {
                newImage.SetPixel(x, y, new Color(1, 1, 1, 0)); // Transparent background
            }
        }
        
        Vector2Int[] positions;
        if (toolType == ToolType.Pickaxe)
        {
            positions = new Vector2Int[] {
                new Vector2Int(5, 5),   // Top right
                new Vector2Int(4, 4),   // Middle
                new Vector2Int(-1, -4)    // Bottom Left
            };
        }
        else if (toolType == ToolType.Sword)
        {
            positions = new Vector2Int[] {
                new Vector2Int(5, 5),   // Top right
                new Vector2Int(4, 4),   // Middle
                new Vector2Int(-1, -4)    // Bottom Left
            };
        }
        else
        {
            // Default positions if not a specific tool type (add other cases as needed)
            positions = new Vector2Int[] {
                new Vector2Int(5, 5),   // Top right
                new Vector2Int(4, 4),   // Middle
                new Vector2Int(-1, -4)    // Bottom Left
            };
        }


        // Loop through the tool part items and copy only the non-transparent pixels
        for (int i = Mathf.Min(toolPartItems.Count, positions.Length) - 1; i >= 0; i--) {
            Sprite partSprite = toolPartItems[i].Image;   // Get the current sprite
            Texture2D partTexture = partSprite.texture;   // Access the texture

            // Loop over the 16x16 part sprite
            for (int x = 0; x < 16; x++) {
                for (int y = 0; y < 16; y++) {
                    // Get the pixel color from the part sprite
                    Color pixelColor = partTexture.GetPixel(x + (int)partSprite.textureRect.x, y + (int)partSprite.textureRect.y);

                    // Only paste non-transparent pixels (alpha > 0)
                    if (pixelColor.a > 0f) {
                        // Calculate the destination position for this pixel
                        int destX = positions[i].x + x;
                        int destY = positions[i].y + y;

                        // Ensure the destination is within bounds of the 16x16 new image
                        if (destX >= 0 && destX < 16 && destY >= 0 && destY < 16) {
                            newImage.SetPixel(destX, destY, pixelColor);
                        }
                    }
                }
            }
        }
        
        newImage.Apply();
        var newSprite = Sprite.Create(newImage, new Rect(0, 0, newImage.width, newImage.height), new Vector2(0.5f, 0.5f));
        newSprite.name = "MergedSprite";
        return newSprite;
    }
}
