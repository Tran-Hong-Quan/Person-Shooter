using UnityEngine;
using UnityEngine.UI;

public class TestingTimeScale : MonoBehaviour
{
    [SerializeField] private Text timeScaleText;

    public void DecreaseTimeScale()
    {
        float timeScale = Time.timeScale;
        timeScale--;
        if (timeScale > 0)
            Time.timeScale = timeScale;
        UpdateText();
    }

    public void IncreaseTimeScale()
    {
        Time.timeScale++;
        UpdateText();
    }

    private void UpdateText()
    {
        timeScaleText.text = "X" + Time.timeScale.ToString();
    }

    public void SetTimeScale(float timeScale)
    {
        Time.timeScale = timeScale;
        UpdateText();
    }    

    private void OnEnable()
    {
        UpdateText();
    }
}
