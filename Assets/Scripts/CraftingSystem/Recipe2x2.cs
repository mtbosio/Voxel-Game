using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Recipe/2x2")]
public class Recipe2x2 : ScriptableObject
{
    public Item output;
    public int outputCount;

    public Item item_00;
    public Item item_10;

    public Item item_01;
    public Item item_11;

    public Item GetItem(int x, int y){
        if(x==0 && y==0) return item_00;
        if(x==0 && y==1) return item_01;
        
        if(x==1 && y==0) return item_10;
        if(x==1 && y==1) return item_11;

        return null;
    }
}
