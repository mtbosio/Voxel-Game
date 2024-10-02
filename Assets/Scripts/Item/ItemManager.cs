using System.Collections.Generic;
using UnityEngine;
 
public class ItemManager : MonoBehaviour
{
    public List<Item> itemList;
    public Dictionary<int, Item> itemDictionary = new Dictionary<int, Item>();
       
    public static ItemManager instance;
    
    public void Awake()
    {
        instance = this;
    }
    public void Start(){
        foreach(Item item in itemList){
            itemDictionary[item.ItemId] = item;
        }
    }
    // find a blockitem from a blocktype
    public BlockItem blockToItem(BlockType blockType){
        foreach(var pair in itemDictionary){
            if (pair.Value is BlockItem blockItem && blockItem.blockType == blockType){
                return blockItem;
            }
        }
        return null;
    }

}