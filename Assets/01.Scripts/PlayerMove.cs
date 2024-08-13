using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator frontAnimator;
    Animator backAnimator;
    Animator leftAnimator;
    Animator rightAnimator;

    Vector3 dirVec;

    public GameObject frontAnim;
    public GameObject backAnim;
    public GameObject leftAnim;
    public GameObject rightAnim;

    public float moveSpeed;

    float h;
    float v;

    bool isHorizonMove;
    private bool activeInteract = false;
    public bool activeInven;

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
        frontAnimator = frontAnim.GetComponent<Animator>();
        backAnimator = backAnim.GetComponent<Animator>();
        leftAnimator = leftAnim.GetComponent<Animator>();
        rightAnimator = rightAnim.GetComponent<Animator>();
    }

    void Start()
    {
        pState = PlayerState.Move;        
        frontAnim.SetActive(true);
        rightAnim.SetActive(false);
        backAnim.SetActive(false);
        leftAnim.SetActive(false);
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
            Debug.Log("pState = Move");
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

            // Ray Direction
            if (vDown && v == 1)
            {
                dirVec = Vector3.up;
                backAnim.SetActive(true);
                frontAnim.SetActive(false);
                leftAnim.SetActive(false);
                rightAnim.SetActive(false);
            }                
            else if (vDown && v == -1)
            {
                dirVec = Vector3.down;
                frontAnim.SetActive(true);
                leftAnim.SetActive(false);
                rightAnim.SetActive(false);
                backAnim.SetActive(false);
            }
            else if (hDown && h == -1)
            {
                dirVec = Vector3.left;
                leftAnim.SetActive(true);
                rightAnim.SetActive(false);
                frontAnim.SetActive(false);
                backAnim.SetActive(false);
            }
            else if (hDown && h == 1)
            {
                dirVec = Vector3.right;
                rightAnim.SetActive(true);
                leftAnim.SetActive(false);
                frontAnim.SetActive(false);
                backAnim.SetActive(false);
            }

            UpdateAnimator(frontAnimator);
            UpdateAnimator(backAnimator);
            UpdateAnimator(leftAnimator);
            UpdateAnimator(rightAnimator);
        }

        void UpdateAnimator(Animator anim)
        {
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
            }

            // Interaction 상태에서 Tab 누르면 Inventory 상태로
            //if (Input.GetKeyDown(KeyCode.Tab))
            //{
            //    pState = PlayerState.Inventory;
            //}
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

            if(Input.GetKeyDown(KeyCode.Tab))
            {
                activeInven = false;
                pState = PlayerState.Move;
            }
        }
    }

    void FixedUpdate()
    {
        if(pState == PlayerState.Move)
        {
            Vector2 moveVec = isHorizonMove ? new Vector2(h, 0) : new Vector2(0, v);
            rigid.velocity = moveVec * moveSpeed;
        }
        
        // Ray
        Debug.DrawRay(rigid.position, dirVec * 1f, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, dirVec, 1f, LayerMask.GetMask("InteractObj"));

        if (rayHit.collider != null)
        {
            Debug.Log("F키 활성화");
            if (Input.GetKeyDown(KeyCode.F))
                ActiveInteract = true;
        }
    }
}
