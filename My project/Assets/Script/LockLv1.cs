using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using static UnityEngine.GraphicsBuffer;

public class LockLv1 : MonoBehaviour
{
   [SerializeField] private GameObject wallBlock; // กำแพง/ประตูที่จะปิดทาง
   [SerializeField] private GameObject wallBlock2;

    [SerializeField] private GameObject bossspawn;

    [SerializeField] private GameObject hp; // กำแพง/ประตูที่จะปิดทาง
    [SerializeField] private GameObject hpbar;

    [SerializeField] private CameraFollow cameraFollow;
    [SerializeField] private BGFollow bgFollow;
    [SerializeField] private Transform bossRoomCamPos; // จุด Fix กล้องในห้องบอส


    [SerializeField] private AudioSource bgmSource;  // อ้างถึง BGMusic ที่คุณมีอยู่แล้ว
    [SerializeField] private AudioClip bossMusic;    // เพลงบอส
    private AudioClip previousMusic; // เก็บเพลงที่เล่นก่อนเข้า Boss Room




    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("Lock Active!");
            
            // ✅ เปิดใช้งานกำแพงปิดทาง
            if (wallBlock != null)
            {
                wallBlock.SetActive(true);
                wallBlock2.SetActive(true);
                bossspawn.SetActive(true);
                hp.SetActive(true);
                hpbar.SetActive(true);

                Debug.Log("Camera Fixed in Boss Room");
                cameraFollow.FixCamera(bossRoomCamPos.position);
                bgFollow.FixCamera(bossRoomCamPos.position);

            }
            if (bgmSource != null && bossMusic != null)
            {
                previousMusic = bgmSource.clip; // เก็บเพลงเดิม
                bgmSource.Stop();
                bgmSource.clip = bossMusic;
                bgmSource.loop = true;
                bgmSource.Play();
            }

            // ✅ ปิดตัว Trigger เอง กันจากการเรียกซ้ำ
            gameObject.SetActive(false);
         
        }
    }
}
