using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;

public class DispDepth : MonoBehaviour
{
    [SerializeField] Material _depthMat;    //デプス表示用マテリアル
    [SerializeField] SetScene _setScene;

    const string SCREENSHOT_PATH = "C:/Unity/Dataset/ScreenShots/"; //スクリーンショットの保存先
    const string DEPTH_IMG_PATH = "C:/Unity/Dataset/DepthImg/";  //デプス画像の保存先
    const string EXTENSION = ".png";

    void Start()
    {
        GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;  //カメラがデプステクスチャを生成するモード

        StartCoroutine(CaptureScreenshot());
        //StartCoroutine(CaptureDepth());
    }

    //スクリーンショットを保存
    //ReadPixelsはWaitForEndOfFrameのあとで実行しなければいけないのでコルーチンで実行 https://qiita.com/su10/items/a8f3f825155835de3d2a
    IEnumerator CaptureScreenshot()
    {
        for (int i = 0; i < 10; i++) {
            var texture = new Texture2D(Screen.width, Screen.height, TextureFormat.ARGB32, false);
            yield return new WaitForEndOfFrame();

            texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            texture.Apply();
            var bytes = texture.EncodeToPNG();
            File.WriteAllBytes(SCREENSHOT_PATH + i + EXTENSION, bytes);

            _setScene.Set(i);
            //if (i == 9) {
            //    Graphics.Blit(texture, null, _depthMat);    //ポストエフェクトでデプス画像に変換する
            //    yield return StartCoroutine(CaptureDepth());
            //}
            Destroy(texture);
        }
        EditorApplication.isPlaying = false;    //エディタ再生終了
    }

    //デプス画像を保存
    IEnumerator CaptureDepth()
    {
        var texture = new Texture2D(Screen.width, Screen.height, TextureFormat.ARGB32, false);
        yield return new WaitForEndOfFrame();

        texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        texture.Apply();
        var bytes = texture.EncodeToPNG();
        Destroy(texture);
        File.WriteAllBytes(DEPTH_IMG_PATH + "1" + EXTENSION, bytes);
    }
}