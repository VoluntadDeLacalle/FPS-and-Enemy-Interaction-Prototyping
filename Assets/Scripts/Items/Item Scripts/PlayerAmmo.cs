using UnityEngine;

[CreateAssetMenu(fileName = "Arrow", menuName = "Item/Ammo")]
public class PlayerAmmo : ScriptableItem
{
    public override void Use()
    {
        Debug.Log("Item can't be used like this.");
    }
}
