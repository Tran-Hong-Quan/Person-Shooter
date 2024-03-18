using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewItems : MonoBehaviour
{
    public List<InventoryItem> items = new List<InventoryItem>();

    protected virtual void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("InventoryItem")) return;
        if (!other.TryGetComponent(out InventoryItem item)) return;
        if (items.Contains(item)) return;
        AddItem(item);
        item.onBeingTaken.AddListener(OnTakeItem);
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("InventoryItem")) return;
        if (!other.TryGetComponent(out InventoryItem item)) return;
        RemoveItemFromPrevew(item);
    }

    protected virtual void AddItem(InventoryItem item)
    {
        items.Add(item);
    }

    public virtual void TakeItem(int location)
    {
        if (location < 0) return;
        if (items.Count == 0) return;
        if (items.Count <= location) return;
        items[location].OnBeingTaken(this);
        RemoveItemFromPrevew(location);
    }

    public virtual void TakeItem(InventoryItem item)
    {
        item.OnBeingTaken(this);
        SimplePool.Despawn(item.gameObject);
    }

    protected virtual void OnTakeItem(InventoryItem item, PreviewItems caller)
    {
        item.onBeingTaken.RemoveListener(OnTakeItem);
        RemoveItemFromPrevew(item);
    }

    protected virtual void RemoveItemFromPrevew(InventoryItem item)
    {
        items.Remove(item);
    }
    protected virtual void RemoveItemFromPrevew(int location)
    {
        items.RemoveAt(location);
    }
}
