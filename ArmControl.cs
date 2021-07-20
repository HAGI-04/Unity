using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmControl1 : MonoBehaviour
{
    public static float BaseHeight = 0.26f;
    public static float link1Length = 0.6f;
    public static float link2Length = 2f;
    public static float link3Length = 2.4f;

    GameObject DestinationPoint;
    GameObject joint1;
    GameObject joint2;
    GameObject joint3;

    // Start is called before the first frame update
    void Start()
    {
        Component[] childList = gameObject.GetComponentsInChildren(typeof(Transform));
        foreach (Component child in childList)
        {
            if (child.name == "DestinationPoint") DestinationPoint = child.gameObject;
            if (child.name == "Link1(Y-Rotate)") joint1 = child.gameObject;
            if (child.name == "Link2(Z-Rotate)") joint2 = child.gameObject;
            if (child.name == "Link3(Z-Rotate)") joint3 = child.gameObject;
        }
    }

    // Update is called once per frame
    void Update()
    {
        float theta_a, theta_b, theta_c;
        float k_a, b_b = link3Length, b_c = link2Length;
        float BaseRotationAngle;

        Vector3 IncreaseX = new Vector3(0.05f, 0, 0);
        Vector3 IncreaseY = new Vector3(0, 0.05f, 0);
        Vector3 IncreaseZ = new Vector3(0, 0, 0.05f);

        // 目標地点を示すボールの移動
        if (Input.GetKey(KeyCode.DownArrow))
            DestinationPoint.transform.Translate(IncreaseX);
        if (Input.GetKey(KeyCode.UpArrow))
            DestinationPoint.transform.Translate(-1 * IncreaseX);

        if (Input.GetKey(KeyCode.Space))
            DestinationPoint.transform.Translate(IncreaseY);
        if (Input.GetKey(KeyCode.C))
            DestinationPoint.transform.Translate(-1 * IncreaseY);

        if (Input.GetKey(KeyCode.RightArrow))
            DestinationPoint.transform.Translate(IncreaseZ);
        if (Input.GetKey(KeyCode.LeftArrow))
            DestinationPoint.transform.Translate(-1 * IncreaseZ);

        //目標値点の座標取得
        Vector3 DestPos = DestinationPoint.transform.localPosition;
        Vector3 DestPosFromJoint2 = DestPos - new Vector3(0, BaseHeight + link1Length, 0);
        k_a = DestPosFromJoint2.magnitude;
        theta_a = Mathf.Atan(DestPosFromJoint2.y / Mathf.Sqrt(Mathf.Pow(DestPosFromJoint2.x, 2.0f) + Mathf.Pow(DestPosFromJoint2.z, 2.0f)));
        BaseRotationAngle = Mathf.Atan2(DestPosFromJoint2.z, DestPosFromJoint2.x);

        // 逆運動学による各関節の角度の算出
        float Cos_theta_b_minus_theta_a = ( Mathf.Pow(b_c, 2.0f) - Mathf.Pow(b_b, 2.0f) - Mathf.Pow(k_a, 2.0f) ) / ( 2 * b_b * k_a );
        float Sin_theta_b_minus_theta_a = Mathf.Sqrt(1 - Mathf.Pow(Cos_theta_b_minus_theta_a, 2.0f));
        float Tan_theta_b_minus_theta_a = Sin_theta_b_minus_theta_a / Cos_theta_b_minus_theta_a;
        theta_b = Mathf.Atan2(Sin_theta_b_minus_theta_a, Cos_theta_b_minus_theta_a) + theta_a;
        float tmp1 = b_b * Mathf.Sin(theta_b) + k_a * Mathf.Sin(theta_a);
        float tmp2 = b_b * Mathf.Cos(theta_b) + k_a * Mathf.Cos(theta_a);
        theta_c = Mathf.Atan2(tmp1, tmp2);

        // ロボットアームの移動
        BaseRotationAngle *= Mathf.Rad2Deg;
        theta_b *= Mathf.Rad2Deg;
        theta_c *= Mathf.Rad2Deg;
        joint1.transform.localEulerAngles = new Vector3(0, -1 * BaseRotationAngle, 0);
        joint2.transform.localEulerAngles = new Vector3(0, 0, -90 + theta_c);
        joint3.transform.localEulerAngles = new Vector3(0, 0, 180 + theta_b - theta_c);
    }
}
