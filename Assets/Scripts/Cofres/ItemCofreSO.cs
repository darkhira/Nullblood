using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item Data")]
public class ItemCofreSO : ScriptableObject
{
    public string itemName = "New Item";
    public Sprite itemIcon = null;
    [TextArea(3, 5)]
    public string itemDescription = "";
    // Add other item properties if needed (e.g., item type, stack size, effects)
}
