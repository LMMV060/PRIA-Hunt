using UnityEngine;

public class HunterCamera : MonoBehaviour
{
    public Transform Target;
    public float MouseSensitivity = 10f;

    private float vertialRotation;
    private float horizontalRotation;

    void LateUpdate()
    {
        if (Target == null)
        {
            return;
        }

        transform.position = Target.position+Vector3.up;      

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        vertialRotation -= mouseY * MouseSensitivity;
        vertialRotation = Mathf.Clamp(vertialRotation, -70f, 70f);

        horizontalRotation += mouseX * MouseSensitivity;

        transform.rotation = Quaternion.Euler(vertialRotation, horizontalRotation, 0);
    }
}
