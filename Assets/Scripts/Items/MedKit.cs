using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniversalInventorySystem;

public class MedKit : MonoBehaviour
{
    [SerializeField] protected Item inventoryItemData;

    private void Awake()
    {
        inventoryItemData.onUse.AddListener(OnUse);
        inventoryItemData.onDrop.AddListener(OnDrop);
    }

    private void OnUse(UseItemData data)
    {

    }

    private void OnDrop(DropItemData data)
    {

    }
}
