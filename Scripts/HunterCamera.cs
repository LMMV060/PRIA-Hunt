using Fusion;
using UnityEngine;

public class HunterCamera : NetworkBehaviour
{
    public Transform Target;
    public float MouseSensitivity = 10f;

    private float vertialRotation;
    private float horizontalRotation;

    void Start()
    {
        if (!Object.HasStateAuthority)
        {
            GetComponent<Camera>().enabled = false;
            return;
        }
    }

    void LateUpdate()
    {
        if (!Object.HasStateAuthority || Target == null) return;

        transform.position = Target.position + Vector3.up;

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        vertialRotation -= mouseY * MouseSensitivity;
        vertialRotation = Mathf.Clamp(vertialRotation, -70f, 70f);

        horizontalRotation += mouseX * MouseSensitivity;

        transform.rotation = Quaternion.Euler(vertialRotation, horizontalRotation, 0);
    }
}
