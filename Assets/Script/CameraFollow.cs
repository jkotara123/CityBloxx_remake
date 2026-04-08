using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameController controller;
    public float lerpSpeed = 2f;
    public Vector3 offset = new Vector3(0, 2, -10);
    
    void LateUpdate()
    {
        float targetY = controller.highestY + offset.y;
        Vector3 targetPosition = new Vector3(0, targetY, offset.z);

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * lerpSpeed);
    }
}