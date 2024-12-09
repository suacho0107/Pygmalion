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

            // �� �ε� �̺�Ʈ ���
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        // �� �ε� �̺�Ʈ ����
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // �� �ε� �� BGM ����
        UpdateStageBGM();
    }

    public void SetStageBGM(Stage.StageState state)
    {
        // ���� ���¿� ���Ͽ� �����ϸ� �ߺ� ���� ����
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
