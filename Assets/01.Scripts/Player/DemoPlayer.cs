using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoPlayer : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator anim;

    Vector3 dirVec;

    public GameObject leftAnim;
    public GameObject frontAnim;
    public GameObject backAnim;

    public float moveSpeed;

    float h;
    float v;

    bool isHorizonMove;
    private bool activeInteract = false;
    public bool activeInven;
    bool keyDown = false; // 키 중복 입력 방지

    public bool ActiveInteract
    {
        get { return activeInteract; }
        set
        {
            activeInteract = value;
            if (activeInteract == true)
            {
                NPC npc = FindObjectOfType<NPC>();
                npc.StartDialogue();
            }
        }
    }

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
    }

    void Start()
    {
        pState = PlayerState.Move;
        frontAnim.SetActive(true);
        leftAnim.SetActive(false);
        backAnim.SetActive(false);
    }

    void Update()
    {
        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");

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
            // Debug.Log("pState = Move");
            if (hDown)
                isHorizonMove = true;
            else if (vDown)
                isHorizonMove = false;
            else if (hUp || vUp)
                isHorizonMove = h != 0;

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                activeInven = true;
                pState = PlayerState.Inventory;
            }

            if (activeInteract == true)
            {
                pState = PlayerState.Interaction;
            }

            if (anim.GetInteger("hAxisRaw") != h)
            {
                anim.SetBool("isChange", true);
                anim.SetInteger("hAxisRaw", (int)h);
            }
            else if (anim.GetInteger("vAxisRaw") != v)
            {
                anim.SetBool("isChange", true);
                anim.SetInteger("vAxisRaw", (int)v);
            }
            else
                anim.SetBool("isChange", false);

            // Ray Direction
            if (vDown && v == 1)
            {
                dirVec = Vector3.up;
                backAnim.SetActive(true);
                leftAnim.SetActive(false);
                frontAnim.SetActive(false);
            }
            else if (vDown && v == -1)
            {
                dirVec = Vector3.down;
                frontAnim.SetActive(true);
                leftAnim.SetActive(false);
                backAnim.SetActive(false);
            }
            else if (hDown)
            {
                dirVec = (h == -1) ? Vector3.left : Vector3.right;

                // 왼쪽 방향일 때
                if (h == -1)
                {
                    leftAnim.SetActive(true);
                    frontAnim.SetActive(false);
                    backAnim.SetActive(false);
                    leftAnim.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f); // 왼쪽을 향하도록 스케일 조정
                }
                // 오른쪽 방향일 때
                else if (h == 1)
                {
                    leftAnim.SetActive(true);
                    frontAnim.SetActive(false);
                    backAnim.SetActive(false);
                    leftAnim.transform.localScale = new Vector3(-0.4f, 0.4f, 0.4f); // 오른쪽을 향하도록 스케일 반전
                }

            }
        }

        void pStateInteraction()
        {
            Debug.Log("pState = Interaction");
            if (hDown && h == -1)
            {
                Debug.Log("Interaction A");
            }
            else if (hDown && h == 1)
            {
                Debug.Log("Interaction D");
            }
            else if (vDown && v == -1)
            {
                Debug.Log("Interaction S");
            }
            else if (vDown && v == 1)
            {
                Debug.Log("Interaction W");
            }

            if (activeInteract == false)
            {
                pState = PlayerState.Move;
                keyDown = false;
            }
        }

        void pStateInventory()
        {
            Debug.Log("pState = Inventory");
            if (hDown && h == -1)
            {
                Debug.Log("Inventory A");
            }
            else if (hDown && h == 1)
            {
                Debug.Log("Inventory D");
            }
            else if (vDown && v == -1)
            {
                Debug.Log("Inventory S");
            }
            else if (vDown && v == 1)
            {
                Debug.Log("Inventory W");
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                activeInven = false;
                pState = PlayerState.Move;
            }
        }
    }

    void FixedUpdate()
    {
        if (pState == PlayerState.Move)
        {
            Vector2 moveVec = isHorizonMove ? new Vector2(h, 0) : new Vector2(0, v);
            rigid.velocity = moveVec * moveSpeed;
        }

        // Ray
        Debug.DrawRay(rigid.position, dirVec * 1f, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, dirVec, 1f, LayerMask.GetMask("InteractObj"));

        if (rayHit.collider != null)
        {
            if (Input.GetKeyDown(KeyCode.F) && !keyDown)
            {
                keyDown = true;
                ActiveInteract = true;
            }
        }
    }
}
