using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipGunForPlayer : MonoBehaviour
{
    [SerializeField] PlayerController player;
    [SerializeField] Gun[] gunPrefabs;

    private void Start()
    {
        for (int i = 0; i < gunPrefabs.Length; i++)
        {
            var gun = Instantiate(gunPrefabs[i]);
            gun.Equip(player.EquipController);
        }
    }
}
