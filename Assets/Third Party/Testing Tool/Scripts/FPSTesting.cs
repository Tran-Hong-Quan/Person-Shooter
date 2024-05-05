using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FPSTesting : MonoBehaviour
{
    [SerializeField] private Text fpsText;

    private IEnumerator Start()
    {
        while (true)
        {
            fpsText.text = ((int)(1f / Time.unscaledDeltaTime)).ToString() + " FPS";
            yield return new WaitForSecondsRealtime(0.1f);
        }
    }
}
