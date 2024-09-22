using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectParser : MonoBehaviour
{
    public Select[] Parse(string _CSVFileName)
    {
        List<Select> selectList = new List<Select>(); //���� List ����
        TextAsset csvData = Resources.Load<TextAsset>(_CSVFileName); //csv���� TextAsset���� ��ȯ�ؼ� ������

        if (csvData == null) //null ó��
        {
            return null;
        }

        string[] data = csvData.text.Split(new char[] { '\n' }); //'\n' ������ �ɰ�

        for (int i = 1; i < data.Length - 1;) //��ȯ �������� �� �ڿ� �� ���� �� ���� ��
        {
            string[] row = data[i].Split(new char[] { ',' }); //',' ������ �ɰ�

            Select select = new Select();

            //List ����
            List<string> contextList = new List<string>();
            List<string> moveList = new List<string>();

            do
            {
                contextList.Add(row[1]);
                moveList.Add(row[2]);

                if (++i < data.Length - 1)
                {
                    row = data[i].Split(new char[] { ',' });
                }
                else
                {
                    break;
                }

            } while (row[0].ToString() == ""); //ID�� �����̸� context�� �߰�

            //List �迭ȭ
            select.contexts = contextList.ToArray();
            select.moveNum = moveList.ToArray();


            selectList.Add(select);
        }
        return selectList.ToArray();
    }
}
