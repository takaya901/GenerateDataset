using UnityEngine;

/// <summary>シーンの各パラメータを変更する</summary>
public class SetScene : MonoBehaviour
{
    [SerializeField] GameObject _plane;
    [SerializeField] Light _light;
    
    public void Set()
    {
        _light.intensity = Random.Range(3f, 8f);
        
        //床の明るさ(HSVのVのみ)をランダムに変更　http://nn-hokuson.hatenablog.com/entry/2017/04/12/194631
        var value = Random.Range(30f, 100f);
        _plane.GetComponent<Renderer>().material.color = Color.HSVToRGB(0f, 0f, value/255f);

        Debug.Log($"Light Intensity: {_light.intensity}\nPlane Value: {value}");
    }
}
