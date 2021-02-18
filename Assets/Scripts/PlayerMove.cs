using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public GameManger gameManager;
    public AudioClip audioJump;
    public AudioClip audioAttack;
    public AudioClip audioDamaged;
    public AudioClip audioItem;
    public AudioClip audioDie;
    public AudioClip audioFinish;
    public float MoveSpeed;
    public float MaxSpeed;
    public float JumpPower;
    public float ReactPower;

    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer sprite;
    CapsuleCollider2D collid;
    AudioSource audioSource;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        collid = GetComponent<CapsuleCollider2D>();
        audioSource = GetComponent<AudioSource>();
        anim.SetBool("isJumping", false);

    }

    void PlayeSound(AudioClip audio)
    {
        audioSource.clip = audio;
        audioSource.Play();
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
            PlayeSound(audioJump);
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
            //Attack
            if (rigid.velocity.y < 0 && transform.position.y > collision.transform.position.y)
                OnAttack(collision.transform);
            else //Damaged
                OnDamaged();
        }
    }
    
    void OnAttack(Transform monster)
    {
        //Point
        gameManager.stagePoint += 1;

        //Reaction Force
        rigid.AddForce(Vector2.up * Time.deltaTime * 150, ForceMode2D.Impulse);

        //Monster Die
        MonsterMove monsterMove = monster.GetComponent<MonsterMove>();
        monsterMove.OnDamaged();

        //Sound
        PlayeSound(audioAttack);
    }

    void OnDamaged()
    {
        //Health Down
        gameManager.HealthDown();

        //Change Layer (Immortal Active)
        gameObject.layer = LayerMask.NameToLayer("PlayerDamaged");

        //Change Color
        sprite.color = Color.red;

        //Reaction Force
        float ReactDir = rigid.velocity.normalized.x * -1;
        rigid.AddForce(new Vector2(ReactDir, 1) * Time.deltaTime * ReactPower, ForceMode2D.Impulse);
        
        //Animation
        anim.SetTrigger("doDamaged");

        //Sound
        PlayeSound(audioDamaged);

        Invoke("OffDamaged", 1);
    }

    void OffDamaged()
    {
        gameObject.layer = LayerMask.NameToLayer("Player");
        sprite.color = Color.white;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Item")
        {
            //Point
            if (collision.gameObject.name.Contains("Bronze")) //~.Contains(""); 포함하고 있습니까? bool값 반환
                gameManager.stagePoint += 1;
            else if (collision.gameObject.name.Contains("Silver"))
                gameManager.stagePoint += 2;
            else if (collision.gameObject.name.Contains("Gold"))
                gameManager.stagePoint += 3;

            //Deactive Item
            collision.gameObject.SetActive(false);

            //Sound
            PlayeSound(audioItem);
        }
        else if (collision.tag == "Finish") //Finish
        {
            //Next Stage
            gameManager.NextStage();
        }
    }
    public void OnDie()
    {
        //Sprite Red
        sprite.color = Color.red;

        //Sprite Flip Y
        sprite.flipY = true;

        //Collider Disable
        collid.enabled = false;

        //Die Effect Jump
        rigid.AddForce(Vector2.up * Time.deltaTime * 200, ForceMode2D.Impulse);

        //Sound
        PlayeSound(audioDie);
    }
    public void VelocityZero()
    {
        rigid.velocity = Vector2.zero;
    }
}