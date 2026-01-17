using Fusion;
using UnityEngine;

public class SpectatorMovement : NetworkBehaviour
{
    private CharacterController _controller;

    public float MoveSpeed = 5f;
    public float VerticalSpeed = 5f;

    public Camera Camera;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            Camera.GetComponent<HiderCamera>().Target = transform;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority)
            return;

        // Movimiento horizontal basado en la cámara
        Vector3 forward = Camera.transform.forward;
        Vector3 right = Camera.transform.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        Vector3 move = forward * Input.GetAxis("Vertical") + right * Input.GetAxis("Horizontal");

        // Movimiento vertical
        if (Input.GetKey(KeyCode.Space))
        {
            move.y += 1f; // subir
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            move.y -= 1f; // bajar
        }

        _controller.Move(move * MoveSpeed * Runner.DeltaTime);

        // Rotar el espectador hacia la dirección de movimiento (opcional)
        if (move.sqrMagnitude > 0.001f)
        {
            transform.forward = new Vector3(move.x, 0, move.z);
        }
    }
}