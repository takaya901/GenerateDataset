using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

//http://atali.jp/blog/2014/08/geodesicdome/
public class GeodesicDomeVertices : MonoBehaviour
{
    static readonly float G = (1f + (float)Math.Sqrt(5)) / 2f;  //黄金比
    const float DIST_ADJ_VTX = 2f;    //正二十面体の隣接頂点間の距離

    readonly List<Vector3> _icosahedronVertices = new List<Vector3>
    {
        new Vector3(1, G, 0), new Vector3(1, -G, 0), new Vector3(-1, G, 0), new Vector3(-1, -G, 0),
        new Vector3(0, 1, G), new Vector3(0, 1, -G), new Vector3(0, -1, G), new Vector3(0, -1, -G),
        new Vector3(G, 0, 1), new Vector3(-G, 0, 1), new Vector3(G, 0, -1), new Vector3(-G, 0, -1)
    };  //正二十面体の頂点群
    List<Vector3> _vertices;    //ジオデシックドームの頂点群
    float _radius;  //正二十面体の外接球の半径

    void Start()
    {
        _vertices = new List<Vector3>(_icosahedronVertices);  //正二十面体の頂点群⊆ジオデシックドームの頂点群
        _radius = _icosahedronVertices[0].magnitude;
        Split();
    }

    void Split()
    {
        foreach (var vtx in _icosahedronVertices) {
            //隣接する頂点を取得
            var adjacentVertices = _icosahedronVertices
                .Where(v => v != vtx)
                .Where(otherVtx => Math.Abs((otherVtx - vtx).magnitude - DIST_ADJ_VTX) < float.Epsilon)
                .ToList();
            
            //隣接頂点との中点を求め，正二十面体の外接球の表面に移動させる
            foreach (var avtx in adjacentVertices) {
                var newVtx = Vector3.Lerp(vtx, avtx, 0.5f).normalized * _radius;
                if (!_icosahedronVertices.Contains(newVtx)) {
                    _vertices.Add(newVtx);
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        if (!EditorApplication.isPlaying) return;
        
        Gizmos.color = Color.red;
        foreach (var vtx in _vertices) {
            //隣接する頂点を取得
            var adjacentVertices = _vertices
                .Where(v => v != vtx)
                .Where(otherVtx => (otherVtx - vtx).magnitude < 1.5f)
                .ToList();
            
            //隣接頂点との中点を求め，正二十面体の外接球の表面に移動させる
            foreach (var avtx in adjacentVertices) {
                Gizmos.DrawLine(vtx, avtx);
            }
        }
    }
}
