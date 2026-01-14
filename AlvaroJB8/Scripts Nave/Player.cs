using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    public enum LateralDirection
    {
        None = 0,
        Left = -1,
        Right = 1
    }
    [Header("Physics")]
    [SerializeField] private Vector2 speed = new Vector2(5f, 8f);

    [Header("Sprites")]
    [SerializeField] private Sprite spriteNormal;
    [SerializeField] private Sprite spriteTurnLeft01;
    [SerializeField] private Sprite spriteTurnLeft02;
    [SerializeField] private Sprite spriteTurnRight01;
    [SerializeField] private Sprite spriteTurnRight02;

    [Header("Scroll")]
    [SerializeField] private bool enableCameraFollow = true;
    [SerializeField] private Camera mainCamera;
    
    [Header("Fire")]
    [SerializeField] private GameObject firePrefab;
    
    private Rigidbody2D _rigidbody2D;
    private SpriteRenderer _spriteRenderer;
    private Vector2 _direction;
    private float _timeTurning;
    private LateralDirection _lastLateralDirection = LateralDirection.None;

    private Transform _cameraLimitLeft;
    private Transform _cameraLimitRight;

    private void Awake()
    {
        //get reference to RigidBody
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        GameManager.Instance.Player = this.gameObject;
    }

    private void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        _cameraLimitLeft = GameManager.Instance.CameraLimitLeft.transform;
        _cameraLimitRight = GameManager.Instance.CameraLimitRight.transform;
        UpdateCamera();
    }
    private void Update()
    {
        GetInputs();
        UpdatePhysics();
        UpdateGraphics();
    }

    private void LateUpdate()
    {
        UpdateCamera();
    }
    
    private void UpdateGraphics()
    {
        // sprite representation switch turning time
        if (_timeTurning >= 0.5f)
        {
            // set sprite right or left depending on the lateral desired direction sign
            if (_direction.x == 0)
            {
                // use the last lateral direction
                _spriteRenderer.sprite = (_lastLateralDirection == LateralDirection.Right) ? spriteTurnRight02 : spriteTurnLeft02;
            }
            else
            {
                _spriteRenderer.sprite = (_direction.x > 0) ? spriteTurnRight02 : spriteTurnLeft02;
            }
        }
        else if (_timeTurning > 0f)
        {
            // set sprite right or left depending on the lateral desired direction sign
            if (_direction.x == 0)
            {
                // use the last lateral direction
                _spriteRenderer.sprite = (_lastLateralDirection == LateralDirection.Right) ? spriteTurnRight01 : spriteTurnLeft01;
            }
            else
            {
                _spriteRenderer.sprite = (_direction.x > 0) ? spriteTurnRight01 : spriteTurnLeft01;
            }
        }
        else
        {
            _spriteRenderer.sprite = spriteNormal;
        }
    }

    private void GetInputs()
    {
        //movement direction reset
        _direction = Vector2.zero;
        
        //adds the top and bottom components direction
        if (Input.GetKey(KeyCode.W))
        {
            _direction += Vector2.up;
        }

        if (Input.GetKey(KeyCode.S))
        {
            _direction += Vector2.down;
        }
        
        if (Input.GetKey(KeyCode.D))
        {
            _direction += Vector2.right;
            _lastLateralDirection = LateralDirection.Right;
        }
        if (Input.GetKey(KeyCode.A))
        {
            _direction += Vector2.left;
            _lastLateralDirection = LateralDirection.Left;
        }

        // reset turning time when the player start moving
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            _timeTurning = 0;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Fire();
        }
    }
    
    private void Fire()
    {
        // Create a new GameObject "firePrefab"
        GameObject fire = Instantiate(firePrefab);
        fire.transform.position = transform.position;
    }

    private void UpdateCamera()
    {
        if (enableCameraFollow)
        {
            if (mainCamera)
            {
                Vector3 cameraPosition = mainCamera.transform.position;
                cameraPosition.y = transform.position.y;

                if (_cameraLimitLeft && _cameraLimitRight)
                {
                    float cameraHalfWidth = mainCamera.orthographicSize * mainCamera.aspect;
                    if ((transform.position.x - cameraHalfWidth) > _cameraLimitLeft.position.x &&
                        (transform.position.x - cameraHalfWidth) < _cameraLimitRight.position.x)
                    {
                        cameraPosition.x = transform.position.x;
                    }
                }
                else
                {
                    cameraPosition.x = transform.position.x;
                }
                mainCamera.transform.position = cameraPosition;
            }
        }
    }

    private void UpdatePhysics()
    {
        // limit the lateral movement of the player (when camera is in the lateral limits) to stop the player at the screen edge
        float halfWidth = _spriteRenderer.bounds.extents.x;
        float cameraHalfWidth = mainCamera.orthographicSize * mainCamera.aspect;
        // simulate lateral movement at linear velocity
        Vector3 pos = transform.position + _direction.x * speed.x * Time.deltaTime * Vector3.right;
        Vector3 newDirection= _direction;
        if ((pos.x < mainCamera.transform.position.x - cameraHalfWidth + halfWidth) && _direction.x < 0)
        {
            newDirection.x = 0;
        }
        if ((pos.x > mainCamera.transform.position.x + cameraHalfWidth - halfWidth) && _direction.x > 0)
        {
            newDirection.x = 0;
        }
        // apply linear velocity
        _rigidbody2D.linearVelocity = new Vector2(newDirection.x * speed.x, newDirection.y * speed.y);
    
        // update the "turning time" of the plane if the "lateral speed direction" [originalDirection vs real speed] (in any direction)
        // is greater than a minimum threshold (0.1) to avoid flat plane turning when the player is on the screen edge
        if (Mathf.Abs(_direction.x) > 0.1f)
        {
            _timeTurning += Time.deltaTime;
            _timeTurning = Mathf.Min(_timeTurning, 0.5f);
        }
        else
        {
            // return to the horizontal position (slowly)
            _timeTurning -= Time.deltaTime * 4f;
            _timeTurning = Mathf.Max(_timeTurning, 0f);
        }
    }

}
