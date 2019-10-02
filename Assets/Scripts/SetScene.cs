using System.Collections.Generic;
using System.IO;
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
    [SerializeField] GameObject _plane;

    [SerializeField] Camera _camera;
    [SerializeField] Light _light;

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
        SetLight();
        var probeIdx = SetProbe();
        SetTarget();
        SetPlane(isTrain);

        if (idx % 1000 == 0) {
            Debug.Log($"{idx}");   
        }
    }
    
    /// <summary>ジオデシックドームの頂点上にカメラを配置</summary>
    void SetCameraPos(bool isTrain)
    {
        var idx = Random.Range(0, _geoDomeVertices.Count);
        var pos = _camera.transform;
//        pos.position = _geoDomeVertices[idx] * 4f;
//        var list = new List<float>()
//        {
//            3f, 3.25f, 3.5f, 3.75f, 4f, 4.25f, 4.5f, 4.75f, 5f
//        };
//        var dist = list[Random.Range(0, list.Count)];
//        pos.position = _geoDomeVertices[idx] * dist;
//        Debug.Log(dist);
        
//        if (!_camPosList.Contains(transform.position)) {
//            _camPosList.Add(pos.position);
//        }

//        if (!isTrain) {
            pos.position = _geoDomeVertices[idx] * Random.Range(3.5f, 5f);
            var x = Random.Range(-1f, 1f);
            var z = Random.Range(-1f, 1f);
            _camera.transform.Translate(x, 0f, z);
//            _camPosList.Add(transform1.position);
//        }
        
        _camera.transform.LookAt(new Vector3(0f, 0f, 0f));
    }

    //光源の位置と強さを設定
    void SetLight()
    {
        _light.intensity = Random.Range(0f, 1f);
    }
    
    float SetProbe()
    {
        var idx = Random.Range(0, _probes.Length);  //probe識別子
        var y = _probes[idx].transform.position.y;
        var pos = GetRandPos(y);
        _probe = SetPrefab(_probes[idx], pos, false);
//        _probe.transform.Rotate(new Vector3(1f, 0f, 0f), 90f);

        return idx;
    }
    
    void SetTarget()
    {
        var y = _target.transform.position.y;
        var pos = GetRandPos(0f);
        var probePos = _probe.transform.position;

        //Probeと異なる位置に生成
        while (pos.x == probePos.x && pos.z == probePos.z) {
            pos = GetRandPos(0f);
        }
        
        _instantiatedTarget = SetPrefab(_target, pos, true);
//        _instantiatedTarget.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.Off;
//        _instantiatedTarget.transform.Rotate(new Vector3(1f, 0f, 0f), 90f);
    }

    GameObject SetPrefab(GameObject prefab, Vector3 pos, bool isTarget)
    {
        var instantiatedObj = Instantiate(prefab, pos, Quaternion.identity);

        //Probeの下端が地面に接するように移動
        if (isTarget) {
            var posY = instantiatedObj.GetComponent<Collider>().bounds.size.y / 2f;
            instantiatedObj.transform.Translate(0f, posY, 0f);
        }
        
        instantiatedObj.transform.localRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

        return instantiatedObj;
    }

    //Prefabの生成位置を9点からランダムに選ぶ
    Vector3 GetRandPos(float y)
    {
        return new Vector3 {
            x = Random.Range(-1, 2) * 1.8f,
            y = y,
            z = Random.Range(-1, 2) * 1.8f
        };
    }

    void SetPlane(bool isTrain)
    {
        var interval = 10f;
//        var h = interval * Random.Range(0, 360 / (int)interval + 1);
//        h /= 360f;
//        var s = Random.Range(0f, 1f);
//        var v = Random.Range(0f, 1f);

//        var hsv = new Color(h, 0.5f, 0.5f);
        
//        if (!isTrain) {
            var hsv = new Color();
            hsv.r = Random.Range(0f, 1f);
            hsv.g = Random.Range(0f, 1f);
            hsv.b = Random.Range(0f, 1f);
//        }
        
        var rgb = Color.HSVToRGB(hsv.r, hsv.g, hsv.b);
        _plane.GetComponent<Renderer>().material.color = rgb;
    }

    public void CastTargetShadow()
    {
//        _instantiatedTarget.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.On;
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
