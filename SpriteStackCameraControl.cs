using System.Collections;
using UnityEngine;

public class SpriteStackCameraControl : MonoBehaviour
{
    public Transform cameraTransform;
    public float rotate_speed = 50.0f;
    public float distance = 10.0f;
    public float minAngle = 10f;
    public float maxAngle = 80f;

    private Vector3 direction = Vector3.one;
    private Vector3 defaultDirection = Vector3.one;

    void Start()
    {
        minAngle = minAngle * (2 * Mathf.PI) / 360;
        maxAngle = maxAngle * (2 * Mathf.PI) / 360;
    }
    void LateUpdate()
    {
        float degreeHorizontal = 0;
        float degreeVertical = 0;

        // 检测按键输入
        if (Input.GetKey(KeyCode.W))
        {
            degreeVertical = 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            degreeVertical = -1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            degreeHorizontal = 1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            degreeHorizontal = -1;
        }

        // 旋转方向
        Quaternion rot = Quaternion.identity;
        // 如果 degreeVertical 不为 0，表示需要绕 local right 旋转
        if (degreeVertical != 0)
        {
            // 使用 local right 作为旋转轴
            rot = Quaternion.AngleAxis(degreeVertical * rotate_speed * Time.deltaTime, cameraTransform.right);
        }

        // 如果 degreeHorizontal 不为 0，表示需要绕 local up 旋转
        if (degreeHorizontal != 0)
        {
            // 使用 local up 作为旋转轴
            rot *= Quaternion.AngleAxis(degreeHorizontal * rotate_speed * Time.deltaTime, cameraTransform.up);
            // 更新默认Dir
            defaultDirection = Quaternion.AngleAxis(degreeHorizontal * rotate_speed * Time.deltaTime, cameraTransform.up)*defaultDirection.normalized;
        }

        // 限制角度
        // 计算夹角
        float angle = Vector3.Angle(Vector3.forward, rot * direction.normalized);
        // 如果夹角超出 0° 到 90° 的范围，进行调整
        if (angle > 80f || angle < 40f)
        {
            StartCoroutine(ResetCameraRotation());
            return;
        }


        // 更新方向
        direction = rot * direction.normalized;

        // 绘制调试线
        Debug.DrawLine(cameraTransform.position, cameraTransform.position - direction * distance, Color.red);

        // 移动到"原"点
        //cameraTransform.position = transform.position;
        // 计算目标旋转
        var targetRight = Vector3.Cross(direction, Vector3.forward);
        Vector3 target_up = Vector3.Cross(direction, targetRight);
        Quaternion targetRotation = Quaternion.LookRotation(direction, target_up);
        // 应用目标旋转
        cameraTransform.rotation = Quaternion.Slerp(cameraTransform.rotation, targetRotation, rotate_speed * Time.deltaTime);
        // 更新相机位置
        // cameraTransform.position = transform.position + direction * distance;
        // 平滑更新相机位置
        cameraTransform.position = Vector3.Lerp(cameraTransform.position, transform.position + direction * distance, rotate_speed * Time.deltaTime);
    }
    /*
    void Update()
    {
        float degreeHorizontal = 0;
        float degreeVertical = 0;
        if (Input.GetKey(KeyCode.W))
        {
            degreeVertical = 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            degreeVertical = -1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            degreeHorizontal = -1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            degreeHorizontal = 1;
        }
        Quaternion rot = Quaternion.Euler(degreeVertical * rotate_speed * Time.deltaTime, 0, degreeHorizontal * rotate_speed * Time.deltaTime);

        direction = rot * direction.normalized;
        Debug.DrawLine(cameraTransform.position, cameraTransform.position - direction * distance, Color.red);



        cameraTransform.position = transform.position;

        cameraTransform.position = transform.position + direction * distance;

        var targetRight = Vector3.Cross(direction, Vector3.forward);
        Vector3 target_up = Vector3.Cross(direction, targetRight);

        Quaternion targetRotation = Quaternion.LookRotation(direction, target_up);

        cameraTransform.rotation = targetRotation;
        // var rot2 = Quaternion.FromToRotation(cameraTransform.forward, direction);
        // cameraTransform.rotation = rot2 * cameraTransform.rotation;

        // cameraTransform.forward = direction.normalized;


        // var targetRight = Vector3.Cross(direction, Vector3.forward);
        // var rot2 = Quaternion.FromToRotation(cameraTransform.right, targetRight);
        // cameraTransform.rotation = rot2 * cameraTransform.rotation;
    }


    */

    IEnumerator ResetCameraRotation()
    {
        yield return new WaitForSeconds(0.1f);
        while (true)
        {
            yield return null;
  
            direction = Vector3.Lerp(direction, defaultDirection, 0.1f * Time.deltaTime).normalized;
            // 计算夹角
            float angle = Vector3.Angle(defaultDirection, direction);
            if (angle < 5.0f)
            {
                break;
            }
        }
        Debug.Log("结束");

    }
}
