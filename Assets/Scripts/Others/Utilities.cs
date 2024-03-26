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

        public static Tweener SmoothLayerMask(this Animator animator, int id, float to, float duration = 0.5f, TweenCallback onDone = null)
        {
            return DOVirtual.Float(animator.GetLayerWeight(id), to, duration, value =>
            {
                animator.SetLayerWeight(id, value);
            }).OnComplete(onDone).SetEase(Ease.Linear);
        }

        public static Tweener SmoothRig(this Rig rig, float to, float duration = 0.1f, TweenCallback onDone = null)
        {
            return DOVirtual.Float(rig.weight, to, duration, value =>
            {
                rig.weight = value;
            }).OnComplete(onDone).SetEase(Ease.Linear);
        }
        public static bool GetRandomSpawnPoint(Vector3 spawnCenter, float spawnRadious, LayerMask spawnLayermask, out Vector3 point)
        {
            Vector3 castCenter = spawnCenter + Vector3.up * 50f;
            castCenter += spawnRadious * new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
            if (Physics.Raycast(castCenter, Vector3.down, out RaycastHit hitInfo, 100, spawnLayermask))
            {
                point = hitInfo.point;
                return true;
            }
            point = default;
            return false;
        }
    }

    public delegate void NoParamaterDelegate();
}


