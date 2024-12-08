using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DeleteAllData : MonoBehaviour
{
    public void DeleteAllJsonFiles()
    {
        PlayerPrefs.DeleteAll();

        string folderPath = Application.persistentDataPath;

        string[] jsonFiles = Directory.GetFiles(folderPath, "*.json");

        foreach (string file in jsonFiles)
        {
            try
            {
                File.Delete(file);
                Debug.Log($"삭제됨: {file}");
            }
            catch (IOException e)
            {
                Debug.LogError($"파일 삭제 중 오류 발생: {file} - {e.Message}");
            }
        }

        if (jsonFiles.Length > 0)
        {
            Debug.Log($"총 {jsonFiles.Length}개의 JSON 파일을 삭제했습니다.");
        }
        else
        {
            Debug.Log("삭제할 JSON 파일이 없습니다.");
        }
    }
}
