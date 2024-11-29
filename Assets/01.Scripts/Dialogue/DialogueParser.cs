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
            List<string> eventList = new List<string>();
            List<string> skipList = new List<string>();

            //do
            //{
            //    contextList.Add(row[2]);
            //    eventList.Add(row[3]);
            //    skipList.Add(row[4]);


            //    if (++i < data.Length - 1)
            //    {
            //        row = data[i].Split(new char[] { ',' });
            //    }
            //    else
            //    {
            //        break;
            //    }

            //} while (row[0].ToString() == ""); //ID�� �����̸� sentence�� �߰�

            do
            {
                contextList.Add(row.Length > 2 ? row[2] : ""); // ���� ���̸� üũ�ϰ� �����ϸ� �� ���ڿ� �߰�
                eventList.Add(row.Length > 3 ? row[3] : "");
                skipList.Add(row.Length > 4 ? row[4] : "");

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
            dialgoue.eventNum = eventList.ToArray();
            dialgoue.skipNum = skipList.ToArray();


            dialogueList.Add(dialgoue);
        }

        return dialogueList.ToArray();
    }
}
