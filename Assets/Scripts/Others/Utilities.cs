using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

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

        public static void SmoothLayerMask(this Animator animator, string name, float to, float duration = 0.5f, TweenCallback onDone = null)
        {
            int id = animator.GetLayerIndex(name);
            SmoothLayerMask(animator, id, to, duration, onDone);
        }

        public static void SmoothLayerMask(this Animator animator, int id, float to, float duration = 0.5f, TweenCallback onDone = null)
        {
            DOVirtual.Float(animator.GetLayerWeight(id), to, duration, value =>
            {
                animator.SetLayerWeight(id, value);
            }).OnComplete(onDone).SetEase(Ease.Linear);
        }

        public static void SmoothRig(this Rig rig, float to, float duration = 0.1f, TweenCallback onDone = null)
        {
            DOVirtual.Float(rig.weight, to, duration, value =>
            {
                rig.weight = value;
            }).OnComplete(onDone).SetEase(Ease.Linear);
        }
    }

    public delegate void NoParamaterDelegate();
}


