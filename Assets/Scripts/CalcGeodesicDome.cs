using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CalcGeodesicDome 
{
	List<Vector3> _vertices;                                   //ジオデシックドームの頂点群
	static readonly float G = (1f + (float)Math.Sqrt(5)) / 2f; //黄金比
	float _distAdjVtx = 2f;                                    //ジオデシックドームの隣接頂点間の距離　初期値2は正二十面体のもの
	float _radius;                                             //正二十面体の外接球の半径

	//正二十面体の頂点群
	static readonly List<Vector3> ICOSAHEDRON_VERTICES = new List<Vector3>
	{
		new Vector3(1, G, 0), new Vector3(1, -G, 0), new Vector3(-1, G, 0), new Vector3(-1, -G, 0),
		new Vector3(0, 1, G), new Vector3(0, 1, -G), new Vector3(0, -1, G), new Vector3(0, -1, -G),
		new Vector3(G, 0, 1), new Vector3(-G, 0, 1), new Vector3(G, 0, -1), new Vector3(-G, 0, -1)
	};
	
	public CalcGeodesicDome()
	{
		_vertices = new List<Vector3>(ICOSAHEDRON_VERTICES); //正二十面体の頂点群⊆ジオデシックドームの頂点群
		_radius = ICOSAHEDRON_VERTICES[0].magnitude;
		Split(4);
		_vertices = _vertices.Distinct().Where(vtx => vtx.y > 1.0f).ToList();
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
			
			_vertices = new List<Vector3>(newVertices); //頂点群を更新
			_vertices = _vertices.Distinct().ToList();
			_distAdjVtx = CalcAdjDist();                //隣接頂点間の距離を更新
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

	public List<Vector3> GetVertices()
	{
		Debug.Log(_vertices.Count);
		Debug.Log(_vertices.Distinct().Where(vtx => vtx.y > 1.0f).ToList().Count);
		return _vertices;
	}
}
