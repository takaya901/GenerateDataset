﻿using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;

public class DispDepth : MonoBehaviour
{
    [SerializeField] Material _depthMat;      //デプス表示用マテリアル
    [SerializeField] SetScene _setScene;
    
    const string DATASET_PATH = "/Users/takaya/Documents/Research/Dataset/";
    const string WITHOUT_TARGET = "without/";
    const string WITH_TARGET = "with/";
    
    static readonly int TRAIN_IMG_NUM = 1000; //訓練画像生成枚数
    static readonly int TEST_IMG_NUM = 100;  //テスト画像生成枚数
    static readonly int FIRST_INDEX = 0;     //既存のデータセットに追加する場合はこれを変更
    
    static readonly string DEPTH_IMG_PATH = "/Users/takaya/Documents/Unity/GenerateDataset/Dataset/DepthImg/";           //デプス画像の保存先
    const string EXTENSION = ".jpg";
    const int SS_SIZE = 400; //生成画像の縦横サイズ

    void Start()
    {
        GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;  //カメラがデプステクスチャを生成するモード
        StartCoroutine(CaptureScreenshot(isTrain:false));
//        StartCoroutine(CaptureDepth());
    }

    //スクリーンショットを保存
    //ReadPixelsはWaitForEndOfFrameのあとで実行しなければいけないのでコルーチンで実行 https://qiita.com/su10/items/a8f3f825155835de3d2a
    IEnumerator CaptureScreenshot(bool isTrain)
    {
        var imgNum = isTrain ? TRAIN_IMG_NUM : TEST_IMG_NUM;
        var dirPrefix = isTrain ? "train/" : "test/";
        
        var ssBeginPos = (Screen.width - SS_SIZE) / 2;    //スクリーンショットの左端
        
        for (int i = FIRST_INDEX; i < imgNum; i++) {
            _setScene.Set(i, isTrain);
            var texture = new Texture2D(SS_SIZE, SS_SIZE, TextureFormat.RGB24, false);
            yield return new WaitForEndOfFrame();

            texture.ReadPixels(new Rect(ssBeginPos, 0, SS_SIZE, SS_SIZE), 0, 0);
            texture.Apply();
            var bytes = texture.EncodeToJPG();
            File.WriteAllBytes(DATASET_PATH + dirPrefix + WITHOUT_TARGET + i + EXTENSION, bytes);

            //if (i == 9) {
            //    Graphics.Blit(texture, null, _depthMat);    //ポストエフェクトでデプス画像に変換する
            //    yield return StartCoroutine(CaptureDepth());
            //}
            //Destroy(texture);
            _setScene.CastTargetShadow();
            yield return new WaitForEndOfFrame();

            texture = new Texture2D(SS_SIZE, SS_SIZE, TextureFormat.RGB24, false);
            texture.ReadPixels(new Rect(ssBeginPos, 0, SS_SIZE, SS_SIZE), 0, 0);
            texture.Apply();
            bytes = texture.EncodeToJPG();
            File.WriteAllBytes(DATASET_PATH + dirPrefix + WITH_TARGET + i + EXTENSION, bytes);

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
}