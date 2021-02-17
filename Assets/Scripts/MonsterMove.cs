using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMove : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer sprite;
    public int nextMove; //행동지표 변수

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        Think();
    }

    private void FixedUpdate()
    {
        //Move
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y); //Vector2.left 가 아닌 -1도 가능

        //Platform Check
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove * 0.5f, rigid.position.y);

        Debug.DrawRay(frontVec, Vector2.down, Color.white);
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector2.down, 2);
        if (rayHit.collider == null)
            Turn();
    }

    //재귀함수
    void Think()
    {
        //MoveThink
        nextMove = Random.Range(-1, 2); //min <= value < max 이므로 최대값에는 +1해줄 것
        float nextThinkTime = Random.Range(2f, 5f);
       
        //Animation
        anim.SetInteger("WalkSpeed", nextMove); //isWalking bool값과는 다른 방식, 더 간단한듯.

        //Flip Sprite
        if(nextMove != 0)
            sprite.flipX = nextMove == 1;

        Invoke("Think", nextThinkTime); //Invoke("", 초) : n초 뒤에 함수를 실행
    }

    void Turn()
    {
        nextMove = nextMove * -1; //nextMove *= -1 도 가능
        sprite.flipX = !sprite.flipX;
        CancelInvoke(); //Invoke 되고 있는 함수 멈춤!
        Invoke("Think", 5);
    }
}
