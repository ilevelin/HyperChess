using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowMarker : MonoBehaviour
{
    [SerializeField] Transform head, body, tail;
    [HideInInspector] public int[] headPosition, tailPosition;

    public void Initialize()
    {
        tail.position = new Vector3(tailPosition[0], tailPosition[1]);
        head.localPosition = new Vector3(headPosition[0] - tailPosition[0], headPosition[1] - tailPosition[1]);
        Render();
    }

    private void Render() 
    {
        float distance = Vector3.Distance(head.transform.position, tail.transform.position);
        body.localScale = new Vector3(body.transform.localScale.x, distance - 0.5f, body.transform.localScale.z);
        float rot = (Mathf.Atan2(head.localPosition.y, head.localPosition.x) * Mathf.Rad2Deg) - 90;
        if (distance != 0)
        {
            head.localEulerAngles = new Vector3(0.0f, 0.0f, rot);
            body.localEulerAngles = new Vector3(0.0f, 0.0f, rot);
        }
        else
        {
            head.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
            body.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
        }
    }

    public void Remove()
    {
        GameObject.Destroy(gameObject);
    }
}
