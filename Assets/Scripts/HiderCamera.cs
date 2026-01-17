using Fusion;
using UnityEngine;

public class HiderCamera : NetworkBehaviour
{
    public Transform Target;
    public Vector3 Offset = new Vector3(0, 2, -4);

    [Range(0.1f, 2f)]
    public float Sensitivity = 1f;

    public bool InvertX;
    public bool InvertY;

    private Camera _cam;
    private float _yaw;
    private float _pitch;

    private void Awake()
    {
        _cam = GetComponentInChildren<Camera>();
    }
    
    public override void Spawned()
    {
        if (!HasInputAuthority)
        {
            _cam.gameObject.SetActive(false);
            enabled = false;
            return;
        }
    }
    private void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * 90f * Sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * 90f * Sensitivity * Time.deltaTime;

        if (InvertX) mouseX *= -1;
        if (InvertY) mouseY *= -1;

        _yaw += mouseX;
        _pitch -= mouseY;
        _pitch = Mathf.Clamp(_pitch, -40f, 75f);
    }

    private void LateUpdate()
    {
        if (Target == null) return;

        Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0);
        transform.position = Target.position + rotation * Offset;
        transform.LookAt(Target.position + Vector3.up * 1.5f);
    }
    
    public float GetYaw()
    {
        return _yaw;
    }
}