using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game
{
    public class Entity : MonoBehaviour
    {
        [Header("Entity Fields")]
        public UnityEvent<Entity> onDie;
    }

}

