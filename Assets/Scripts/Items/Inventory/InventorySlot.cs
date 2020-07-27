using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image icon;
    public GameObject count;
    private Text countText;

    ScriptableItem item;

    public void AddItem(ScriptableItem newItem)
    {
        item = newItem;
        icon.sprite = item.icon;
        icon.enabled = true;
        if(item.numberAvalible>1)
        {
            count.SetActive(true);
            countText = count.GetComponentInChildren<Text>();
            countText.text = item.numberAvalible.ToString();
        }
    }

    public void ClearSlot()
    {
        item = null;

        icon.sprite = null;
        icon.enabled = false;
    }

    public void UseItem()
    {
        if (item != null)
        {
            item.Use();
            if (item != null && item.numberAvalible > 1)
            {
                countText.text = item.numberAvalible.ToString();
            }
            else
            {
                count.SetActive(false);
            }
        }
    }
}
