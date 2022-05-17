using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceTo : MonoBehaviour
{
    public Transform target;

    private float Angle(Transform from, Transform to)
    {
        float angle = Vector3.Angle(from.up, to.position - from.position);
        Vector3 normal = Vector3.Cross(from.up, to.position - from.position);
        angle *= Mathf.Sign(Vector3.Dot(normal, Vector3.back));
        return angle;
    }

    private float Angle(Transform from, Vector3 to)
    {
        float angle = Vector3.Angle(from.up, to - from.position);
        Vector3 normal = Vector3.Cross(from.up, to - from.position);
        angle *= Mathf.Sign(Vector3.Dot(normal, Vector3.back));
        return angle;
    }

    private void Update()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);//求得世界坐标 

        MathF.FaceTo(transform, target.position, 180);
        print(target.position - mousePosition);
    }
}
