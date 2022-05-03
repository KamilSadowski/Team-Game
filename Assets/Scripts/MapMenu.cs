using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GameObject;

public class MapMenu : MonoBehaviour
{
    [SerializeField] private CanvasGroup MapCanvas;
    [SerializeField] private Camera MinimapCamera;
    private static float animationTime = .5f;
    public bool isVisible;

    private Vector2Int PrevMousePos;

    private float OriginalMinimapSize;
    private Vector3 OriginalCameraPosition;

    // Start is called before the first frame update
    void Start()
    {
        isVisible = false;
        MapCanvas.alpha = 0f;

        MinimapCamera = FindGameObjectWithTag("MinimapCamera")?.GetComponent<Camera>();

        // Set original camera properties
        if (MinimapCamera)
        {
            OriginalMinimapSize = MinimapCamera.orthographicSize;
        }

    }

    void ResetCamera()
    {
        if (MinimapCamera)
        {
            MinimapCamera.orthographicSize = OriginalMinimapSize;
            MinimapCamera.transform.localPosition = Vector3.zero;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            // Toggle map
            if (MapCanvas)
            {
                StartCoroutine(ShowCanvas(MapCanvas, MapCanvas.alpha < 0.5f ? 1.0f : 0f, true));
                isVisible = !isVisible;


                if (isVisible)
                {
                    // Set pause
                    Time.timeScale = .0f;

                    Cursor.visible = true;
                }
                else
                {
                    Time.timeScale = 1f;
                    Cursor.visible = false;
                    
                    ResetCamera();
                }
            }
        }

        if (isVisible && MinimapCamera)
        {
            var rect = MinimapCamera.rect;

            // Handle camera movement across the map
            if (Input.GetMouseButton(0))
            {
                var mousePos = new Vector2Int((int)Input.mousePosition.x, (int)Input.mousePosition.y);
                var diff = mousePos - PrevMousePos;

                if (diff != Vector2Int.zero)
                {
                    MinimapCamera.transform.position += new Vector3(-diff.x,-diff.y,0f) / 10f;
                    MinimapCamera.rect = rect;
                }

            }

            // Handle Map zoom

            var delta = Input.mouseScrollDelta.y;
            var size = MinimapCamera.orthographicSize;
            if (delta < 0f && size < 20f)
            {
                MinimapCamera.orthographicSize -= Input.mouseScrollDelta.y * 3;
            }
            else if (delta > 0f && size > 3f)
            {
                MinimapCamera.orthographicSize -= Input.mouseScrollDelta.y * 3;
            }

        }

        PrevMousePos = new Vector2Int((int)Input.mousePosition.x, (int)Input.mousePosition.y);
    }


    private IEnumerator ShowCanvas(CanvasGroup group, float target, bool isBlockRaycast)
    {
        if (group != null)
        {
            float startAlpha = group.alpha;
            float t = 0.0f;

            group.interactable = target >= 1.0f;
            group.blocksRaycasts = isBlockRaycast;

            while (t < animationTime)
            {
                t = Mathf.Clamp(t + Time.unscaledDeltaTime, 0.0f, animationTime);
                group.alpha = Mathf.SmoothStep(startAlpha, target, t / animationTime);
                yield return null;
            }
        }
    }
}
