using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class UI01_Ready : UIBase
{
    [SerializeField] GameObject UI01_loacation;

    public override void Enter()
    {
        base.Enter();

        UI01_loacation.SetActive(true);
    }

    public override void Exit()
    {
        UI01_loacation.SetActive(false);

        base.Exit();
    }
}
