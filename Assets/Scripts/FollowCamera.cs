using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 localOffset = new Vector3(0, 7, -12);
    public Vector3 lookOffset = new Vector3(0, 1, 5);
    public float smoothing = 5f;
    public bool followRotation = true;

    private void LateUpdate()
    {
        if (target == null) return;
        Vector3 desired = followRotation ? target.TransformPoint(localOffset) : target.position + localOffset;
        transform.position = Vector3.Lerp(transform.position, desired, Time.deltaTime * smoothing);
        transform.LookAt(target.position + (followRotation ? target.TransformDirection(lookOffset) : lookOffset));
    }
}
