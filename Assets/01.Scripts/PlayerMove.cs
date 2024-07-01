using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator anim;

    public float moveSpeed;

    float h;
    float v;

    bool isHorizonMove;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");

        bool hDown = Input.GetButtonDown("Horizontal");
        bool hUp = Input.GetButtonUp("Horizontal");
        bool vDown = Input.GetButtonDown("Vertical");
        bool vUp = Input.GetButtonUp("Vertical");

        if (hDown)
            isHorizonMove = true;
        else if (vDown)
            isHorizonMove = false;
        else if (hUp || vUp)
            isHorizonMove = h != 0;

        //// 애니메이션
        //if (anim.GetInteger("hAxisRaw") != h)
        //{
        //    anim.SetBool("isChange", true);
        //    anim.SetInteger("hAxisRaw", (int)h);
        //}
        //else if (anim.GetInteger("vAxisRaw") != v)
        //{
        //    anim.SetBool("isChange", true);
        //    anim.SetInteger("vAxisRaw", (int)v);
        //}
        //else
        //    anim.SetBool("isChange", false);
    }

    void FixedUpdate()
    {
        Vector2 moveVec = isHorizonMove ? new Vector2(h, 0) : new Vector2(0, v);
        rigid.velocity = moveVec * moveSpeed;
    }
}
