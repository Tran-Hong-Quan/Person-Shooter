using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralRecoil : MonoBehaviour
{
    public Vector3 recoilForce = Vector3.one;
    public Game.CharacterController character;

    private void Awake()
    {
        recoilForce.z = 0;
    }

    public void Init(Game.CharacterController character)
    {
        this.character = character;
    }

    public void FireRecoil()
    {
        //character.Recoil(recoilForce*Time.deltaTime);
    }
}
