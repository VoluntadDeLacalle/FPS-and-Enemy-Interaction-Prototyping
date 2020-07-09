using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Item")]
public class ScriptableItem : ScriptableObject
{
    public string ItemName = "New Item";
    public Sprite icon = null;
    public bool stackable = true;
    //should be 1 by default
    public int numberAvalible = 1;


    public virtual void Use()
    {
        //use item
        //add effect here

        Debug.Log("Used " + ItemName + ".");
    }
}
