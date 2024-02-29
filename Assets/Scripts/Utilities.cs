using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HongQuan
{
    public static class Utilities
    {
        public static IEnumerator DelayFuction(this MonoBehaviour mono, float delay, NoParamaterDelegate fuction)
        {
            var res = StartDelayFuction(delay, fuction);
            mono.StartCoroutine(res);
            return res;
        }

        static IEnumerator StartDelayFuction(float time, NoParamaterDelegate fuction)
        {
            yield return new WaitForSeconds(time);
            fuction?.Invoke();
        }
    }

    public delegate void NoParamaterDelegate();
}


