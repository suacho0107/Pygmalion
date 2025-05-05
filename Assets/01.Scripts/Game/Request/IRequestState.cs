using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRequestState
{
    void Enter(RequestNPC request);
    void Update();
    void Exit();
}
