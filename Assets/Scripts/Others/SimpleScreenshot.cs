using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
public static class SimpleScreenshot
{
    private static ScreenshotData screenshotData;

    public static Sprite TakeScreenshot(Camera cam)
    {
        RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        var tTex = cam.targetTexture;
        cam.targetTexture = renderTexture;
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = renderTexture;

        cam.Render();

        Texture2D screenshot = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        screenshot.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        screenshot.Apply();

        RenderTexture.active = currentRT;
        cam.targetTexture = tTex;

        return Sprite.Create(screenshot, new Rect(0, 0, screenshot.width, screenshot.height), Vector2.one / 2, 100);
    }

    public static Sprite TakeScreenshotWithDefaultRenderTexture(Camera cam)
    {
        RenderTexture renderTexture = cam.targetTexture;
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = renderTexture;

        cam.Render();

        Texture2D screenshot = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
        screenshot.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        screenshot.Apply();

        RenderTexture.active = currentRT;

        return Sprite.Create(screenshot, new Rect(0, 0, screenshot.width, screenshot.height), Vector2.one / 2, 100);
    }

    public static Sprite TakeScreenshot()
    {
        var texture = ScreenCapture.CaptureScreenshotAsTexture();
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one / 2, 100);
    }

    public static string SaveSprite(Sprite sprite, string name)
    {
        LoadScreenshotData();
        string fileName = $"{name}_{DateTime.Now.Ticks}.png";
        string dataPath = $"{Application.persistentDataPath}/{fileName}";
        screenshotData.fileNames.Add(fileName);
        File.WriteAllBytes(dataPath, sprite.texture.EncodeToPNG());
        SaveScreenshotData();
        LoadScreenshotData();
        return fileName;
    }

    public static Sprite LoadSpriteData(string fileName)
    {
        var imageData = File.ReadAllBytes($"{Application.persistentDataPath}/{fileName}");
        var texture = new Texture2D(Screen.width, Screen.height, TextureFormat.ARGB32, false);
        texture.LoadImage(imageData);
        var res = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one / 2, 100);
        res.name = fileName;
        return res;
    }
    public static Texture2D LoadTextureData(string fileName)
    {
        var imageData = File.ReadAllBytes($"{Application.persistentDataPath}/{fileName}");
        var texture = new Texture2D(Screen.width, Screen.height, TextureFormat.ARGB32, false);
        texture.LoadImage(imageData);
        var res = texture;
        res.name = fileName;
        return res;
    }

    public static Sprite[] GetAllScreenshot(string name)
    {
        LoadScreenshotData();
        List<Sprite> result = new List<Sprite>();
        foreach (string fileName in screenshotData.fileNames)
        {
            if (!fileName.Contains(name)) continue;

            result.Add(LoadSpriteData(fileName));
        }
        return result.ToArray();
    }
    public static Texture2D[] GetAllScreenshotTexture(string name)
    {
        LoadScreenshotData();
        List<Texture2D> result = new List<Texture2D>();
        foreach (string fileName in screenshotData.fileNames)
        {
            if (!fileName.Contains(name)) continue;

            result.Add(LoadTextureData(fileName));
        }
        return result.ToArray();
    }


    public static void GetAllScreenshotWithNames(string name, out List<Sprite> sprites, out List<string> names)
    {
        LoadScreenshotData();
        sprites = new List<Sprite>();
        names = new List<string>();
        foreach (string fileName in screenshotData.fileNames)
        {
            if (!fileName.Contains(name)) continue;

            sprites.Add(LoadSpriteData(fileName));
            names.Add(fileName);
        }

    }

    public static void DeleteImage(string fileName)
    {
        LoadScreenshotData();
        string dataPath = $"{Application.persistentDataPath}/{fileName}";
        if (File.Exists(dataPath))
        {
            File.Delete(dataPath);
            screenshotData.fileNames.Remove(fileName);
            SaveScreenshotData();
        }
    }

    private static void SaveScreenshotData()
    {
        for (int i = 0; i < screenshotData.fileNames.Count; i++)
        {
            var name = screenshotData.fileNames[i];
            if (!IsExists(name))
            {
                screenshotData.fileNames.Remove(name);
                i--;
            }
        }
        string data = JsonUtility.ToJson(screenshotData);
        SaveData(data, "AllImageNames");
    }

    private static void LoadScreenshotData()
    {
        string data = LoadData("AllImageNames");
        if (data == "")
        {
            screenshotData.fileNames = new List<string>();
            return;
        }
        screenshotData = JsonUtility.FromJson<ScreenshotData>(data);
        if (screenshotData.fileNames == null)
            screenshotData.fileNames = new List<string>();
        for (int i = 0; i < screenshotData.fileNames.Count; i++)
        {
            var name = screenshotData.fileNames[i];
            if (!IsExists(name))
            {
                screenshotData.fileNames.Remove(name);
                i--;
            }
        }
    }

    private static void SaveData(string data, string fileName)
    {
        string dataPath = $"{Application.persistentDataPath}/{fileName}.txt";

        File.WriteAllText(dataPath, data);
    }
    private static string LoadData(string fileName)
    {
        string dataPath = $"{Application.persistentDataPath}/{fileName}.txt";

        if (File.Exists(dataPath))
            return File.ReadAllText(dataPath);
        else
            return "";
    }

    private static bool IsExists(string fileName)
    {
        return File.Exists($"{Application.persistentDataPath}/{fileName}");
    }
    [Serializable]
    public struct ScreenshotData
    {
        public List<string> fileNames;
    }

}
