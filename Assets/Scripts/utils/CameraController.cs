using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static bool MovementEnabled = true;
    public float panSpeed = 20f;
    public float panBorderThickness = 10f;
    public Vector2 panLimit;
    public float scrollSpeed = 20f;
    public Vector2 scrollLimit;

    private Camera _camera;

    private void Start()
    {
        _camera = GetComponent<Camera>();
    }

    private void Update()
    {
        if (!MovementEnabled) return;

        var pos = transform.position;
        var size = _camera.orthographicSize;

        if (Input.GetKey("w") || Input.mousePosition.y >= Screen.height - panBorderThickness)
            pos.y += panSpeed * Time.deltaTime;
        if (Input.GetKey("s") || Input.mousePosition.y <= panBorderThickness) pos.y -= panSpeed * Time.deltaTime;
        if (Input.GetKey("a") || Input.mousePosition.x <= panBorderThickness) pos.x -= panSpeed * Time.deltaTime;
        if (Input.GetKey("d") || Input.mousePosition.x >= Screen.height - panBorderThickness)
            pos.x += panSpeed * Time.deltaTime;

        var scroll = Input.GetAxis("Mouse ScrollWheel");
        size -= scroll * scrollSpeed * Time.deltaTime;

        pos.x = Mathf.Clamp(pos.x, -panLimit.x, panLimit.x);
        pos.y = Mathf.Clamp(pos.y, -panLimit.y, panLimit.y);
        size = Mathf.Clamp(size, scrollLimit.x, scrollLimit.y);

        _camera.orthographicSize = size;
        transform.position = pos;
    }
}