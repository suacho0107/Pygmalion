using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Part : MonoBehaviour
{
    public int partHp;
    public int partMaxHp;
    public int partSort;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPartHp()
    {
        partHp = partMaxHp; // Hp �ʱ�ȭ; ������ ���� �� ����
    }
}
