using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pyattcke : MonoBehaviour
{
    private float timeBtwAttack;
    public float startTimeBtwAttack;

    public float TmoveX;
    private Animator anim;
    public Transform attackPos;
    public LayerMask whatIsEnemies;
    public float attackRange;
    public int damage;
    private Rigidbody2D rb;
    private void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        
    }
        void Update()
    {
        
        if (timeBtwAttack <= 0)
        {

            if (Input.GetKeyDown(KeyCode.J))
            {
                anim.SetTrigger("Attack");
                rb.velocity = Vector2.zero;
                Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPos.position, attackRange, whatIsEnemies);
                for (int i = 0; i < enemiesToDamage.Length; i++) {
                    enemiesToDamage[i].GetComponent<EnemyCon>().TakeDamage(damage);
                }
                
            }
            timeBtwAttack = startTimeBtwAttack;
        }
        else
        {
            timeBtwAttack -= Time.deltaTime;
        }
    }
    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, attackRange);
    }
}
