using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.Mathf;

/// <summary>シーンの各パラメータを設定する</summary>
public class SetScene : MonoBehaviour
{
    [SerializeField] GameObject[] _probes;  //probeにするオブジェクトのリスト
    [SerializeField] GameObject _target;
    [SerializeField] GameObject _sphere;    //for debug

    GameObject _probe, _instantiatedTarget;
    CalcGeodesicDome _cgd;
    List<Vector3> _geoDomeVertices;
    Shader _standardShader;
    List<Vector3> _camPosList = new List<Vector3>();
    Dictionary<Vector3, int> _cnt = new Dictionary<Vector3, int>();

    void Awake()
    {
        _cgd = new CalcGeodesicDome();
        _geoDomeVertices = _cgd.GetVertices();
        _standardShader = Shader.Find("Standard");
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
        var probeIdx = SetProbe();
        SetTarget();

        if (idx % 1000 == 0) {
            Debug.Log($"{idx}");   
        }
    }
    
    /// <summary>ジオデシックドームの頂点上にカメラを配置</summary>
    void SetCameraPos(bool isTrain)
    {
        var idx = Random.Range(0, _geoDomeVertices.Count);
        var pos = transform;
        pos.position = _geoDomeVertices[idx] * 4f;
        
//        if (!_camPosList.Contains(transform.position)) {
//            _camPosList.Add(pos.position);
//        }

        if (!isTrain) {
            var x = Random.Range(-1f, 1f);
            var z = Random.Range(-1f, 1f);
            transform.Translate(x, 0f, z);
//            _camPosList.Add(transform1.position);
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
    
    float SetProbe()
    {
        var idx = Random.Range(0, _probes.Length);  //probe識別子
        var y = _probes[idx].transform.position.y;
        var pos = GetRandPos(y);
        _probe = SetPrefab(_probes[idx], pos);

        return idx;
    }
    
    void SetTarget()
    {
        var y = _target.transform.position.y;
        var pos = GetRandPos(y);
        var probePos = _probe.transform.position;

        //Probeと異なる位置に生成
        while (pos.x == probePos.x && pos.z == probePos.z) {
            pos = GetRandPos(y);
        }
        
        _instantiatedTarget = SetPrefab(_target, pos);
        _instantiatedTarget.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.Off;
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
    Vector3 GetRandPos(float y)
    {
        return new Vector3 {
            x = Random.Range(-1, 2) * 1.8f,
            y = 0f,
            z = Random.Range(-1, 2) * 1.8f
        };
    }

    public void CastTargetShadow()
    {
        _instantiatedTarget.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.On;
        _instantiatedTarget.GetComponent<Renderer>().material.shader = _standardShader;
    }

    public void SetSpheres()
    {
        Debug.Log(_camPosList.Count);
        foreach (var pos in _camPosList) {
            Instantiate(_sphere, pos, Quaternion.identity).transform.localScale = new Vector3(0.1f,0.1f, 0.1f);
        }
//        foreach (var pos in _camPosList) {
//            if (_cnt.ContainsKey(pos)) {
//                continue;
//            }
//            var count = _camPosList.Where(p => Approximately(pos.x, p.x) && Approximately(pos.y, p.y) && Approximately(pos.z, p.z)).ToList().Count;
//            _cnt.Add(pos, count);
//        }
//
//        foreach (var map in _cnt) {
//            Debug.Log($"{map.Key} : {map.Value}\n");
//        }
    }
}
