using System.Collections;
using System.IO;
using UnityEngine;

public class DispDepth : MonoBehaviour
{
    [SerializeField] Material _depthMat;    //デプス表示用マテリアル
    const string SCREENSHOT_PATH = "C:/Unity/Dataset/ScreenShots/"; //スクリーンショットの保存先
    const string DEPTH_IMG_PATH = "C:/Unity/Dataset/DepthImg/";  //デプス画像の保存先

    void Start()
    {
        GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;  //カメラがデプステクスチャを生成するモード
        StartCoroutine(CaptureCoroutine());
    }
    
    //ReadPixelsはWaitForEndOfFrameのあとで実行しなければいけないのでコルーチンで実行 https://qiita.com/su10/items/a8f3f825155835de3d2a
    IEnumerator CaptureCoroutine()
    {
        var texture = new Texture2D(Screen.width, Screen.height, TextureFormat.ARGB32, false);

        yield return new WaitForEndOfFrame();

        //スクリーンショットを保存
        texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        texture.Apply();
        var bytes = texture.EncodeToPNG();
        File.WriteAllBytes(SCREENSHOT_PATH + "1.png", bytes);

        Graphics.Blit(texture, null, _depthMat);    //ポストエフェクトでデプス画像に変換する

        //デプス画像を保存
        texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        texture.Apply();
        bytes = texture.EncodeToPNG();
        Destroy(texture);
        File.WriteAllBytes(DEPTH_IMG_PATH + "1.png", bytes);
    }
}