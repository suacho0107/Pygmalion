using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerObject : MonoBehaviour
{
    [SerializeField] GameObject targetObject;

    StageNPC    object_npc;
    RequestNPC  object_request;

    bool isNext = false;

    public bool Get_IsNext() { return isNext; }

    void Start()
    {
        if (null == targetObject)
            return;

        Check_ObjectType();
    }

    void Update()
    {
        Check_StateChange();
    }

    void Check_ObjectType()
    {
        if (targetObject.TryGetComponent(out StageNPC npc))
        {
            object_npc = npc;
        }
        else if (targetObject.TryGetComponent(out RequestNPC request))
        {
            object_request = request;
        }
    }

    void Check_StateChange()
    {
        if (object_npc && object_npc.isTutoFin)
        {
            isNext = true;
        }
        
        if (object_request && object_request.canOff)
        {
            isNext = true;
        }
    }
}
