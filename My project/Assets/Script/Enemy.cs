//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//using static UnityEngine.GraphicsBuffer;
//public class Enemy : MonoBehaviour
//{
//    public int health;
//    public float speed;

//    private Animation anim;

//    private void Start()
//    {
//        anim = GetComponent<Animation>();
//        anim.SetBool("Run", true);
//    }
//    private void Update()
//    {
//        transform.Translate(Vector2.left * speed * Time.deltaTime);
//    }
//    public void TakeDamege(int damege)
//    {
//        health -= damege;
//        Debug.Log("demage Taken");
//    }
//}

