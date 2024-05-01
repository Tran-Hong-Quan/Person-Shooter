using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendMessageAnimation : MonoBehaviour
{
    private void SendAttackMsg()
    {
        gameObject.SendMessage("OnAttackAnimation",options:SendMessageOptions.DontRequireReceiver);
    }
}
