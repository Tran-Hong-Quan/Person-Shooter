using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniversalInventorySystem;

public class PlayerPreviewItems : PreviewItems
{
    [SerializeField] PlayerController playerController;
    [SerializeField] PreviewItemSlot slotPrefab;
    [SerializeField] Transform content;

    [SerializeField] List<PreviewItemSlot> slots = new List<PreviewItemSlot>();

    private void Awake()
    {
        if (playerController == null) playerController = GetComponent<PlayerController>();
    }
    protected override void AddItem(InventoryItem item)
    {
        base.AddItem(item);
        var slot = Instantiate(slotPrefab, content);
        slot.gameObject.SetActive(true);
        slot.SetData(item, TakeItemFromUI);
        slots.Add(slot);
    }

    public void TakeItemFromUI(PreviewItemSlot slot)
    {
        Debug.Log("Take Item");
        playerController.Inventory.AddItem(slot.item.InventoryItemData, slot.item.amount);
        TakeItem(slot.item);
        slots.Remove(slot);
        Destroy(slot.gameObject);
    }

    protected override void RemoveItemFromPrevew(InventoryItem item)
    {
        base.RemoveItemFromPrevew(item);
        foreach(var slot  in slots)
        {
            if(slot.item == item)
            {
                slots.Remove(slot);
                Destroy(slot.gameObject);
                break;
            }
        }
    }
}
