using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace QuanUtilities
{
    public class AddressableHelper : MonoBehaviour
    {
        public void Instantiate<T>(string path,Action<T> onDone) where T : Component
        {
            StartCoroutine(LoadedInstantiate<T>(path,onDone));
        }

        IEnumerator LoadedInstantiate<T>(string path,Action<T> onDone) where T : Component
        {
            AsyncOperationHandle<T> goHandle = Addressables.LoadAssetAsync<T>(path);
            yield return goHandle;
            if (goHandle.Status == AsyncOperationStatus.Succeeded)
            {
                T obj = goHandle.Result;
                Instantiate(obj);
                onDone?.Invoke(obj);
            }

            //Addressables.Release(goHandle);
        }

        public void Load<T>(string path, Action<T> onDone)
        {
            StartCoroutine(Loaded(path, onDone));
        }

        IEnumerator Loaded<T>(string path, Action<T> scriptAction)
        {
            AsyncOperationHandle<T> goHandle = Addressables.LoadAssetAsync<T>(path);
            yield return goHandle;
            if (goHandle.Status == AsyncOperationStatus.Succeeded)
            {
                T obj = goHandle.Result;
                scriptAction(obj);
            }

            //Addressables.Release(goHandle);
        }
        /// <summary>
        /// You can use this fuction to download data from server, or fetch all data of packet/group in to ram
        /// </summary>
        /// <param name="path">The path of any child in packet/group </param>
        /// <param name="onLoading">Update per frame and return percentage</param>
        /// <param name="onComplete">Call when complete load all packet/group</param>
        public void LoadPacket(string path, Action<float> onLoading, Action<bool> onComplete)
        {
            StartCoroutine(StartLoadPacket(path, onLoading, onComplete));   
        }

        IEnumerator StartLoadPacket(string path, Action<float> onLoading, Action<bool> onComplete)
        {
            AsyncOperationHandle<GameObject> go = Addressables.LoadAssetAsync<GameObject>(path);

            while (!go.IsDone)
            {
                float percent = go.GetDownloadStatus().Percent;
                Debug.Log("percent: " + percent);
                onLoading?.Invoke(percent);
                yield return null;
            }

            switch (go.Status)
            {
                case AsyncOperationStatus.Succeeded:
                    onComplete?.Invoke(true);
                    break;
                default:
                    onComplete?.Invoke(false);
                    break;
            }

            //Addressables.Release(go);
        }
    }
}
