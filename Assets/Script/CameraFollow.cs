using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameController controller;
    public float lerpSpeed = 2f;
    
    private float minY = 5f;
    private float yOffset = 2f; 

    void LateUpdate()
    {
        float targetY = Mathf.Max(minY, controller.highestY + yOffset);
        
        Vector3 targetPosition = new Vector3(0, targetY, -10f);
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * lerpSpeed);
    }
}