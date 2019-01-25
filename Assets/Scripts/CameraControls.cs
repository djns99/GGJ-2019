using UnityEngine;
using System.Collections;

public class CameraControls : MonoBehaviour
{

    public float zoomSpeed = 1;
    public float targetOrtho;
    public float smoothSpeed = 2.0f;
    public float minOrtho = 1.0f;
    public float maxOrtho = 20.0f;


    public float mouseSensitivity = 1.0f;

    public float worldWidth = 100.0f;
    public float worldHeight = 75.0f;

    private Vector3 lastPosition;

    void Start()
    {
        targetOrtho = Camera.main.orthographicSize;
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