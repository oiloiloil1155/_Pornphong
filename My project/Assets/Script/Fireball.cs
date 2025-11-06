using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    public GameObject explosionEffect;
    
    private void Start()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // ส่งดาเมจ
            Myscript1 player = collision.GetComponent<Myscript1>();
            if (player != null)
            {
                Destroy(gameObject);
            }
        }
        if (collision.CompareTag("Ground"))
        {
            // ส่งดาเมจ
            Myscript1 player = collision.GetComponent<Myscript1>();
            if (player != null)
            {
                Destroy(gameObject);
            }
        }

        // สร้างเอฟเฟกต์ระเบิด
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

      
    }
}
