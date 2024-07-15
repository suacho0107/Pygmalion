using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator anim;

    Vector3 dirVec;
    GameObject scanObject;

    // Canvas 오브젝트의 InventoryUI 스크립트 가져오기
    public Canvas CanvasObj;
    private InventoryUI invenScript;

    public float moveSpeed;

    float h;
    float v;

    bool isHorizonMove;
    bool activeInteract = false;

    public PlayerState pState;

    public enum PlayerState
    {
        Move,
        Interaction,
        Inventory
    }

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        invenScript = CanvasObj.GetComponent<InventoryUI>();
    }

    void Start()
    {
        pState = PlayerState.Move;
    }

    void Update()
    {
        bool hDown = Input.GetButtonDown("Horizontal");
        bool hUp = Input.GetButtonUp("Horizontal");
        bool vDown = Input.GetButtonDown("Vertical");
        bool vUp = Input.GetButtonUp("Vertical");

        switch (pState)
        {
            case PlayerState.Move:
                pStateMove();
                break;
            case PlayerState.Interaction:
                pStateInteraction();
                break;
            case PlayerState.Inventory:
                pStateInventory();
                break;
        }

        void pStateMove()
        {
            h = Input.GetAxisRaw("Horizontal");
            v = Input.GetAxisRaw("Vertical");

            if (hDown)
                isHorizonMove = true;
            else if (vDown)
                isHorizonMove = false;
            else if (hUp || vUp)
                isHorizonMove = h != 0;

            // Interaction 입력
            if (Input.GetKeyDown(KeyCode.F))
            {
                activeInteract = !activeInteract;
            }

            // Move 상태에서 Inventory, Interaction 상태로 전환
            if (invenScript.activeInventory == true)
            {
                pState = PlayerState.Inventory;
            }
            else if (activeInteract == true)
            {
                pState = PlayerState.Interaction;
            }

            // Ray Direction
            if (vDown && v == 1)
                dirVec = Vector3.up;
            else if (vDown && v == -1)
                dirVec = Vector3.down;
            else if (hDown && h == -1)
                dirVec = Vector3.left;
            else if (hDown && h == 1)
                dirVec = Vector3.right;

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

        void pStateInteraction()
        {
            // 대화창 구현 전까지는 콘솔 출력
            if (hDown)
            {
                Debug.Log("Interaction A");
            }
            else if (hUp)
            {
                Debug.Log("Interaction D");
            }
            else if (vDown)
            {
                Debug.Log("Interaction S");
            }
            else if (vUp)
            {
                Debug.Log("Interaction W");
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                activeInteract = false;
                pState = PlayerState.Move;
            }

            // Interaction 상태에서 Tab 누르면 Inventory 상태로
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                pState = PlayerState.Inventory;
            }
        }

        void pStateInventory()
        {
            // 인벤토리 구현 전까지는 콘솔 출력
            if (hDown)
            {
                Debug.Log("Inventory A");
            }
            else if (hUp)
            {
                Debug.Log("Inventory D");
            }
            else if (vDown)
            {
                Debug.Log("Inventory S");
            }
            else if (vUp)
            {
                Debug.Log("Inventory W");
            }

            if (invenScript.activeInventory == false)
            {
                pState = PlayerState.Move;
            }
        }


        // Scan Object
        if (scanObject != null)
            Debug.Log(scanObject.name);
    }

    void FixedUpdate()
    {
        Vector2 moveVec = isHorizonMove ? new Vector2(h, 0) : new Vector2(0, v);
        rigid.velocity = moveVec * moveSpeed;

        // Ray
        Debug.DrawRay(rigid.position, dirVec * 0.7f, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, dirVec, 0.7f, LayerMask.GetMask("Object"));

        if (rayHit.collider != null)
            scanObject = rayHit.collider.gameObject;
        else
            scanObject = null;
    }
}
