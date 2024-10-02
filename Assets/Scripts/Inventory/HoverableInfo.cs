using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class HoverableInfo : MonoBehaviour{
    private TextMeshProUGUI text;

    public void Start(){
        text = transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>();
        gameObject.SetActive(false);
    }
    public void UpdateText(Item item){
        if(item is BlockItem blockItem){
            text.text = blockItem.ItemName;
        } else if (item is ToolPartItem toolPartItem){
            text.text = item.ItemName + 
            "\n+" + toolPartItem.addedDurability + " Durability" + 
            "\n+" + toolPartItem.speedMultiplier + " Mining Speed";
        } else if (item is ToolItem toolItem){
            if(toolItem.maxDurability > 0){
                text.text = item.ItemName + 
                "\n" + toolItem.durability + "/" + toolItem.maxDurability + " Durability" + 
                "\n" + toolItem.speedMultiplier + " Mining Speed";
            } else {
                text.text = item.ItemName + 
                "\n" + toolItem.durability + "/" + toolItem.CalculateMaxDurability() + " Durability" + 
                "\n" + toolItem.speedMultiplier + " Mining Speed";
            }
        }
    }
}   