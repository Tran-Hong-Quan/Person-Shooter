using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniversalInventorySystem;

public class MedKitItem : InventoryItem
{
    [SerializeField] float regenration = 20;
    protected override void OnUseItem(InventoryHandler.UseItemEventArgs dea)
    {
        print("Use Med kit");
        if (dea.inv.parent != null)
        {
            if (dea.inv.parent.TryGetComponent(out IHealth healthable))
            {
                healthable.Regeneration(regenration, null);
            }
        }
        base.OnUseItem(dea);
    }
}
