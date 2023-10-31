using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantMovement : MonoBehaviour
{
    [SerializeField] Vector3 direction;
    [SerializeField] float speed, timeScale;

    private void Awake()
    {
        Time.timeScale = timeScale;
    }

    private void Update()
    {
        transform.Translate(direction.normalized * speed * Time.deltaTime, Space.Self);
    }
}
