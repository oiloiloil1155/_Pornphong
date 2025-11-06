using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRoomCameraTrigger : MonoBehaviour
{
    [Header("Zoom Settings")]
    public float zoomOutSize = 8f;     // ขนาดกล้องตอนเข้าห้องบอส
    public float zoomInSize = 5f;      // ขนาดกล้องปกติ
    public float zoomDuration = 1.5f;  // เวลาที่ใช้ในการเปลี่ยนขนาด

    private bool isInBossRoom = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isInBossRoom)
        {
            isInBossRoom = true;
            StopAllCoroutines();
            StartCoroutine(ZoomCamera(zoomOutSize));
        }
    }

    

    private IEnumerator ZoomCamera(float targetSize)
    {
        Camera cam = Camera.main;
        float startSize = cam.orthographicSize;
        float elapsed = 0f;

        while (elapsed < zoomDuration)
        {
            cam.orthographicSize = Mathf.Lerp(startSize, targetSize, elapsed / zoomDuration);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        cam.orthographicSize = targetSize;
    }
}
