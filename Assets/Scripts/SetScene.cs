using UnityEngine;
using static UnityEngine.Mathf;

/// <summary>シーンの各パラメータを変更する</summary>
public class SetScene : MonoBehaviour
{
    [SerializeField] Transform _targetTransform;
    [SerializeField] Light _light;
    [SerializeField] GameObject _plane;

    Vector3 _camTargetPos;  //カメラの向き
    const float CAM_RADIUS = 5f;    //中心からカメラまでの距離
    const float LIGHT_RADIUS = 20f;    //中心から光源までの距離

    void Start()
    {
        _camTargetPos = _targetTransform.position;
    }

    public void Set(int idx)
    {
        SetCameraPos();
        SetLight();
        
        //床の明るさ(HSVのVのみ)をランダムに変更　http://nn-hokuson.hatenablog.com/entry/2017/04/12/194631
        var value = Random.Range(30f, 100f);
        _plane.GetComponent<Renderer>().material.color = Color.HSVToRGB(0f, 0f, value/255f);

        Debug.Log($"{idx}: Light Intensity: {_light.intensity}\nPlane Value: {value}");
    }
    
    //カメラ位置を変更
    void SetCameraPos()
    {
        var theta = Random.Range(0f, 360f); //方位角
        var phi = Random.Range(0f, 90f);    //仰角
        SetOnSphere(transform, CAM_RADIUS, theta, phi);

        transform.eulerAngles = new Vector3(phi, theta - 180f, 0);  //中心を向く
    }

    //光源の位置と強さを変更
    void SetLight()
    {
        var theta = Random.Range(0f, 360f); //方位角
        var phi = Random.Range(60f, 90f);    //仰角
        SetOnSphere(_light.transform, LIGHT_RADIUS, theta, phi);

        _light.intensity = Random.Range(3f, 8f);
    }

    //オブジェクトを球面上に配置
    void SetOnSphere(Transform transform, float radius, float theta, float phi)
    {
        transform.position = new Vector3(
                                 radius * Cos(phi * Deg2Rad) * Sin(theta * Deg2Rad),
                                 radius * Sin(phi * Deg2Rad),
                                 radius * Cos(phi * Deg2Rad) * Cos(theta * Deg2Rad))
                             + _camTargetPos;
    }
}
