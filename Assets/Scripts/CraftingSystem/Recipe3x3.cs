using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Recipe/3x3")]
public class Recipe3x3 : ScriptableObject
{
    public Item output;

    public Item item_00;
    public Item item_10;
    public Item item_20;

    public Item item_01;
    public Item item_11;
    public Item item_21;

    public Item item_02;
    public Item item_12;
    public Item item_22;
}
