using UnityEngine;

[CreateAssetMenu(fileName = "Torch", menuName = "Item/Torch")]
public class Torch : ScriptableItem
{
    public override void Use()
    {
        base.Use();

        RemoveFromInventory();
    }
}
