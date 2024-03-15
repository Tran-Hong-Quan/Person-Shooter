using HongQuan;
using UnityEngine;
using UniversalInventorySystem;

public class InventoryItem : MonoBehaviour
{
    [SerializeField] protected Item inventoryItemData;
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public Collider col;
    [HideInInspector] public Inventory inventory;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Init(Item inventoryItemData)
    {
        this.inventoryItemData = inventoryItemData;
    }

    public void AddItemToInventory(Inventory inventory)
    {
        if (this.inventory == inventory) return;
        this.inventory = inventory;
        this.inventory.AddItem(inventoryItemData, 1);
        SimplePool.Despawn(gameObject);
    }
}
