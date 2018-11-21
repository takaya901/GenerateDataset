using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;

public class DispDepth : MonoBehaviour
{
    [SerializeField] Material _depthMat;    //デプス表示用マテリアル
    [SerializeField] SetScene _setScene;
    [SerializeField] GameObject _sphere;

    static readonly string WITHOUT_TARGET_PATH = "C:/Unity/Dataset/rgbWithoutTarget/"; //スクリーンショットの保存先
    static readonly string WITH_TARGET_PATH = "C:/Unity/Dataset/rgbWithTarget/"; //スクリーンショットの保存先
    static readonly string DEPTH_IMG_PATH = "C:/Unity/Dataset/DepthImg/";  //デプス画像の保存先
    static readonly string EXTENSION = ".png";

    void Start()
    {
        GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;  //カメラがデプステクスチャを生成するモード
        
        //StartCoroutine(CaptureScreenshot());
        MakeGeoDome();
        //StartCoroutine(CaptureDepth());
    }

    //スクリーンショットを保存
    //ReadPixelsはWaitForEndOfFrameのあとで実行しなければいけないのでコルーチンで実行 https://qiita.com/su10/items/a8f3f825155835de3d2a
    IEnumerator CaptureScreenshot()
    {
        for (int i = 0; i < 10; i++) {
            _setScene.Set(i);
            var texture = new Texture2D(Screen.width, Screen.height, TextureFormat.ARGB32, false);
            yield return new WaitForEndOfFrame();

            texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            texture.Apply();
            var bytes = texture.EncodeToPNG();
            File.WriteAllBytes(WITHOUT_TARGET_PATH + i + EXTENSION, bytes);

            //if (i == 9) {
            //    Graphics.Blit(texture, null, _depthMat);    //ポストエフェクトでデプス画像に変換する
            //    yield return StartCoroutine(CaptureDepth());
            //}
            //Destroy(texture);
            _setScene.SetTarget();
            yield return new WaitForEndOfFrame();

            texture = new Texture2D(Screen.width, Screen.height, TextureFormat.ARGB32, false);
            texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            texture.Apply();
            bytes = texture.EncodeToPNG();
            File.WriteAllBytes(WITH_TARGET_PATH + i + EXTENSION, bytes);

            //if (i == 9) {
            //    Graphics.Blit(texture, null, _depthMat);    //ポストエフェクトでデプス画像に変換する
            //    yield return StartCoroutine(CaptureDepth());
            //}
            Destroy(texture);
        }
        EditorApplication.isPlaying = false;    //データ取り終えたらエディタ再生終了
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

    void MakeGeoDome()
    {
        var gdv = new GeodesicDomeVertices();

        foreach (var vtx in gdv.Vertices) {
            Instantiate(_sphere, vtx, Quaternion.identity).transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        }

        Instantiate(_sphere, new Vector3(0, 0, 0), Quaternion.identity).transform.localScale = new Vector3(gdv.Diameter, gdv.Diameter, gdv.Diameter);
        Debug.Log(gdv.Vertices.Count);
    }
}