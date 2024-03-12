using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D coll;
    private SpriteRenderer sprite;
    private Animator anim;

    [SerializeField] private LayerMask jumpableBackground;
    
    private float dirX = 0f;
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float jumpForce = 14f;

    private enum MovementState { idle, run, jump, die }


    // Start is called before the first frame update
    private void Start()
    {
        //Debug.Log("Hello World");
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {
        dirX = Input.GetAxisRaw("Horizontal");   //direction   GetAxis: smooths - GetAxisRaw:raw
        rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y);    //y: thành phần ngang của vận tốc

        if (Input.GetButtonDown("Jump") && IsGrounded()){
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
        UdpateAnimationUpdate();
    }

    private void UdpateAnimationUpdate()
    {
        MovementState state;
        if (dirX > 0f) //right
        {
            //anim.SetBool("running", true);
            state = MovementState.run;
            sprite.flipX = false;
        }
        else if (dirX < 0f) //left
        {
            state = MovementState.run;
            sprite.flipX = true;
        }
        else
        {
            state = MovementState.idle;
        }

        if(rb.velocity.y > .1f)
        {
            state = MovementState.jump;
        }
        
        anim.SetInteger("state",(int)state);
    }

    private bool IsGrounded()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, 1f,jumpableBackground);
    }
}
