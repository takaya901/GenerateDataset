using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//http://atali.jp/blog/2014/08/geodesicdome/
public class GeodesicDomeVertices
{
    static readonly float G = (1f + (float)Math.Sqrt(5)) / 2f;  //黄金比
    const float DIST_BETWEEN_VTX = 2f;    //正二十面体の隣接頂点間の距離

    readonly List<Vector3> IcosahedronVertices = new List<Vector3>
    {
        new Vector3(1, G, 0), new Vector3(1, -G, 0), new Vector3(-1, G, 0), new Vector3(-1, -G, 0),
        new Vector3(0, 1, G), new Vector3(0, 1, -G), new Vector3(0, -1, G),new Vector3(0, -1, -G),
        new Vector3(G, 0, 1), new Vector3(-G, 0, 1), new Vector3(G, 0, -1), new Vector3(-G, 0, -1)
    };  //正二十面体の頂点群
    public List<Vector3> Vertices;    //ジオデシックドームの頂点群
    public float Diameter;  //正二十面体の外接円の直径
    public float Radius;  //正二十面体の外接円の半径

    public GeodesicDomeVertices()
    {
        Vertices = new List<Vector3>(IcosahedronVertices);  //正二十面体の頂点群⊆ジオデシックドームの頂点群
        Diameter = IcosahedronVertices[0].magnitude * 2f;
        Radius = IcosahedronVertices[0].magnitude;
        MakeGeoDome();
    }

    void MakeGeoDome()
    {
        foreach (var vtx in IcosahedronVertices) {
            //隣接する頂点を取得
            var adjacentVertices = IcosahedronVertices
                .Where(v => v != vtx)
                .Where(otherVtx => Math.Abs((otherVtx - vtx ).magnitude - DIST_BETWEEN_VTX) < float.Epsilon)
                .ToList();

            //隣接頂点との中点を求め，正二十面体の外接球の表面に移動させる
            foreach (var avtx in adjacentVertices) {
                var newVtx = Vector3.Lerp(vtx, avtx, 0.5f).normalized * Radius;
                if (!IcosahedronVertices.Contains(newVtx)) {
                    Vertices.Add(newVtx);
                }
            }
        }
    }
}
