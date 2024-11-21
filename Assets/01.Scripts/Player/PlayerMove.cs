using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator anim;

    Vector3 dirVec;

    public GameObject frontAnim;
    public GameObject backAnim;
    public GameObject leftAnim;
    private string currentState = "";

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
                //Vector2 rayOg = new Vector2(rigid.position.x, rigid.position.y + 0.7f);
                RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, dirVec, 1f, LayerMask.GetMask("InteractObj"));

                if (rayHit.collider != null)
                {
                    // NPC의 npc 객체 가져오기(방법 다르게)
                    NPC npc = rayHit.collider.GetComponent<NPC>();
                    if (npc != null)
                    {
                        npc.StartDialogue();
                    }
                }
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
        backAnim.SetActive(false);
        leftAnim.SetActive(false);
    }

    void Update() 
    {
        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");

        isHorizonMove = Mathf.Abs(h) > Mathf.Abs(v);

        bool hDown = Input.GetButtonDown("Horizontal");
        bool hUp = Input.GetButtonUp("Horizontal");
        bool vDown = Input.GetButtonDown("Vertical");
        bool vUp = Input.GetButtonUp("Vertical");

        AnimationState();

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
            {
                anim.SetBool("isChange", false);
            }

            // Ray Direction
            if (vDown && v == 1)
                {
                    dirVec = Vector3.up;
                }
            else if (vDown && v == -1)
                {
                    dirVec = Vector3.down;
                }
            else if (hDown && isHorizonMove)
                {
                    dirVec = (h == -1) ? Vector3.left : Vector3.right;
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

            if(Input.GetKeyDown(KeyCode.Tab))
            {
                activeInven = false;
                pState = PlayerState.Move;
            }
        }

        Debug.DrawRay(rigid.position, dirVec * 1f, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, dirVec, 1f, LayerMask.GetMask("InteractObj"));

        if (rayHit.collider != null)
        {
            //Debug.Log("F키 활성화");
            //FKeyDown();
            if (Input.GetKeyDown(KeyCode.F) && !keyDown)
            {
                keyDown = true;
                ActiveInteract = true;
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
        //Vector2 rayOg = new Vector2(rigid.position.x, rigid.position.y + 0.7f);
        //Debug.DrawRay(rayOg, dirVec * 1f, new Color(0, 1, 0));
        //RaycastHit2D rayHit = Physics2D.Raycast(rayOg, dirVec, 1f, LayerMask.GetMask("InteractObj"));
        
        // Update로 이동
        //Debug.DrawRay(rigid.position, dirVec * 1f, new Color(0, 1, 0));
        //RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, dirVec, 1f, LayerMask.GetMask("InteractObj"));
        //Debug.Log("대상 오브젝트: "+ rayHit.collider.gameObject.name);

        // Update로 이동
        //if (rayHit.collider != null)
        //{
        //    //Debug.Log("F키 활성화");
        //    //FKeyDown();
        //    if (Input.GetKeyDown(KeyCode.F) && !keyDown)
        //    {
        //        keyDown = true;
        //        ActiveInteract = true;
        //    }
        //}
    }

    //public void FKeyDown()
    //{
    //    if (Input.GetKeyDown(KeyCode.F) && !keyDown)
    //    {
    //        keyDown = true;
    //        ActiveInteract = true;
    //    }
    //}

    void AnimationState()
    {
        AnimatorStateInfo aState = anim.GetCurrentAnimatorStateInfo(0);
        string newState = "";

        if (aState.IsName("front walk") || aState.IsName("front idle"))
            newState = "front";
        else if (aState.IsName("back walk") || aState.IsName("back idle"))
            newState = "back";
        else if (aState.IsName("left walk") || aState.IsName("left idle"))
            newState = "left";
        else if (aState.IsName("right walk") || aState.IsName("right idle"))
            newState = "right";

        if(newState != currentState)
        {
            currentState = newState;
            UpdatePrefab(newState);
        }
    }

    void UpdatePrefab(string state)
    {
        switch(state)
        {
            case "front":
                ActivePrefab(frontAnim);
                break;
            case "back":
                ActivePrefab(backAnim);
                break;
            case "left":
                ActivePrefab(leftAnim);
                leftAnim.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                break;
            case "right":
                ActivePrefab(leftAnim);
                leftAnim.transform.localScale = new Vector3(-0.4f, 0.4f, 0.4f);
                break;
        }
    }

    void ActivePrefab(GameObject activePrefab)
    {
        frontAnim.SetActive(false);
        backAnim.SetActive(false);
        leftAnim.SetActive(false);

        activePrefab.SetActive(true);
    }
}
