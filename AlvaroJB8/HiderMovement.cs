using UnityEngine;
using Fusion;

public class HiderMovement : NetworkBehaviour
{
    private CharacterController _controller;
    private Vector3 _velocity;
    private bool _jumpPressed;

    public float PlayerSpeed = 5f;
    public float JumpForce = 5f;
    public float GravityValue = -9.81f;
    public float RotationSpeed = 10f;

    public HiderCamera Camera;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (!HasStateAuthority) return;

        if (Input.GetButtonDown("Jump"))
        {
            _jumpPressed = true;
        }
    }

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            Camera.Target = transform;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority) return;

        if (_controller.isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Quaternion camRot = Quaternion.Euler(0, Camera.GetYaw(), 0);
        Vector3 move = camRot * new Vector3(h, 0, v);
        move.Normalize();

        _controller.Move(move * PlayerSpeed * Runner.DeltaTime);
        
        if (move.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                RotationSpeed * Runner.DeltaTime
            );
        }
        
        _velocity.y += GravityValue * Runner.DeltaTime;

        if (_jumpPressed && _controller.isGrounded)
        {
            _velocity.y = JumpForce;
        }

        _controller.Move(_velocity * Runner.DeltaTime);
        _jumpPressed = false;
    }
}