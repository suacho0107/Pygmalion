using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NPCAnim : MonoBehaviour
{
    Animator anim;

    public GameObject side;
    public GameObject tremble;

    string sceneName;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        sceneName = SceneManager.GetActiveScene().name;
    }

    void Start()
    {
        if(anim != null)
        {
            side.SetActive(true);
        }
    }

    void Update()
    {
        if (sceneName.StartsWith("Museum_Garden"))
        {
            MuseumGuard museumGuard = GetComponent<MuseumGuard>();
            if (!museumGuard.uncontacted)
            {
                side.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            else // 주인공 알아차리는 대사 전 뒤돌아있기
            {
                side.gameObject.transform.rotation = Quaternion.Euler(0, 180, 0);
            }
        }
        else if (sceneName.StartsWith("Library_2F"))
        {
            StageNPC stageNPC = GetComponent<StageNPC>();
            if (!stageNPC.isInteract)
            {
                side.SetActive(false);
                tremble.SetActive(true);
            }
            
            if (anim.GetBool("melEnd"))
            {
                side.SetActive(true);
                tremble.SetActive(false);
            }
        }
    }
}
