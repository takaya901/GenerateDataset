using System.Collections;
using System.IO;
using UnityEngine;

public class DispDepth : MonoBehaviour
{
    /// <summary>カラー画像の保存先</summary>
    string SCREENSHOT_PATH;
    /// <summary>デプス画像の保存先</summary>
    string DEPTH_IMG_PATH;
    /// <summary>デプス表示用マテリアル</summary>
    [SerializeField] Material _depthMat;

    void Start()
    {
        SCREENSHOT_PATH = Application.dataPath + "/Dataset/ScreenShots/ss.png";
        DEPTH_IMG_PATH = Application.dataPath + "/Dataset/DepthImg/depth.png";

        GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;

        StartCoroutine(CaptureCoroutine());
    }

    //https://qiita.com/su10/items/a8f3f825155835de3d2a
    //ReadPixelsはWaitForEndOfFrameのあとで実行しなければいけないのでコルーチンで実行
    private IEnumerator CaptureCoroutine()
    {
        var texture = new Texture2D(Screen.width, Screen.height, TextureFormat.ARGB32, false);

        yield return new WaitForEndOfFrame();

        texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        texture.Apply();

        var bytes = texture.EncodeToPNG();
        File.WriteAllBytes(SCREENSHOT_PATH, bytes);

        Graphics.Blit(texture, null, _depthMat);

        texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        texture.Apply();

        bytes = texture.EncodeToPNG();
        Destroy(texture);
        File.WriteAllBytes(DEPTH_IMG_PATH, bytes);
    }
}