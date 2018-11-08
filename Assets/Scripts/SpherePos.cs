using UnityEngine;

public class SpherePos : MonoBehaviour
{

    public Transform targetTransform;
    public float radius, theta, phi, speed;

    public Vector3 camTargetPos;

    void Start()
    {
        camTargetPos = targetTransform.position;
        radius = 5f;    //半径長
        theta = 0f;     //方位角
        phi = 30f;      //仰角
        speed = 1f;
    }

    void Update()
    {
        //theta += Input.GetAxis("Horizontal") * speed;
        //phi += Input.GetAxis("Vertical") * speed;
        theta += speed;
        phi += speed;

        //回転角のクリッピング
        theta %= 360f;
        phi %= 360f;

        //カメラ位置を球面座標に配置し、回転角を逆算して入力している
        transform.position = new Vector3(
                                             radius * Mathf.Cos(phi * Mathf.Deg2Rad) * Mathf.Sin(theta * Mathf.Deg2Rad),
                                             radius * Mathf.Sin(phi * Mathf.Deg2Rad),
                                             radius * Mathf.Cos(phi * Mathf.Deg2Rad) * Mathf.Cos(theta * Mathf.Deg2Rad))
                                         + camTargetPos;

        transform.eulerAngles = new Vector3(phi, theta - 180f, 0);
        //transform.LookAt(camTargetPos);
    }
}
