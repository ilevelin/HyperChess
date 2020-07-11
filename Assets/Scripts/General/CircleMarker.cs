using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleMarker : MonoBehaviour
{

    [HideInInspector] public int[] position;

    public void Initialize()
    {
        transform.position = new Vector3(position[0], position[1]);
    }

    internal void Remove()
    {
        GameObject.Destroy(gameObject);
    }
}
