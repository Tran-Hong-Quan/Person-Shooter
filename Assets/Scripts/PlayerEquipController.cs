using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipController : EquipController
{
    private int sortingEquipIndex = 0;

    private void ChangeEquipment(float dir)
    {
        if(dir < 0)
        {
            BackToNextItem();
        }
        else
        {
            JumpToNextEquipment();
        }

    }

    private void JumpToNextEquipment()
    {
    }

    private void BackToNextItem()
    {

    }
}
