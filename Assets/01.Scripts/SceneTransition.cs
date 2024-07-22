using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] PlayerPosition playerPos;
    [SerializeField] Vector3 nextPos;
    [SerializeField] string nextScene;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            // Scriptable Objectø° nextPos ¿˙¿Â
            playerPos.nextPosition = nextPos;
            playerPos.isChecked = true;

            SceneManager.LoadScene(nextScene);
        }
    }
}
