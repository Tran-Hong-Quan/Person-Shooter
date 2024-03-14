using HongQuan;
using UnityEngine;
using UniversalInventorySystem;

public class InventoryItem : MonoBehaviour
{
    [SerializeField] protected Item inventoryItemData;
    [SerializeField] protected InventoryItem prefab;

    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public Collider col;
    [HideInInspector] public Inventory inventory;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void OnUse(object sender, InventoryHandler.UseItemEventArgs e)
    {
        e.inv.RemoveItem(e.item, 1);
    }

    public void OnDrop(object sender, InventoryHandler.UseItemEventArgs e)
    {
        InventoryItem invItem = SimplePool.Spawn(prefab);
        invItem.transform.position = e.inv.parent.transform.position;
        invItem.rb.AddForce(e.inv.parent.transform.forward * 2, ForceMode.VelocityChange);
        invItem.inventory = e.inv;
        e.inv.parent.DelayFuction(1, () => invItem.inventory = null);
    }

    public void AddItemToInventory(Inventory inventory)
    {
        if (this.inventory == inventory) return;
        this.inventory = inventory;
        this.inventory.AddItem(inventoryItemData, 1);
        SimplePool.Despawn(gameObject);
    }
}
