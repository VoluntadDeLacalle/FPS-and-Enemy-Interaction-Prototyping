using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickUp : MonoBehaviour
{
    //public varables
    public Text Ptext;
    public ScriptableItem item;
    //private varables
    private bool inRange = false;

    void Start()
    {
        Ptext.text = "";
    }
   
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && inRange)
        {
            //add the item to the invantory and delete it
            if (Inventory.instance.Add(item))
            {
                Ptext.text = "";
                Destroy(this.gameObject);
            }
            Debug.Log("pick it up");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
       if (other.CompareTag("Player"))
        {
            inRange = true;
            Ptext.text = "Press E to pickup " + item.ItemName + ".";
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inRange = false;
            Ptext.text = "";
        }
    }
}
