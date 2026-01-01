using Fusion;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    private Vector3 _velocity;
    private bool _jumpPressed;
    private bool _resetPressed;
    private CharacterController _controller;
    [SerializeField] private Transform spawnPointLobby;

    public float PlayerSpeed = 2f;
    public float Sprint = 2f;
    public float JumpForce = 5f;
    public float GravityValue = -9.81f;

    public Camera Camera;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            _jumpPressed = true;
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            _resetPressed = true;
        }
    }
    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {            
            Camera.GetComponent<HunterCamera>().Target = transform;
        }
    }


    public override void FixedUpdateNetwork()
    {
        // FixedUpdateNetwork is only executed on the StateAuthority
        if (HasStateAuthority == false)
        {
            return;
        }
        
        if (_resetPressed)
        {
            TeleportToOrigin();
            _resetPressed = false;
            return;
        }

        if (_controller.isGrounded)
        {
            _velocity = new Vector3(0, -1, 0);
        }
        
        float currentSpeed = PlayerSpeed;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed *= Sprint;
        }
        
        Quaternion cameraRotationY = Quaternion.Euler(0, Camera.transform.rotation.eulerAngles.y, 0);

        Vector3 move = cameraRotationY * new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * Runner.DeltaTime * currentSpeed;

        _velocity.y += GravityValue * Runner.DeltaTime;
        
        if (_jumpPressed && _controller.isGrounded)
        {
            _velocity.y += JumpForce;
        }
        _controller.Move(move + _velocity * Runner.DeltaTime);

        if (move != Vector3.zero)
        {
            gameObject.transform.forward = move;
        }

        _jumpPressed = false;
    }
    
    private void TeleportToOrigin()
    {
        _controller.enabled = false;
        transform.position = spawnPointLobby.position;
        _velocity = Vector3.zero;
        _controller.enabled = true;
    }
    
}