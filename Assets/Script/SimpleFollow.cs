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

      
        Vector3 currentDir = (transform.position - target.position).normalized;

        
        if (currentDir == Vector3.zero)
        {
           
            currentDir = Vector3.forward;
        }
        Vector3 desiredPosition = target.position + currentDir * fixedDistance;

            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
     
            transform.LookAt(target.position);
        
    }
}
