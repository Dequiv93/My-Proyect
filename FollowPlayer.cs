using Unity.VisualScripting;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform target;
    public float height = 10f;
    public float distance = 10f;
    public float smoothSpeed = 10.125f;



    // Update is called once per frame
    void LateUpdate()
    {
        Vector3  desirePosition = target.position + Quaternion.Euler(45f, 45f, 0f) * new Vector3(0, height, -distance);

        Vector3 smoothPosition = Vector3.Lerp(transform.position, desirePosition, smoothSpeed);
        transform.position = smoothPosition;

        transform.LookAt(target);
    }
}

