using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tentacle : MonoBehaviour
{
    Animator anim;
    public Rigidbody2D rb;
    Vector3 drop;
    public int animNum;
    public int hp;
    public bool isHurt;
    public bool isGone;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Spear")
        {
            hp--;

            if (hp <= 0)
            {
                Destroy(rb);
                Destroy(anim);
                isHurt = true;
            }
        }
    }
    
    void Start()
    {
        hp = 5;
        isHurt = false;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        anim.Play("Tentacle" + animNum); // Different animation clips play in same state w/ editor variable animNum
        drop = Vector3.down * 0.07f + Vector3.left * 0.16f;
    }
    
    void Update()
    {
        if(isHurt && !isGone)
        {
            transform.position += drop;
        }

        if(transform.position.y < -100 && !isGone)
        {
            Destroy(gameObject.GetComponent<SpriteRenderer>());
            isGone = true;
        }
    }
}
