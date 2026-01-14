using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton
    // singleton pattern
    private static GameManager _instance;
    public static GameManager Instance => _instance;
    
    private void Awake()
    {
        // Game Manager "complete singleton"
        if (_instance != null)
        {
            if (_instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                DontDestroyOnLoad(this.gameObject);
            }
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
    #endregion
    
    // Player GameObject with getter&setter
    [SerializeField] private GameObject _player;
    public GameObject Player
    {
        get { return _player; }
        set { _player = value; }
    }
    
    // GameObjects for camera limits with getter&setter
    [SerializeField] private GameObject _cameraLimitLeft;
    public GameObject CameraLimitLeft
    {
        get { return _cameraLimitLeft; }
        set { _cameraLimitLeft = value; }
    }
    [SerializeField] private GameObject _cameraLimitRight;
    public GameObject CameraLimitRight
    {
        get { return _cameraLimitRight; }
        set { _cameraLimitRight = value; }
    }
    
}
