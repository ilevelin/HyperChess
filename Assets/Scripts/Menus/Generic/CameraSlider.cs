using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSlider : MonoBehaviour
{
    [SerializeField] MenuSlider slider;
    private Camera camera;
    bool started = false;

    const float minPosition = -0.5f;
    float difPosition = 0.0f;

    public void AfterLibraryLoaded(int loadedPieces)
    {
        difPosition = -0.5f * Mathf.Max(loadedPieces - 3, 0);
        camera = gameObject.GetComponent<Camera>();
        camera.orthographicSize = (16f / 9f) / (((float)Screen.width) / ((float)Screen.height));
        transform.position = new Vector3((((float)Screen.width) / ((float)Screen.height)) * camera.orthographicSize, 0f, -10f);
        started = true;
    }

    void Update()
    {
        if (started)
        {
            float objectivePosition = minPosition + (1 - slider.GetScrollerPosition()) * difPosition;

            transform.position = new Vector3(transform.position.x, objectivePosition, -10f);
        }
    }
}
