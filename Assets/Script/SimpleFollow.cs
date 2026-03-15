using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleFollow : MonoBehaviour
{
    public Transform target;      
    public float smoothSpeed = 5f;
    public float fixedDistance = 0.5f;

    void LateUpdate()
    {
        if (target == null) return;

        // 1. 计算从target指向当前物体的方向
        Vector3 currentDir = (transform.position - target.position).normalized;

        // 2. 如果距离恰好是0（重合），则需要一个默认方向
        if (currentDir == Vector3.zero)
        {
            // 可以给一个默认方向，例如世界Z轴
            currentDir = Vector3.forward;
        }
        Vector3 desiredPosition = target.position + currentDir * fixedDistance;

            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
     
            transform.LookAt(target.position);
        
    }
}
