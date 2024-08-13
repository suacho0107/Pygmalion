using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueParser : MonoBehaviour
{
    public Dialogue[] Parse(string _CSVFileName)
    {
        List<Dialogue> dialogueList = new List<Dialogue>(); //��ȭ List ����
        TextAsset csvData = Resources.Load<TextAsset>(_CSVFileName); //csv���� TextAsset���� ��ȯ�ؼ� ������

        string[] data = csvData.text.Split(new char[] { '\n' }); //'\n' ������ �ɰ�

        for (int i = 1; i < data.Length - 1;) //��ȯ �������� �� �ڿ� �� ���� �� ���� ��
        {
            string[] row = data[i].Split(new char[] { ',' }); //',' ������ �ɰ�

            Dialogue dialgoue = new Dialogue(); // ��� ����Ʈ ����

            dialgoue.name = row[1];

            //List ����
            List<string> contextList = new List<string>();

            do
            {
                contextList.Add(row[2]);

                if (++i < data.Length - 1)
                {
                    row = data[i].Split(new char[] { ',' });
                }
                else
                {
                    break;
                }

            } while (row[0].ToString() == ""); //ID�� �����̸� sentence�� �߰�

            //List �迭ȭ
            dialgoue.contexts = contextList.ToArray();

            dialogueList.Add(dialgoue);
        }

        return dialogueList.ToArray();
    }
}
