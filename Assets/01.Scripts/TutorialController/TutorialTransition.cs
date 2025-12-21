using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialTransition : TutorialBase
{
    // to be deleted
    [SerializeField]
    private PlayerPosition playerPos;
    [SerializeField]
    private Vector3 nextPos;

    [SerializeField]
    private SceneData sceneData;
    [SerializeField]
    private string nextScene;

    public override void Enter()
    {
        
    }

    public override void Execute(TutorialController controller)
    {
        playerPos.nextPosition = nextPos;
        playerPos.isChecked = true;

        SetUIStateWork(nextScene);

        SceneManager.LoadScene(nextScene);
    }

    private void SetUIStateWork(string _nextScene)
    {
        if (_nextScene == "Museum_Lobby" || _nextScene == "Library_1F" || _nextScene == "Park" ||
            _nextScene == "CityHall_Lobby" || _nextScene == "Broadcast_1F" || _nextScene == "Hospital_1F")
        {
            UIManager.u_instance.Set_UIState(Define.UI.UIState.Work);
        }
    }

    public override void Exit()
    {
        
    }
}
