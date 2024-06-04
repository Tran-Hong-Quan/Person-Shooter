using HongQuan;
using UnityEngine;
using UnityEngine.Events;
using UniversalInventorySystem;

public class InventoryItem : MonoBehaviour
{
    public int amount;
    [SerializeField] protected Item inventoryItemData;

    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public Collider col;
    [HideInInspector] public Inventory inventory;

    public UnityEvent<InventoryItem, PreviewItems> onBeingTaken;

    public Item InventoryItemData => inventoryItemData;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        if (inventoryItemData.onUse == null)
        {
            inventoryItemData.onUse = OnUseItem;
        }
        if (inventoryItemData.onDrop == null)
        {
            inventoryItemData.onDrop = OnDropItem;
        }
    }

    private void OnEnable()
    {
        SpawnItemOnFloor();
    }

    public void SpawnItemOnFloor(Vector3? pos = null, Quaternion? rot = null)
    {
        if (pos == null) pos = transform.position;
        if (rot == null) rot = Quaternion.identity;

        Ray ray = new Ray(pos.Value, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit, 100, InventoryHandler.current.dropItemLayermask))
        {
            transform.position = hit.point;
            transform.localScale = Vector3.one;
            transform.rotation = rot.Value;

            //Debug.Log(hit.transform.name);

            rb.isKinematic = true;
            col.isTrigger = true;
        }
        else
        {
            rb.isKinematic = false;
            col.isTrigger = false;
        }
    }

    public void Init(Item inventoryItemData, int amount = 1)
    {
        this.inventoryItemData = inventoryItemData;
        this.amount = amount;
    }

    public void AddItemToInventory(Inventory inventory)
    {
        if (this.inventory == inventory) return;
        this.inventory = inventory;
        this.inventory.AddItem(inventoryItemData, amount);
        SimplePool.Despawn(gameObject);
    }

    public void OnBeingTaken(PreviewItems preview)
    {
        onBeingTaken?.Invoke(this, preview);
    }

    protected virtual void OnUseItem(InventoryHandler.UseItemEventArgs dea)
    {
        DeleteItem(dea);
    }

    public void DeleteItem(InventoryHandler.UseItemEventArgs dea)
    {
        dea.inv.RemoveItemInSlot(dea.slot, 1);
    }

    protected void OnDropItem(InventoryHandler.DropItemEventArgs dea)
    {
        dea.inv.RemoveItemInSlot(dea.slot, dea.amount);
        if (dea.inv.parent == null) return;
        InventoryItem invItem = SimplePool.Spawn(InventoryHandler.current.itemPrefabs[dea.item]);
        invItem.transform.position = dea.inv.parent.transform.position + Vector3.up + dea.inv.parent.transform.forward;
        invItem.Init((Item)dea.item, (int)dea.amount);
        invItem.SpawnItemOnFloor(rot: Quaternion.Euler(0, (float)dea.inv.parent.transform.eulerAngles.y, 0));
        invItem.inventory = dea.inv;
        dea.inv.parent.DelayFunction(1, () => invItem.inventory = null);
    }
}
