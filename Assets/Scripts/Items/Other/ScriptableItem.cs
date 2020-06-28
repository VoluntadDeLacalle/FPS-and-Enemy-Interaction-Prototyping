using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Item")]
public class ScriptableItem : ScriptableObject
{
    public string ItemName = "New Item";
    public Sprite icon = null;
    public bool stackable = true;

    public virtual void Use()
    {
        //use item
        //add effect here

        Debug.Log("Used " + ItemName + ".");
    }
}
