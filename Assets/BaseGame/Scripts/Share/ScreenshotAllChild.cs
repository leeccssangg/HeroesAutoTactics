using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class ScreenshotAllChild : MonoBehaviour
{
    public int resWidth = 1024;
    public int resHeight = 1024;
    
    private Camera m_Camera;
    public string m_Path = "D:\\Screenshot";
    public bool m_RandomRotate;

    public List<GameObject> m_AllObject;

    private void Awake()
    {
        m_Camera = Camera.main;
    }
    public void Start()
    {
        StartCoroutine(CoTakeSceenShot());
    }
    public void TakeScreenShot(GameObject gameObject)
    {
        gameObject.SetActive(true);
        if (m_RandomRotate)
        {
            gameObject.transform.eulerAngles = new Vector3(0, Random.Range(0, 360), 0);
        }
        if (!Directory.Exists(m_Path))
        {
            Directory.CreateDirectory(m_Path);
        }
        TakeTransparentScreenshot(m_Camera, resWidth, resHeight, ScreenShotPath(resWidth, resHeight, gameObject.name));

        gameObject.SetActive(false);
    }
    public void TakeShot(string objectName)
    {
        RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
        m_Camera.targetTexture = rt;
        Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
        m_Camera.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
        m_Camera.targetTexture = null;
        RenderTexture.active = null; // JC: added to avoid errors
        Destroy(rt);
        byte[] bytes = screenShot.EncodeToPNG();
        string filename = ScreenShotPath(resWidth, resHeight, objectName);
        System.IO.File.WriteAllBytes(filename, bytes);
        Debug.Log(string.Format("Took screenshot to: {0}", filename));
    }
    public string ScreenShotPath(int width, int height, string objectName)
    {
        return string.Format("{0}/{1}_{2}x{3}.png",
                     m_Path,
                     objectName,
                     width, height);
    }
    public void TakeTransparentScreenshot(Camera cam, int width, int height, string savePath)
    {
        // Depending on your render pipeline, this may not work.
        var bak_cam_targetTexture = cam.targetTexture;
        var bak_cam_clearFlags = cam.clearFlags;
        var bak_RenderTexture_active = RenderTexture.active;

        var tex_transparent = new Texture2D(width, height, TextureFormat.ARGB32, false);
        // Must use 24-bit depth buffer to be able to fill background.
        var render_texture = RenderTexture.GetTemporary(width, height, 24, RenderTextureFormat.ARGB32);
        var grab_area = new Rect(0, 0, width, height);

        RenderTexture.active = render_texture;
        cam.targetTexture = render_texture;
        cam.clearFlags = CameraClearFlags.SolidColor;

        // Simple: use a clear background
        cam.backgroundColor = Color.clear;
        cam.Render();
        tex_transparent.ReadPixels(grab_area, 0, 0);
        tex_transparent.Apply();

        // Encode the resulting output texture to a byte array then write to the file
        byte[] pngShot = ImageConversion.EncodeToPNG(tex_transparent);
        File.WriteAllBytes(savePath, pngShot);

        cam.clearFlags = bak_cam_clearFlags;
        cam.targetTexture = bak_cam_targetTexture;
        RenderTexture.active = bak_RenderTexture_active;
        RenderTexture.ReleaseTemporary(render_texture);
        Texture2D.Destroy(tex_transparent);

        Debug.Log(string.Format("Took screenshot to: {0}", savePath));
    }
    IEnumerator CoTakeSceenShot() 
    {
        Debug.Log("Start take screenshots");
        for (int i = 0; i < m_AllObject.Count; i++)
        {
            m_AllObject[i].SetActive(true);
            yield return new WaitForSeconds(1f);
            TakeScreenShot(m_AllObject[i]);
        }
        Debug.Log("End take screenshots");
    }
}
