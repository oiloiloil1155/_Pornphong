using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
public class BGFollow : MonoBehaviour
{
    public Transform player;
    public Vector3 offset;
    private bool isFixed = false;
    private Vector3 fixedPosition;
    private void Update()
    {
        if (!isFixed) // ปกติ: กล้องตามผู้เล่น
        {
            transform.position = new Vector3(
                player.position.x + offset.x,
                player.position.y + offset.y,
                offset.z);
        }
        else // ถ้า Fix: กล้องจะอยู่ตำแหน่งเดียว
        {
            transform.position = fixedPosition;
        }

    }
    public void FixCamera(Vector3 pos)
    {
        isFixed = true;
        fixedPosition = new Vector3(pos.x, pos.y, offset.z);
    }

    // ✅ ฟังก์ชันปลดล็อก (กลับมาตามผู้เล่น)
    public void ReleaseCamera()
    {
        isFixed = false;
    }
}
