using UnityEngine;

public class MinimapCameraFollow : MonoBehaviour
{
    public Transform carTransform; 
    public Vector3 offset;

    void LateUpdate()
    {
        if (carTransform != null)
        {
            Vector3 newPosition = carTransform.position + offset;
            transform.position = newPosition;

            transform.rotation = Quaternion.Euler(90f, carTransform.eulerAngles.y, 0f);
        }
    }

}
