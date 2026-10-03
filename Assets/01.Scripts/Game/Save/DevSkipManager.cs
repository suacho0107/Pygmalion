using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Automatically available in the Editor and Development Builds only.
public class DevSkipManager : MonoBehaviour
{
#if UNITY_EDITOR || DEVELOPMENT_BUILD
    private bool loading;
    private int checkpoint;
    private string targetScene;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Initialize()
    {
        var host = new GameObject("DevSkipManager (1: Museum End, 2: Company Day2, 3: Library Start)");
        DontDestroyOnLoad(host);
        host.AddComponent<DevSkipManager>();
    }

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void Update()
    {
        if (loading) return;
        if (Input.GetKeyDown(KeyCode.Alpha1)) SelectCheckpoint(1);
        else if (Input.GetKeyDown(KeyCode.Alpha2)) SelectCheckpoint(2);
        else if (Input.GetKeyDown(KeyCode.Alpha3)) SelectCheckpoint(3);
    }

    private void SelectCheckpoint(int value)
    {
        if (SaveManager.s_instance == null || UIManager.u_instance == null)
        {
            Debug.LogWarning("[DevSkip] Start the game first; SaveManager and UIManager are required.");
            return;
        }

        checkpoint = value;
        targetScene = value == 1 ? "Museum_Lobby" :
            value == 2 ? "Company_LobbyTuto-2" : "Library_1F";
        if (!Application.CanStreamedLevelBeLoaded(targetScene))
        {
            Debug.LogError("[DevSkip] Scene is not enabled in Build Settings: " + targetScene);
            return;
        }
        loading = true;
        Vector3 spawn = value == 1 ? new Vector3(-0.1f, -2f, 0f) :
            value == 2 ? new Vector3(3.1f, -2.2f, 0f) : new Vector3(-0.1f, -4.5f, 0f);
        SaveManager.s_instance.BeginDevelopmentCheckpoint(spawn);
        StartCoroutine(LoadCheckpoint());
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!loading || scene.name != targetScene) return;
        // These presets bypass arrival tutorials, before their Start methods run.
        foreach (var controller in FindObjectsOfType<TutorialController>(true))
            controller.enabled = false;
        ApplyNPCState(); // Doors inspect the guard's state in Start.
    }

    private IEnumerator LoadCheckpoint()
    {
        var operation = SceneManager.LoadSceneAsync(targetScene);
        if (operation == null)
        {
            loading = false;
            yield break;
        }
        yield return operation;
        yield return null; // Let scene Start methods finish before applying the preset.

        foreach (var fade in FindObjectsOfType<FadeEffect>())
            fade.FadeIn(() => { });

        int count = checkpoint == 1 ? 6 : 0;
        foreach (var score in FindObjectsOfType<StatueScore>(true))
        {
            score.statueCount = count;
            score.checkedCount = count;
            score.destroyedCount = 0;
            score.fightCount = 0;
            score.checkCount = 0;
            score.SaveScore();
        }
        // The company scene may not contain a StatueScore.
        PlayerPrefs.SetInt("StatueCount", count);
        PlayerPrefs.SetInt("checkedCount", count);
        PlayerPrefs.SetInt("destroyedCount", 0);
        PlayerPrefs.SetInt("fightCount", 0);
        PlayerPrefs.SetInt("checkCount", 0);
        PlayerPrefs.Save();

        ApplyNPCState();

        var ui = UIManager.u_instance;
        ui.stageIndex = checkpoint == 1 ? 0 : 1;
        ui.isRespawn = false;
        ui.stateWork = false;
        ui.Set_StageState(checkpoint == 1 ? Define.Stage.StageState.Museum :
            checkpoint == 2 ? Define.Stage.StageState.Company : Define.Stage.StageState.Library);
        ui.Set_UIState(checkpoint == 1 ? Define.UI.UIState.End :
            checkpoint == 2 ? Define.UI.UIState.Ready : Define.UI.UIState.Start);

        foreach (var player in FindObjectsOfType<PlayerMove>())
        {
            player.IsMoved = true;
            player.IsAnimation = true;
            player.ActiveInteract = false;
        }
        SaveManager.s_instance.SaveData();
        loading = false;
        Debug.Log("[DevSkip] Checkpoint " + checkpoint + ": " + targetScene +
            " (saved to DevSaveData.json). Score preferences now belong to this test run.");
    }

    private void ApplyNPCState()
    {
        foreach (var npc in FindObjectsOfType<StageNPC>(true))
        {
            if (checkpoint == 1 && npc.tutorial)
            {
                npc.isInteract = true;
                npc.isTutoDialogueChanged = true;
                npc.isTutoFin = true;
                npc.ChangeDialogueFileName("Museum-Lobby_Check3_dialogue");
                npc.selectFileName = "";
            }
            else if (checkpoint == 3)
            {
                npc.isInteract = false;
                npc.isTutoDialogueChanged = false;
                npc.isTutoFin = false;
                npc.questStart = false;
                npc.questEnd = false;
                npc.currentIndex = 0;
                npc.ChangeDialogueFileName("Guard1_dialogue");
                npc.selectFileName = "Guard1_select";
            }
        }

    }
#endif
}
