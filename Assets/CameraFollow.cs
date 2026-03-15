using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // 拖入你的角色Transform
    public float smoothSpeed = 0.125f; // 跟随平滑度（0-1，越小越丝滑）
    private Vector3 offset = new Vector3(0, 0, -10); // 2D相机Z轴固定为-10

    void LateUpdate()
    {
        if (target == null) return;

        // 计算目标位置：角色位置 + 偏移（保证角色在屏幕中心）
        Vector3 desiredPosition = target.position + offset;
        // 平滑移动相机（可选，去掉就是瞬间居中）
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}