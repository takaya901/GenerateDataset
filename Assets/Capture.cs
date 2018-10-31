using System.Collections;
using System.IO;
using UnityEngine;

public class Capture : MonoBehaviour 
{
    string SCREENSHOT_PATH, DEPTH_IMG_PATH;
    [SerializeField] Camera _mainCamera;

    void Start ()
    {
        SCREENSHOT_PATH = Application.dataPath + "/Dataset/ScreenShots/ss.png";
        DEPTH_IMG_PATH = Application.dataPath + "/Dataset/DepthImg/depth.png";

        //StartCoroutine("CaptureCoroutine");

        //_mainCamera.GetComponent("DispDepth").gameObject.SetActive(true);
    }

    private IEnumerator CaptureCoroutine()  //https://qiita.com/su10/items/a8f3f825155835de3d2a
    {
        var texture = new Texture2D(Screen.width, Screen.height, TextureFormat.ARGB32, false);

        yield return new WaitForEndOfFrame();

        texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        texture.Apply();

        var bytes = texture.EncodeToPNG();
        Destroy(texture);
        File.WriteAllBytes(SCREENSHOT_PATH, bytes);
    }
}
