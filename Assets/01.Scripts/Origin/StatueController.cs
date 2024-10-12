using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StatueController : MonoBehaviour
{
    public float moveSpeed = 4f;
    public float findDistance = 8f;
    public float moveDistance = 3f;
    public float minMoveDistance = 2f;

    public float moveTime = 4f;
    float timer;

    Vector2 originPos;
    Vector2 moveRange;
    Vector2 randPos;

    // 조각상 hp 변수
    // float statueHP;

    Rigidbody2D rigid;

    Transform player;

    StatueState sState;

    enum StatueState
    {
        Idle,
        Move,
        Chase,
        Destroyed
    }      
        
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }
    void Start()
    {
        sState = StatueState.Idle;
        originPos = transform.position;
        player = GameObject.Find("Player").transform;        
    }

    void Update()
    {
        switch (sState)
        {
            case StatueState.Idle:
                sStateIdle();
                break;
            case StatueState.Move:
                sStateMove();
                break;
            case StatueState.Chase:
                sStateChase();
                break;
            case StatueState.Destroyed:
                sStateDestroyed();
                break;
        }

        if(sState == StatueState.Move)
        {
            timer += Time.deltaTime;
            if (timer > moveTime)
            {
                RandomPosition();
                timer = 0;
            }
        }        
    }

    void RandomPosition()
    {
        moveRange.x = Random.Range(originPos.x - moveDistance, originPos.x + moveDistance);
        moveRange.y = Random.Range(originPos.y - moveDistance, originPos.y + moveDistance);

        randPos = new Vector2(moveRange.x, moveRange.y);
    }

    void sStateIdle()
    {
        rigid.isKinematic = true;
        // 판별 후 전투 씬으로 이동 - 이 스크립트에서?
        //if(StatueState == enemey)
        //{
        //    SceneManager.LoadScene("Fight");
        //}

        // 전투 종료 후 Destroyed 상태로 - 조각상 hp로 제어?
        //if(statueHP <= 0)
        //    sState = StatueState.Destroyed;
        //

        //if (페이즈2)
        //{
        //    // 페이즈2 기본 상태 Move
        //    sState = StatueState.Move;

        //    // Idle 상태에서 플레이어 발견 시 Chase 상태로, 멀어지면 Move 상태로
        //    // 씬 이동하자마자 Chase될 경우 --> 난이도 조정 필요
        //    if (Vector2.Distance(transform.position, player.position) < findDistace)
        //    {
        //        sState = StatueState.Chase;
        //    }
        //    else
        //        sState = StatueState.Move;
        //}
    }

    void sStateMove()
    {
        rigid.isKinematic = true;
        Vector2 dirVec = randPos - rigid.position;

        if (Mathf.Abs(dirVec.x) > Mathf.Abs(dirVec.y))
        {
            dirVec = new Vector2(dirVec.x, 0);
        }
        else
        {
            dirVec = new Vector2(0, dirVec.y);
        }

        Vector2 nextVec = dirVec.normalized * moveSpeed * Time.deltaTime;
        rigid.MovePosition(rigid.position + nextVec);
        Debug.Log("random move");

        // Move 상태에서 플레이어 발견 시 Chase 상태로, 멀어지면 다시 Move 상태
        if (Vector2.Distance(transform.position, player.position) < findDistance)
        {
            sState = StatueState.Chase;
        }
    }

    void sStateChase()
    {
        rigid.isKinematic = true;
        Vector2 dirVec = player.position - transform.position;

        if (Mathf.Abs(dirVec.x) > Mathf.Abs(dirVec.y))
        {
            dirVec = new Vector2(dirVec.x, 0);
        }
        else
        {
            dirVec = new Vector2(0, dirVec.y);
        }

        Vector2 nextVec = dirVec.normalized * moveSpeed * Time.deltaTime;
        rigid.MovePosition(rigid.position + nextVec);
        Debug.Log("Chase");

        // 원위치로 이동
        if(Vector2.Distance(transform.position, player.position) > findDistance)
        {
            Vector2 ogDirVec = originPos - rigid.position;

            if (Mathf.Abs(ogDirVec.x) > Mathf.Abs(ogDirVec.y))
            {
                ogDirVec = new Vector2(ogDirVec.x, 0);
            }
            else
            {
                ogDirVec = new Vector2(0, ogDirVec.y);
            }

            rigid.MovePosition(rigid.position + ogDirVec);
            sState = StatueState.Move;
        }            
        
        rigid.velocity = Vector2.zero;

        // 충돌 시 전투 씬으로 전환(플레이어 후공)
    }

    void sStateDestroyed()
    {
        rigid.isKinematic = true;
        // 전투 종료(전투 씬 --> 맵 씬 전환) 후
        // 움직임 제거
        // 파괴된 조각상 일러스트
    }
}
