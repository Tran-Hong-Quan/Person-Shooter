using HongQuan;
using UnityEngine;
using UniversalInventorySystem;

public class InventoryItemUseDrop
{
    public void OnUse(object sender, InventoryHandler.UseItemEventArgs e)
    {
        Debug.Log("Use");
        e.inv.RemoveItemInSlot(e.slot, 1);
    }

    public void OnDropItem(object sender, InventoryHandler.DropItemEventArgs e)
    {
        Debug.Log("Drop");
        e.inv.RemoveItemInSlot(e.slot, e.amount);
        if (e.inv.parent == null) return;
        InventoryItem invItem = SimplePool.Spawn(InventoryHandler.current.itemPrefabs[e.item]);
        invItem.transform.position = e.inv.parent.transform.position + Vector3.up + e.inv.parent.transform.forward;
        invItem.Init(e.item, e.amount);
        invItem.SpawnItemOnFloor(rot: Quaternion.Euler(0, e.inv.parent.transform.eulerAngles.y, 0));
        invItem.inventory = e.inv;
        e.inv.parent.DelayFunction(1, () => invItem.inventory = null);
    }
}
