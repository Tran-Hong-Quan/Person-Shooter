using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace HongQuan
{
    public static class Utilities
    {
        public static IEnumerator DelayFunction(this MonoBehaviour mono, float delay, NoParamaterDelegate fuction, bool ignoreTimeScale = false)
        {
            var res = StartDelayFuction(delay, fuction);
            mono.StartCoroutine(res);
            return res;
        }

        static IEnumerator StartDelayFuction(float time, NoParamaterDelegate fuction, bool ignoreTimeScale = false)
        {
            if (ignoreTimeScale)
                yield return new WaitForSecondsRealtime(time);
            else
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

        public static T RandomElement<T>(this T[] list)
        {
            return list[UnityEngine.Random.Range(0, list.Length)];
        }

        public static T RandomElement<T>(this List<T> list)
        {
            return list[UnityEngine.Random.Range(0, list.Count)];
        }

        public static int RandomElementIndex<T>(this T[] list)
        {
            return UnityEngine.Random.Range(0, list.Length);
        }

        /// <summary>
        /// Use this if EventSystem.current.IsPointerOverGameObject() not work, this often occur in mobile
        /// </summary>
        /// <returns></returns>

        public static bool IsPointerOverUIObject()
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            //eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y); //Old Input System
            eventDataCurrentPosition.position = Mouse.current.position.value; //New Input System
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0;
        }

        public static bool IsOverlapBox2D(this BoxCollider2D box1, BoxCollider2D box2)
        {
            return box1.bounds.Intersects(box2.bounds);
        }

        public static void SaveData(string data, string fileName)
        {
            string dataPath = $"{Application.persistentDataPath}/{fileName}.txt";

            File.WriteAllText(dataPath, data);
        }
        public static string LoadData(string fileName)
        {
            string dataPath = $"{Application.persistentDataPath}/{fileName}.txt";

            if (File.Exists(dataPath))
                return File.ReadAllText(dataPath);
            else
                return "";
        }

        #region Camera Position

        public static float BottomBorder(this Camera cam)
        {
            return cam.transform.position.y - cam.orthographicSize;
        }
        public static float TopBorder(this Camera cam)
        {
            return cam.transform.position.y + cam.orthographicSize;
        }
        public static float RightBorder(this Camera cam)
        {
            return cam.transform.position.x + cam.orthographicSize * cam.aspect;
        }
        public static float LeftBorder(this Camera cam)
        {
            return cam.transform.position.x - cam.orthographicSize * cam.aspect;
        }
        public static Vector2 UpRightCorner(this Camera cam)
        {
            return new Vector2(RightBorder(cam), TopBorder(cam));
        }
        public static Vector2 UpLeftCorner(this Camera cam)
        {
            return new Vector2(LeftBorder(cam), TopBorder(cam));
        }
        public static Vector2 DownLeftCorner(this Camera cam)
        {
            return new Vector2(LeftBorder(cam), BottomBorder(cam));
        }
        public static Vector2 DownRightCorner(this Camera cam)
        {
            return new Vector2(RightBorder(cam), BottomBorder(cam));
        }
        #endregion
    }

    public delegate void NoParamaterDelegate();

    public class JsonHelper
    {
        public static T[] FromJsonArray<T>(string json)
        {
            string newJson = "{ \"array\": " + json + "}";
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
            return wrapper.array;
        }

        public static string ToJsonArray<T>(T[] data)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.array = data;
            string jsonData = JsonUtility.ToJson(wrapper);
            Debug.Log(jsonData);
            return jsonData.Substring(9, jsonData.Length - 10);
        }

        [Serializable]
        private class Wrapper<T>
        {
            public T[] array;
        }
    }
}


