using UnityEngine;

public class FOVDetector : MonoBehaviour
{
    [SerializeField] private float detection_angle = 45f; // The angle of the field of view
    private float fov_half_angle; // Half of the field of view angle
    [SerializeField] GameObject target;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //視野角の半分の値を求める
        fov_half_angle = detection_angle / 2f;


        if (!target)
            return;

        Color line_color = Color.green;



        //前方方向を取得
        Vector3 forward = transform.forward;
        //ターゲット方向を取得
        Vector3 target_direction = (target.transform.position - transform.position);

        //ターゲット方向をtransform.upの方向に投影して、ターゲット方向を水平面上に制限する
        target_direction = Vector3.ProjectOnPlane(target_direction, transform.up);

        //内積を計算して、視野角の半分の値と比較することで、ターゲットが視野内にいるかどうかを判定
        if (Vector3.Dot(forward, target_direction.normalized) > Mathf.Cos(Mathf.Deg2Rad * fov_half_angle))
        {
            //ターゲットが視野内にいる場合、デバッグラインの色を変える

            //外積を計算して、ターゲットが右側にいるか左側にいるかを判定
            Vector3 cross_ = Vector3.Cross(forward, target_direction.normalized);


            //右側にいる場合
            if (Vector3.Dot(cross_, transform.up) > 0)
            {
                line_color = Color.blue;
            }
            //左側にいる場合
            else
            {
                line_color = Color.red;
            }
            float max_distance = 10f;
            float max_distance_sqr = max_distance * max_distance;
            if (target_direction.sqrMagnitude > max_distance_sqr)
            {
                line_color = Color.yellow;
            }
        }


        Debug.Log(Vector3.Dot(forward, target_direction.normalized));

        Vector3 fov_left_border = transform.rotation * Quaternion.Euler(0, -fov_half_angle, 0) * Vector3.forward;
        Vector3 fov_right_border = transform.rotation * Quaternion.Euler(0, fov_half_angle, 0) * Vector3.forward;

        Debug.DrawLine(transform.position, transform.position + target_direction, line_color);
        Debug.DrawLine(transform.position, transform.position + fov_left_border.normalized * 10, line_color);
        Debug.DrawLine(transform.position, transform.position + fov_right_border.normalized * 10, line_color);
    }
}