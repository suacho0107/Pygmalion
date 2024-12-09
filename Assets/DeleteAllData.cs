using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class DeleteAllData : MonoBehaviour
{
    private void Update()
    {
        //if (SceneManager.GetActiveScene().name == "GlobalMap" && Input.GetKeyDown(KeyCode.P))
        //{
        //    Debug.Log("����");
        //    DeleteAllJsonFiles();
        //}
    }

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
                Debug.Log($"������: {file}");
            }
            catch (IOException e)
            {
                Debug.LogError($"���� ���� �� ���� �߻�: {file} - {e.Message}");
            }
        }

        if (jsonFiles.Length > 0)
        {
            Debug.Log($"�� {jsonFiles.Length}���� JSON ������ �����߽��ϴ�.");
        }
        else
        {
            Debug.Log("������ JSON ������ �����ϴ�.");
        }
    }
}
