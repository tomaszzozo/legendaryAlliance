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

        Instance = this;
        DontDestroyOnLoad(transform.gameObject);
    }

    private void Start()
    {
        _currentSpeedX = -speedX / 10000;
        _currentSpeedY = speedY / 10000;
    }

    private void Update()
    {
        gameObject.transform.Translate(_currentSpeedX, _currentSpeedY, 0);

        if (_directionLeft)
        {
            if (gameObject.transform.position.x > criticalLeft)
            {
                _currentSpeedX = -speedX / 10000;
                _directionLeft = false;
            }
        }
        else
        {
            if (gameObject.transform.position.x < criticalRight)
            {
                _currentSpeedX = speedX / 10000;
                _directionLeft = true;
            }
        }

        if (_directionTop)
        {
            if (gameObject.transform.position.y < criticalTop)
            {
                _currentSpeedY = speedY / 10000;
                _directionTop = false;
            }
        }
        else
        {
            if (gameObject.transform.position.y > criticalBottom)
            {
                _currentSpeedY = -speedY / 10000;
                _directionTop = true;
            }
        }
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}