using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PreviewItemSlot : MonoBehaviour
{
    [SerializeField] ScaleImageWithTexture image;
    [SerializeField] TMP_Text itemAmountText;

    [HideInInspector] public InventoryItem item;
    public void SetData(InventoryItem item, System.Action<PreviewItemSlot> onTake)
    {
        this.item = item;
        image.SetSprite(item.InventoryItemData.sprite);
        if (item.amount > 1) itemAmountText.text = item.amount.ToString();
        GetComponent<Button>().onClick.AddListener(() => onTake?.Invoke(this));
    }
}
