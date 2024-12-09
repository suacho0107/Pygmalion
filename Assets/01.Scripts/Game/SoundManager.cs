using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Define;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip mainBGM;
    [SerializeField] private AudioClip companyBGM;
    [SerializeField] private AudioClip globalBGM;
    [SerializeField] private AudioClip museumBGM;

    #region Singleton
    static SoundManager s_instance;
    public static SoundManager Instance
    {
        get
        {
            if (s_instance == null)
            {
                s_instance = FindObjectOfType<SoundManager>();
            }
            return s_instance;
        }
    }
    #endregion

    private AudioSource audioSource;

    private Stage.StageState currentState;

    private void Awake()
    {
        if (s_instance == null)
        {
            s_instance = this;
            DontDestroyOnLoad(gameObject);

            audioSource = GetComponent<AudioSource>();
            audioSource.loop = true;

            // 씬 로드 이벤트 등록
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        // 씬 로드 이벤트 해제
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬 로드 후 BGM 설정
        UpdateStageBGM();
    }

    public void SetStageBGM(Stage.StageState state)
    {
        // 현재 상태와 비교하여 동일하면 중복 설정 방지
        if (currentState == state) return;

        currentState = state;
        UpdateBGMClip();
    }

    private void UpdateStageBGM()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == "Company_Lobby" || sceneName == "Company_Office")
        {
            SetStageBGM(Stage.StageState.Company);
        }
        else if (sceneName == "GlobalMap")
        {
            SetStageBGM(Stage.StageState.Global);
        }
        else if (sceneName == "Museum_Lobby" || sceneName == "Museum_ExhibitionRoom1" || sceneName == "Museum_ExhibitionRoom2" || sceneName == "Museum_Garden" || sceneName == "Museum_ExhibitionRoom3")
        {
            SetStageBGM(Stage.StageState.Museum);
        }
        else
        {
            SetStageBGM(Stage.StageState.None);
        }
    }

    private void UpdateBGMClip()
    {
        AudioClip bgmToPlay = null;

        switch (currentState)
        {
            case Stage.StageState.Company:
                bgmToPlay = companyBGM;
                break;
            case Stage.StageState.Global:
                bgmToPlay = globalBGM;
                break;
            case Stage.StageState.Museum:
                bgmToPlay = museumBGM;
                break;
            default:
                bgmToPlay = mainBGM;
                break;
        }

        if (bgmToPlay != null && audioSource.clip != bgmToPlay)
        {
            audioSource.clip = bgmToPlay;
            audioSource.Play();
        }
    }
}
