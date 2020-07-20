using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;
    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Andrew failed to make an instance please let him know");
            return;
        }
        instance = this;
    }

    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallBack;

    public int space = 20;

    public List<ScriptableItem> items = new List<ScriptableItem>();

    public bool Add(ScriptableItem item)
    {
        if (items.Contains(item) && item.stackable)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] == item)
                {
                    items[i].numberAvalible++;
                    Debug.Log("You have " + items[i].numberAvalible + " " + items[i].name + " now.");
                }
            }
        }
        else
        {
            if (items.Count >= space)
            {
                Debug.Log("Inventory Full");
                return false;
            }
            item.numberAvalible = 1;
            items.Add(item);
        }
        if (onItemChangedCallBack != null)
        {
            onItemChangedCallBack.Invoke();
        }
        return true;
    }
    public void Remove(ScriptableItem item)
    {
        items.Remove(item);
        if (onItemChangedCallBack != null)
        {
            onItemChangedCallBack.Invoke();
        }
    }
}
