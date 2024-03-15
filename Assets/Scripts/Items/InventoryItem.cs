using HongQuan;
using UnityEngine;
using UniversalInventorySystem;

public class InventoryItem : MonoBehaviour
{
    public int amount;

    [SerializeField] protected Item inventoryItemData;

    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public Collider col;
    [HideInInspector] public Inventory inventory;

    public Item InventoryItemData => inventoryItemData;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
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

            Debug.Log(hit.transform.name);

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
}
