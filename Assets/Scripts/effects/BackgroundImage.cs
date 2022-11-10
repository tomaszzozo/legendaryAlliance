using UnityEngine;

public class BackgroundImage : MonoBehaviour
{
    public static BackgroundImage Instance { get; private set; }
    public static BackgroundImage Instance2 { get; private set; }
    private Vector2 _initialPosition;

    [SerializeField] private float speed;

    [SerializeField] private float resetPosition;

    public void Destroy()
    {
        Destroy(gameObject.GetComponent<SpriteRenderer>());
        Destroy(this);    
    }
    
    private void Awake()
    {
        if (Instance != null && Instance != this && Instance2 != null && Instance2 != this) 
        { 
            Destroy();
        } 
        else
        {
            if (Instance == null) { Instance = this; }
            else Instance2 = this;
        }
        DontDestroyOnLoad(transform.gameObject);
    }

    private void Start()
    {
        _initialPosition = transform.position;
    }

    private void Update()
    {
        transform.Translate(-speed, 0, 0);
        if (resetPosition == 0 || !(transform.position.x <= resetPosition)) return;
        
        Instance.transform.position = Instance._initialPosition;
        Instance2.transform.position = Instance2._initialPosition;
    }
}
