using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage = 5;        // ความเสียหายของกระสุน
    public float lifeTime = 3f;   // เวลากระสุนอยู่ในเกม

    private void Start()
    {
       
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ตรวจสอบว่าชนศัตรู
        if (collision.CompareTag("Enemy"))
        {
            EnemyCon enemy = collision.GetComponent<EnemyCon>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            // พยายามหา Bat
            Bat bat = collision.GetComponent<Bat>();
            if (bat != null)
            {
                bat.TakeDamage(damage);
            }

            Bossbat bossbat = collision.GetComponent<Bossbat>();
            if (bossbat != null)
            {
                bossbat.TakeDamage(damage);
            }
            EnemyArcher enemyarcher = collision.GetComponent<EnemyArcher>();
            if (enemyarcher != null)
            {
                enemyarcher.TakeDamage(damage);
            }

            BossAssassin bossAssassin = collision.GetComponent<BossAssassin>();
            if (bossAssassin != null)
            {
                bossAssassin.TakeDamage(damage);
            }
            BossBigG bossBigG = collision.GetComponent<BossBigG>();
            if (bossBigG != null)
            {
                bossBigG.TakeDamage(damage);
            }
            BossSamurai bossSamuari = collision.GetComponent<BossSamurai>();
            if (bossSamuari != null)
            {
                bossSamuari.TakeDamage(damage);
            }
            EnemyMagic magicEnemy = collision.GetComponent<EnemyMagic>();
            if (magicEnemy != null)
            {
                magicEnemy.TakeDamage(damage);
            }
            BossDarkOne bossDarkone = collision.GetComponent<BossDarkOne>();
            if (bossDarkone != null)
            {
                bossDarkone.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
        // ชนกับกำแพงหรือสิ่งกีดขวาง
        else if (collision.CompareTag("Ground") || collision.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}