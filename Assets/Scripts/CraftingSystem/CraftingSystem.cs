using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Crafting System Class
public class CraftingSystem : MonoBehaviour
{
    // Default Minecraft style 2x2 Grid
    public const int GRID_SIZE = 2;
    
    [SerializeField]
    private List<Recipe2x2> recipeList;
    public Item NoneItem;
    // Constructor
    public CraftingSystem(List<Recipe2x2> recipeList, InventorySlot[,] craftingGrid) {
        this.recipeList = recipeList;
    }

    public InventoryItem GetItem(int x, int y){
        Transform gridContainer = transform.Find("GridContainer");
        Transform inventorySlot = gridContainer.Find("grid_" + x + "_" + y);
        if(inventorySlot.childCount == 1){
            return inventorySlot.GetChild(0).GetComponent<InventoryItem>();
        }
        return null;
    }

    // try to craft every possible recipe and if items are in correct spot, return that recipe, otherwise false
    public (Item, int) TryCraftItem(){
        // for each craftable item, recipe is the 2x2 recipe
        foreach (Recipe2x2 recipe in recipeList){

            bool completeRecipe = true;

            for(int x = 0; x < GRID_SIZE; x++){
                for(int y=0; y < GRID_SIZE; y++) {
                    // Recipe has item in this position
                    if(GetItem(x,y) != null){
                        if (GetItem(x,y).item != recipe.GetItem(x,y)){
                            // Empty position or different itemType
                            completeRecipe = false;
                        }
                    } else {
                        if (recipe.GetItem(x,y) != NoneItem) {
                            completeRecipe = false;
                        }
                    }
                }
            }
            if(completeRecipe){
                return (recipe.output, recipe.outputCount);
            }
        }
        return (null, -1);
    }

    // decreases each item in the crafting slots by a certain count
    public void DecreaseItemCount(int decreaseCount){
        for(int x = 0; x < GRID_SIZE; x++){
            for(int y=0; y < GRID_SIZE; y++) {
                if(GetItem(x,y) != null){
                    InventoryItem item = GetItem(x,y);
                    if(item.item is ToolItem toolItem){ // decrease durability if tool
                        toolItem.durability -= 1;
                        if(toolItem.durability <= 0){
                            Destroy(item.gameObject);
                        }
                    } else {
                        item.SetCount(item.count - decreaseCount); // else decrease count
                        if(item.count <= 0){
                            Destroy(item.gameObject);
                        }
                    }
                }   
            }
        }
    }

    
}
