using UnityEngine;

public class BackgroundImage : MonoBehaviour
{
    [SerializeField] private float speedX;
    [SerializeField] private float speedY;
    [SerializeField] private float criticalLeft;
    [SerializeField] private float criticalRight;
    [SerializeField] private float criticalTop;
    [SerializeField] private float criticalBottom;
    private float _currentSpeedX;
    private float _currentSpeedY;
    private static float _speedX;
    private static float _speedY;

    private bool _directionLeft;
    private bool _directionTop;
    public static BackgroundImage Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        if (_speedX == 0) _speedX = speedX;
        if (_speedY == 0) _speedY = speedY;
        Instance = this;
        DontDestroyOnLoad(transform.gameObject);
    }

    private void Start()
    {
        _currentSpeedX = -_speedX / 10000;
        _currentSpeedY = _speedY / 10000;
    }

    private void Update()
    {
        gameObject.transform.Translate(_currentSpeedX, _currentSpeedY, 0);

        if (_directionLeft)
        {
            if (gameObject.transform.position.x > criticalLeft)
            {
                _currentSpeedX = -_speedX / 10000;
                _directionLeft = false;
            }
        }
        else
        {
            if (gameObject.transform.position.x < criticalRight)
            {
                _currentSpeedX = _speedX / 10000;
                _directionLeft = true;
            }
        }

        if (_directionTop)
        {
            if (gameObject.transform.position.y < criticalTop)
            {
                _currentSpeedY = _speedY / 10000;
                _directionTop = false;
            }
        }
        else
        {
            if (gameObject.transform.position.y > criticalBottom)
            {
                _currentSpeedY = -_speedY / 10000;
                _directionTop = true;
            }
        }
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}