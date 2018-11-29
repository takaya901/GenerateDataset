using System.Collections.Generic;
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
    CalcGeodesicDome _cgd;
    List<Vector3> _geoDomeVertices;

    void Awake()
    {
        _cgd = new CalcGeodesicDome();
        _geoDomeVertices = _cgd.GetVertices();
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
//        SetLight();
        var probeIdx = SetProbe();

        Debug.Log($"{idx} CameraPos: {transform.position} LightPos: {_light.transform.position}");
    }
    
    /// <summary>ジオデシックドームの頂点上にカメラを配置</summary>
    void SetCameraPos()
    {
        var idx = Random.Range(0, _geoDomeVertices.Count);
        transform.position = _geoDomeVertices[idx] * 4.5f;
        transform.LookAt(new Vector3(0f, 0f, 0f));
    }

    //光源の位置と強さを設定
    void SetLight()
    {
        var idx = Random.Range(0, _geoDomeVertices.Count);
        _light.transform.position = _geoDomeVertices[idx] * 20f;
        _light.transform.LookAt(Vector3.down);
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
            x = Random.Range(-1, 2) * 2,
            y = 0f,
            z = Random.Range(-1, 2) * 2
        };
    }
}
