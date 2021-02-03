using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSlider : MonoBehaviour
{
    private RectTransform rectTransform;
    bool isDragging = false;

    private void Start()
    {
        rectTransform = this.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(0, Screen.height - 100);
    }

    private void Update()
    {
        if (isDragging)
        {
            float mousePosition = Mathf.Clamp(Input.mousePosition.y - 50, 0, Screen.height - 100);
            rectTransform.anchoredPosition = new Vector2(0, mousePosition);
        }
    }

    public void MouseDown()
    {
        isDragging = true;
    }

    public void MouseUp()
    {
        isDragging = false;
    }
    
    public float GetScrollerPosition()
    {
        return rectTransform.anchoredPosition.y / (Screen.height - 100);
    }
}
