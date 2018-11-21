using UnityEngine;
using static UnityEngine.Mathf;

/// <summary>シーンの各パラメータを設定する</summary>
public class SetScene : MonoBehaviour
{
    [SerializeField] Transform _targetTransform;
    [SerializeField] GameObject[] _probes;  //probeにするオブジェクトのリスト
    [SerializeField] GameObject _target;
    [SerializeField] Light _light;
    [SerializeField] GameObject _plane; //床

    GameObject _probe, _instantiatedTarget;
    Vector3 _camTargetPos;  //カメラの向き
    static readonly float CAM_RADIUS = 5f;    //中心からカメラまでの距離
    static readonly float LIGHT_RADIUS = 25f;    //中心から光源までの距離

    void Start()
    {
        _camTargetPos = _targetTransform.position;
    }

    public void Set(int idx)
    {
        if (_probe != null) {   //前のprobeを消去
            Destroy(_probe);
        }
        if (_instantiatedTarget != null) {
            Destroy(_instantiatedTarget);
        }

        SetCameraPos();
        SetLight();
        var planeValue = SetPlane();
        var probeIdx = SetProbe();

        Debug.Log($"{idx} Light Intensity: {_light.intensity} Plane Value: {planeValue} Probe Index: {probeIdx}");
    }
    
    //カメラ位置を設定
    void SetCameraPos()
    {
        var theta = Random.Range(0f, 360f); //方位角
        var phi = Random.Range(0f, 90f);    //仰角
        SetOnSphere(transform, CAM_RADIUS, theta, phi);

        transform.eulerAngles = new Vector3(phi, theta - 180f, 0);  //中心を向く
    }

    //光源の位置と強さを設定
    void SetLight()
    {
        var theta = Random.Range(0f, 360f); //方位角
        var phi = Random.Range(60f, 90f);    //仰角
        SetOnSphere(_light.transform, LIGHT_RADIUS, theta, phi);

        _light.intensity = Random.Range(3f, 7f);
    }

    //オブジェクトを球面上に配置 http://osinko.hatenablog.jp/entry/2017/04/05/184123
    void SetOnSphere(Transform transform, float radius, float theta, float phi)
    {
        transform.position = new Vector3(
                                 radius * Cos(phi * Deg2Rad) * Sin(theta * Deg2Rad),
                                 radius * Sin(phi * Deg2Rad),
                                 radius * Cos(phi * Deg2Rad) * Cos(theta * Deg2Rad))
                             + _camTargetPos;
    }

    //床の明るさ(HSVのVのみ)をランダムに設定　http://nn-hokuson.hatenablog.com/entry/2017/04/12/194631
    float SetPlane()
    {
        var value = Random.Range(30f, 100f);
        _plane.GetComponent<Renderer>().material.color = Color.HSVToRGB(0f, 0f, value / 100f);
        return value;
    }

    //ランダムにprobeを生成
    float SetProbe()
    {
        var idx = Random.Range(0, _probes.Length);  //probe識別子
        _probe = SetPrefab(_probes[idx]);

        return idx;
    }

    public void SetTarget()
    {
        _instantiatedTarget = SetPrefab(_target);
    }

    GameObject SetPrefab(GameObject prefab)
    {
        var pos = new Vector3 {
            x = Random.Range(-3f, 3f),
            y = 0f,
            z = Random.Range(-3f, 3f)
        };
        var rot = Quaternion.AngleAxis(Random.Range(0f, 360f), Vector3.up);
        var scale = Random.Range(0.5f, 2f);

        var instantiatedObj = Instantiate(prefab, pos, rot);
        instantiatedObj.transform.localScale = new Vector3(scale, scale, scale);

        //Probeの下端が地面に接するように移動
        var posY = instantiatedObj.GetComponent<Collider>().bounds.size.y / 2f;
        instantiatedObj.transform.Translate(0f, posY, 0f);

        return instantiatedObj;
    }
}
