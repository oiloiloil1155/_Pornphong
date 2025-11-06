using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flyitem : MonoBehaviour
{
    public float floatSpeed = 2f;     // ความเร็วการลอย
    public float floatHeight = 0.25f; // ระยะขึ้นลง

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // ลอยขึ้นลงตามเวลา
        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }
}
