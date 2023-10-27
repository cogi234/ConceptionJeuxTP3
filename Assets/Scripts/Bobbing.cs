using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bobbing : MonoBehaviour
{
    [SerializeField] float amplitude;
    [SerializeField] float frequency;

    float originalHeight;

    private void Awake()
    {
        originalHeight = transform.localPosition.y;
    }

    private void Update()
    {
        Vector3 newPosition = transform.localPosition;
        newPosition.y = originalHeight + (amplitude * Mathf.Sin(frequency * Time.time));
        transform.localPosition = newPosition;
    }
}
