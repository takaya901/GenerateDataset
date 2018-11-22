using UnityEngine;
using static UnityEngine.Mathf;

/// <summary>シーンの各パラメータを設定する</summary>
public class SetScene : MonoBehaviour
{
    [SerializeField] Transform _targetTransform;
    [SerializeField] GameObject[] _probes;  //probeにするオブジェクトのリスト
    [SerializeField] GameObject _target;
    [SerializeField] Light _light;

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

        //SetCameraPos();
        //SetLight();
        var probeIdx = SetProbe();

        Debug.Log($"{idx} ProbePos: {_probe.transform.position}");
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
    
    float SetProbe()
    {
        var idx = Random.Range(0, _probes.Length);  //probe識別子
        var pos = GetRandPos();
        _probe = SetPrefab(_probes[idx], pos);

        return idx;
    }
    
    public void SetTarget()
    {
        var pos = GetRandPos();
        var probePos = _probe.transform.position;

        //Probeと異なる位置に生成
        while (pos.x == probePos.x && pos.z == probePos.z) {
            pos = GetRandPos();
        }
        _instantiatedTarget = SetPrefab(_target, pos);
    }

    GameObject SetPrefab(GameObject prefab, Vector3 pos)
    {
        var instantiatedObj = Instantiate(prefab, pos, Quaternion.identity);

        //Probeの下端が地面に接するように移動
        var posY = instantiatedObj.GetComponent<Collider>().bounds.size.y / 2f;
        instantiatedObj.transform.Translate(0f, posY, 0f);

        return instantiatedObj;
    }

    //Prefabの生成位置を9点からランダムに選ぶ
    Vector3 GetRandPos()
    {
        return new Vector3 {
            x = Random.Range(-1, 2) * 3,
            y = 0f,
            z = Random.Range(-1, 2) * 3
        };
    }
}
