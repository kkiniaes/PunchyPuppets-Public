using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenshotUtil : MonoBehaviour {
    private Camera screenshotCamera;

    public int width = 3840, height = 2160;

    private void Start()
    {
        screenshotCamera = GetComponent<Camera>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Period))
        {
            StartCoroutine(TakeScreenshot());
        }
    }

    private IEnumerator TakeScreenshot()
    {
        yield return new WaitForEndOfFrame();
        RenderTexture rt = new RenderTexture(width, height, 24);
        screenshotCamera.targetTexture = rt;

        Texture2D screenshot = new Texture2D(width, height, TextureFormat.ARGB32, false);
        screenshotCamera.Render();
        RenderTexture.active = rt;
        screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);

        screenshotCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);

        byte[] bytes = screenshot.EncodeToPNG();

        string filename = Application.persistentDataPath + "/screenshot_" + width.ToString() + "x" + height.ToString() + "_" +
                          System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";
        System.IO.File.WriteAllBytes(filename, bytes);

        Debug.Log(filename);
    }
}
