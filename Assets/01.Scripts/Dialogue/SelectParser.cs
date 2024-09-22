using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectParser : MonoBehaviour
{
    public Select[] Parse(string _CSVFileName)
    {
        List<Select> selectList = new List<Select>(); //선지 List 생성
        TextAsset csvData = Resources.Load<TextAsset>(_CSVFileName); //csv파일 TextAsset으로 변환해서 가져옴

        if (csvData == null) //null 처리
        {
            return null;
        }

        string[] data = csvData.text.Split(new char[] { '\n' }); //'\n' 단위로 쪼갬

        for (int i = 1; i < data.Length - 1;) //변환 과정에서 맨 뒤에 한 줄이 더 들어가는 듯
        {
            string[] row = data[i].Split(new char[] { ',' }); //',' 단위로 쪼갬

            Select select = new Select();

            //List 생성
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

            } while (row[0].ToString() == ""); //ID가 공백이면 context만 추가

            //List 배열화
            select.contexts = contextList.ToArray();
            select.moveNum = moveList.ToArray();


            selectList.Add(select);
        }
        return selectList.ToArray();
    }
}
