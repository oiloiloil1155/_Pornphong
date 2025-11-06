using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using static UnityEngine.GraphicsBuffer;

public class Myscript : MonoBehaviour
{
    public float moveX;
    private Rigidbody2D rb;
    public float speed;
    public float jumpForce;
    public bool isJumping = false;
    private Animator anim;
    private bool ground;
    public int healthbar = 10;
    public Text HP;
    public Slider sliderHP;
    [SerializeField] private bool isRight = true;
    [SerializeField] private float slideSpeed;
    [SerializeField] private bool isSlide = false;
 
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sliderHP.maxValue = healthbar;
        sliderHP.value = healthbar;
    }
    private void Update()
    {
        HP.text = "HP: " + healthbar;
        if (healthbar <= 0) {
            healthbar = 0;
            anim.SetTrigger("Dead");
        }
        sliderHP.value = healthbar;


        moveX = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveX * speed, rb.velocity.y);
        if (Input.GetButtonDown("Jump") && !isJumping && ground)
        {
            Jump();
        }

        if (moveX > 0.01f)
        {
            isRight = true;
            transform.localScale = Vector3.one;
        }
        else if (moveX < -0.01f)
        {
            isRight = false;
            transform.localScale = new Vector3(-1, 1, 1);
        }
        anim.SetBool("run", moveX != 0);
        anim.SetBool("ground", ground);
        if (Input.GetMouseButtonDown(0) && ground){
            anim.SetTrigger("Attack");
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            Slide();
        }
        
    }
    private void Jump()
    {
        isJumping = true;
        rb.AddForce(new Vector2(rb.velocity.x, jumpForce));
        anim.SetTrigger("jump");
        ground = false;
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            ground = true;
            isJumping = false;
        }
       
    }

    public void Slide()
    {
        isSlide = true;
        anim.SetBool("Slide", true);
        if (isRight && !isJumping)
        {
            rb.AddForce(Vector2.right * slideSpeed);
        }
        else if (!isRight && !isJumping)
        {
            rb.AddForce(Vector2.left * slideSpeed);
        }
        StartCoroutine(StopSlide());
    }
   
    IEnumerator StopSlide()
    {
        yield return new WaitForSeconds(0.5f);
        anim.SetBool("Slide",false);
        isSlide = false;
    }
  
    void OnTriggerEnter2D(Collider2D other)
    {
       if (other.gameObject.tag == "Enemy")
        {
            healthbar = healthbar - 1;
        }
        if (other.gameObject.tag == "Trap")
        {
            healthbar = 0;
        }
        if (other.gameObject.tag == "Ground")
        {
            isJumping = false;
        }
    }
    
}
