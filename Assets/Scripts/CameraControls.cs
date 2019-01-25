using UnityEngine;
using System.Collections;

public class CameraControls : MonoBehaviour
{

    public float zoomSpeed = 1;
    public float targetOrtho;
    public float smoothSpeed = 2.0f;
    public float minOrtho = 1.0f;
    private float maxOrtho = 100.0f;

    public float mouseSensitivity = 1.0f;

    public Transform background;

    private Vector3 lastPosition;

    private float cameraWidth;
    private float cameraHeight;
    private float aspect;

    private float gameWidth;
    private float gameHeight;

    void Start()
    {
        targetOrtho = Camera.main.orthographicSize;

        aspect = Camera.main.aspect;
        cameraHeight = Camera.main.orthographicSize * 2;
        cameraWidth = cameraHeight * aspect;

        gameWidth = background.localScale.x;
        gameHeight = background.localScale.y;

        maxOrtho = Mathf.Min(gameHeight / 2, (gameWidth / aspect ) / 2 );
    }

    void Update()
    {
        // Zoom
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0.0f)
        {
            targetOrtho -= scroll * zoomSpeed;
            targetOrtho = Mathf.Clamp(targetOrtho, minOrtho, maxOrtho);
        }

        Camera.main.orthographicSize = Mathf.MoveTowards(Camera.main.orthographicSize, targetOrtho, smoothSpeed * Time.deltaTime);

        cameraWidth = Camera.main.orthographicSize * 2;
        cameraHeight = cameraWidth * aspect;

        // Pan
        if (Input.GetMouseButtonDown(0))
        {
            lastPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 delta = Camera.main.ScreenToWorldPoint(Input.mousePosition) - lastPosition;
            transform.Translate(-delta.x * mouseSensitivity, -delta.y * mouseSensitivity, 0);
            lastPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }
}