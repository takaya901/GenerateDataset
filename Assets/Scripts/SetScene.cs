using System.Collections.Generic;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.Mathf;

/// <summary>シーンの各パラメータを設定する</summary>
public class SetScene : MonoBehaviour
{
    [SerializeField] Transform _targetTransform;
    [SerializeField] GameObject[] _probes;  //probeにするオブジェクトのリスト
    [SerializeField] GameObject _target;
    [SerializeField] GameObject _marker;

    GameObject _probe, _instantiatedTarget, _instantiatedMarker;
    Vector3 _camTargetPos;  //カメラの向き
    static readonly float CAM_RADIUS = 5f;    //中心からカメラまでの距離
    static readonly float LIGHT_RADIUS = 25f;    //中心から光源までの距離
    static readonly float MARKER_Y = 0.01f;
    CalcGeodesicDome _cgd;
    List<Vector3> _geoDomeVertices;
    Vector3 _markerPos;

    void Awake()
    {
        _cgd = new CalcGeodesicDome();
        _geoDomeVertices = _cgd.GetVertices();
        _camTargetPos = _targetTransform.position;
    }

    public void Set(int idx, bool isTrain)
    {
        //前のProbeとTargetを消去
        if (_probe != null) {
            Destroy(_probe);
        }
        if (_instantiatedTarget != null) {
            Destroy(_instantiatedTarget);
        }

        SetCameraPos(isTrain);
//        SetLight();
        var probeIdx = SetProbe();
        SetMarker();

//        Debug.Log($"{idx} CameraPos: {transform.position}");
    }
    
    /// <summary>ジオデシックドームの頂点上にカメラを配置</summary>
    void SetCameraPos(bool isTrain)
    {
        var idx = Random.Range(0, _geoDomeVertices.Count);
        transform.position = _geoDomeVertices[idx] * 4f;

        if (!isTrain) {
            var x = Random.Range(1f, 2f);
            var z = Random.Range(1f, 2f);
            transform.Translate(x, 0f, z);
        }
        
        transform.LookAt(new Vector3(0f, 0f, 0f));
    }

    //光源の位置と強さを設定
    void SetLight()
    {
        var idx = Random.Range(0, _geoDomeVertices.Count);
//        _light.transform.position = _geoDomeVertices[idx] * 20f;
//        _light.transform.LookAt(Vector3.down);
    }

    void SetMarker()
    {
        _markerPos = GetRandPos(0.01f);
        var probePos = _probe.transform.position;
        
        //Probeと異なる位置に生成
        while (_markerPos.x == probePos.x && _markerPos.z == probePos.z) {
            _markerPos = GetRandPos(0.01f);
        }
        _instantiatedMarker = Instantiate(_marker, _markerPos, Quaternion.identity);
    }
    
    float SetProbe()
    {
        var idx = Random.Range(0, _probes.Length);  //probe識別子
        var y = _probes[idx].transform.position.y;
        var pos = GetRandPos(y);
        _probe = SetPrefab(_probes[idx], pos);

        return idx;
    }
    
    public void SetTarget()
    {
        var y = _target.transform.position.y;
        var pos = new Vector3(_markerPos.x, y, _markerPos.z);
        _instantiatedTarget = SetPrefab(_target, pos);
        _instantiatedTarget.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.Off;
        
        if (_instantiatedMarker != null) {
            Destroy(_instantiatedMarker);
        }
    }

    GameObject SetPrefab(GameObject prefab, Vector3 pos)
    {
        var instantiatedObj = Instantiate(prefab, pos, Quaternion.identity);

        //Probeの下端が地面に接するように移動
//        var posY = instantiatedObj.GetComponent<Collider>().bounds.size.y / 2f;
//        instantiatedObj.transform.Translate(0f, posY, 0f);

        return instantiatedObj;
    }

    //Prefabの生成位置を9点からランダムに選ぶ
    Vector3 GetRandPos(float y)
    {
        return new Vector3 {
            x = Random.Range(-1, 2) * 2,
            y = y,
            z = Random.Range(-1, 2) * 2
        };
    }
}
