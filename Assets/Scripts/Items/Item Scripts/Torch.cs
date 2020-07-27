using UnityEngine;

[CreateAssetMenu(fileName = "Torch", menuName = "Item/Torch")]
public class Torch : ScriptableItem
{
    private GameObject torchLight;
    public int maxRange;
    public float torchDuration;
    private TorchTimer timer;

    public override void Use()
    {
        //Finds the light and turns it on
        torchLight = GameObject.FindGameObjectWithTag("Torch Light");
        if (torchLight != null)
        {
            timer = torchLight.GetComponent<TorchTimer>();
            //Removes one use of the item from inventory
            base.Use();
            RemoveFromInventory();
            //Timer for the torch's duration
            timer.Info(torchDuration, maxRange);
        }
        else
        {
            Debug.LogError("Failed to find \"Torch Light\"");
        }
    }
}
