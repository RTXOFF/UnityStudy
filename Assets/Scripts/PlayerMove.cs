﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer sprite;

    public float MoveSpeed;
    public float MaxSpeed;
    public float JumpPower;
    public float ReactPower;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        anim.SetBool("isJumping", false);
    }
    private void Update()
    {
        //StopSpeed
        if (Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
            anim.SetBool("isWalking", false);
        }

        //Jump
        if (Input.GetButtonDown("Jump") && !anim.GetBool("isJumping"))
        {
            rigid.AddForce(Vector2.up * Time.deltaTime * JumpPower, ForceMode2D.Impulse);
            anim.SetBool("isJumping", true);
        }

        //Flipx
        if (Input.GetButtonDown("Horizontal"))
            sprite.flipX = Input.GetAxisRaw("Horizontal") == -1;
    }
    private void FixedUpdate()
    {
        //MoveSpeed
        if (Input.GetButton("Horizontal"))
        {
            float v = Input.GetAxisRaw("Horizontal");
            rigid.AddForce(Vector2.right * v * Time.deltaTime * MoveSpeed, ForceMode2D.Impulse);
            anim.SetBool("isWalking", true);
        }

        //MaxSpeed
        if (Mathf.Abs(rigid.velocity.x) > MaxSpeed)
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * MaxSpeed, rigid.velocity.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //isJump
        if (collision.gameObject.tag == "Platform")
            anim.SetBool("isJumping", false);

        //Damage
        if (collision.gameObject.tag == "Enemy")
        {
            OnDamaged();
        }
    }
    
    void OnDamaged()
    {
        //Change Layer (Immortal Active)
        gameObject.layer = LayerMask.NameToLayer("PlayerDamaged");

        //Change Color
        sprite.color = Color.red;

        //Reaction Force
        float ReactDir = rigid.velocity.normalized.x * -1;
        rigid.AddForce(new Vector2(ReactDir, 1) * Time.deltaTime * ReactPower, ForceMode2D.Impulse);
        
        //Animation
        anim.SetTrigger("doDamaged");

        Invoke("OffDamaged", 1);
    }

    void OffDamaged()
    {
        gameObject.layer = LayerMask.NameToLayer("Player");
        sprite.color = Color.white;
    }
}