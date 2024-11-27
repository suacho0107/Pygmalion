using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueParser : MonoBehaviour
{
    public Dialogue[] Parse(string _CSVFileName)
    {
        List<Dialogue> dialogueList = new List<Dialogue>(); //대화 List 생성
        TextAsset csvData = Resources.Load<TextAsset>(_CSVFileName); //csv파일 TextAsset으로 변환해서 가져옴

        string[] data = csvData.text.Split(new char[] { '\n' }); //'\n' 단위로 쪼갬

        for (int i = 1; i < data.Length - 1;) //변환 과정에서 맨 뒤에 한 줄이 더 들어가는 듯
        {
            string[] row = data[i].Split(new char[] { ',' }); //',' 단위로 쪼갬

            Dialogue dialgoue = new Dialogue(); // 대사 리스트 생성

            dialgoue.name = row[1];

            //List 생성
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

            //} while (row[0].ToString() == ""); //ID가 공백이면 sentence만 추가

            do
            {
                contextList.Add(row.Length > 2 ? row[2] : ""); // 열의 길이를 체크하고 부족하면 빈 문자열 추가
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

            } while (row[0].ToString() == ""); //ID가 공백이면 sentence만 추가


            //List 배열화
            dialgoue.contexts = contextList.ToArray();
            dialgoue.eventNum = eventList.ToArray();
            dialgoue.skipNum = skipList.ToArray();


            dialogueList.Add(dialgoue);
        }

        return dialogueList.ToArray();
    }
}
