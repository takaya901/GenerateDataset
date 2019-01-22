﻿using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;

public class DispDepth : MonoBehaviour
{
    [SerializeField] SetScene _setScene;
    
    const string DATASET_PATH = "/Users/takaya/Documents/Research/Dataset/";
    const string WITHOUT_TARGET = "without/";
    const string WITH_TARGET = "with/";

    static readonly int TRAIN_IMG_NUM = 60000; //訓練画像生成枚数
    static readonly int TEST_IMG_NUM = 6000;   //テスト画像生成枚数
    static readonly bool IS_TRAIN = false;     //訓練かテストか
    static readonly int FIRST_INDEX = 0;      //既存のデータセットに追加する場合はこれを変更
    
    const string EXTENSION = ".jpg";
    const int SS_SIZE = 400; //生成画像の縦横サイズ

    void Start()
    {
        GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;  //カメラがデプステクスチャを生成するモード
        StartCoroutine(CaptureScreenshot(IS_TRAIN));
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

            _setScene.CastTargetShadow();
            yield return new WaitForEndOfFrame();

            texture = new Texture2D(SS_SIZE, SS_SIZE, TextureFormat.RGB24, false);
            texture.ReadPixels(new Rect(ssBeginPos, 0, SS_SIZE, SS_SIZE), 0, 0);
            texture.Apply();
            bytes = texture.EncodeToJPG();
            File.WriteAllBytes(DATASET_PATH + dirPrefix + WITH_TARGET + i + EXTENSION, bytes);

            Destroy(texture);
        }
//        _setScene.SetSpheres();
        EditorApplication.isPlaying = false;    //データ取り終えたらエディタ再生終了
    }
}