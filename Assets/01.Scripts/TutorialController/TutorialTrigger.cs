using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialTrigger : TutorialBase
{
    [SerializeField]
    private GameObject[] triggerObject;    // 플레이어가 충돌해야 하는 오브젝트

    private PlayerMove playerMove;
    [SerializeField] private LibraryRoom libraryRoom;

    public bool isTrigger { set; get; } = false;

    public override void Enter()
    {
        //Debug.Log("Enter: TutorialTrigger");

        playerMove = FindObjectOfType<PlayerMove>();
    }

    public override void Execute(TutorialController controller)
    {
        foreach (GameObject obj in triggerObject)
        {
            if (obj != null)
            {
                bool triggerActive = true;

                if (libraryRoom != null)
                {
                    if (obj.name == "triggerObj_CR2")
                    {
                        if (!InventoryUI.instance.HasItem(20102)) triggerActive = false;
                        else
                        {
                            if (libraryRoom.unlock) triggerActive = true;
                            else triggerActive = false;
                        }
                    }
                    if (obj.name == "triggerObj_S")
                    {
                        if (!InventoryUI.instance.HasItem(20101)) triggerActive = false;
                        else
                        {
                            if (libraryRoom.unlock) triggerActive = true;
                            else triggerActive = false;
                        }
                    }
                }

                obj.gameObject.SetActive(triggerActive);
            }
        }

        Vector3 vecOffsetY = new Vector3(0, 1, 0);
        transform.position = playerMove.transform.position - vecOffsetY;

        if (isTrigger)
        {
            controller.SetNextTutorial();
        }
    }

    public override void Exit()
    {
        //Debug.Log("Exit: TutorialTrigger");

        foreach (GameObject obj in triggerObject)
        {
            obj.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        // Trigger Obj Collision Condition
        foreach (GameObject obj in triggerObject)
        {
            if (col.transform == obj.transform)
            {
                if (obj.TryGetComponent(out TriggerObject triggerObj))
                {
                    if (triggerObj.Get_IsNext())
                    {
                        col.gameObject.SetActive(false);
                        isTrigger = true;
                    }
                    else
                        break;
                }
                else
                {
                    col.gameObject.SetActive(false);
                    isTrigger = true;
                }

                break;
            }
        }
    }
}
