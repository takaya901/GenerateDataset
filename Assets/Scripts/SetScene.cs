using UnityEngine;

public class SetScene : MonoBehaviour
{
    [SerializeField] GameObject _plane;

    void Start ()
    {
        //床の明るさ(V)をランダムに変更　http://nn-hokuson.hatenablog.com/entry/2017/04/12/194631
        var value = Random.Range(30f, 100f);
        _plane.GetComponent<Renderer>().material.color = Color.HSVToRGB(0f, 0f, value/255f);
    }
	
	void Update () 
	{
		
	}
}
