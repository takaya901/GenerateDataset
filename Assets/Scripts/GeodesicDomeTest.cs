using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

//http://atali.jp/blog/2014/08/geodesicdome/
public class GeodesicDomeTest : MonoBehaviour
{
    [SerializeField] GameObject _sphere;    //頂点表示用
    
    List<Vector3> _vertices; //ジオデシックドームの頂点群
    static readonly float G = (1f + (float)Math.Sqrt(5)) / 2f;  //黄金比
    float _distAdjVtx = 2f;    //ジオデシックドームの隣接頂点間の距離　初期値2は正二十面体のもの
    float _radius;  //正二十面体の外接球の半径

    //正二十面体の頂点群
    static readonly List<Vector3> ICOSAHEDRON_VERTICES = new List<Vector3>
    {
        new Vector3(1, G, 0), new Vector3(1, -G, 0), new Vector3(-1, G, 0), new Vector3(-1, -G, 0),
        new Vector3(0, 1, G), new Vector3(0, 1, -G), new Vector3(0, -1, G), new Vector3(0, -1, -G),
        new Vector3(G, 0, 1), new Vector3(-G, 0, 1), new Vector3(G, 0, -1), new Vector3(-G, 0, -1)
    };

    void Start()
    {
        _vertices = new List<Vector3>(ICOSAHEDRON_VERTICES);  //正二十面体の頂点群⊆ジオデシックドームの頂点群
        _radius = ICOSAHEDRON_VERTICES[0].magnitude;
        
        Split(4);
        _vertices = _vertices.Distinct().Where(vtx => vtx.y > 1.0f).ToList();
        Debug.Log(_vertices.Count);
        foreach (var vtx in _vertices) {    //頂点を表示
            Instantiate(_sphere, vtx, Quaternion.identity).transform.localScale = new Vector3(0.08f, 0.08f, 0.08f);
        }
    }

    //正二十面体をtimes回分割した頂点群を返す
    void Split(int times)
    {
        while (times-- > 0) 
        {
            var newVertices = new List<Vector3>(_vertices);
            
            foreach (var vtx in _vertices) {
                //隣接する頂点を取得
                var adjVertices = _vertices
                    .Where(v => v != vtx)
                    .Where(otherVtx => Math.Abs((otherVtx - vtx).magnitude - _distAdjVtx) < 0.14f)
                    .ToList();
            
                //隣接頂点との中点を求め，正二十面体の外接球の表面に移動させる
                foreach (var avtx in adjVertices) {
                    var newVtx = Vector3.Lerp(vtx, avtx, 0.5f).normalized * _radius;
                    if (!_vertices.Contains(newVtx)) {
                        newVertices.Add(newVtx);
                    }
                }
            }
            
            _vertices = new List<Vector3>(newVertices);    //頂点群を更新
            _vertices = _vertices.Distinct().ToList();
            _distAdjVtx = CalcAdjDist();    //隣接頂点間の距離を更新
        }
    }

    //隣接頂点間の距離を求める
    float CalcAdjDist()
    {
        var vertices = new List<Vector3>(_vertices);
        vertices.RemoveAt(0);
        var minDist = float.MaxValue;
        
        foreach (var vtx in vertices) {
            if ((vtx - _vertices[0]).sqrMagnitude < minDist) {
                minDist = (vtx - _vertices[0]).sqrMagnitude;
            }
        }

        return Mathf.Sqrt(minDist);
    }

    //辺を描画
//    void OnDrawGizmos()
//    {
//        if (!EditorApplication.isPlaying) return;    //頂点計算しないと描画できないため
//        Gizmos.color = Color.red;
//        
//        foreach (var vtx in _vertices) 
//        {
//            //隣接する頂点を取得
//            var adjacentVertices = _vertices
//                .Where(v => v != vtx)
//                .Where(otherVtx => Math.Abs((otherVtx - vtx).magnitude - _distAdjVtx) < 0.05f)
//                .ToList();
//            
//            foreach (var avtx in adjacentVertices) {    //隣接頂点間の辺を描画
//                Gizmos.DrawLine(vtx, avtx);
//            }
//        }
//    }
}
