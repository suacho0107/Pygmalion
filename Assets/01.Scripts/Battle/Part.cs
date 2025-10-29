using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Part : MonoBehaviour
{
    public int partHp;
    public int partMaxHp;
    public int partSort;

    public void SetPartHp()
    {
        partHp = partMaxHp; //Hp 초기화; 재진입 구현 시 수정
    }
}
