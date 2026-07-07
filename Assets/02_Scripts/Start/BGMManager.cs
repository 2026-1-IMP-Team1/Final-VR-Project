using UnityEngine;
using UnityEngine.SceneManagement;

public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance;

    [System.Serializable]
    public class SceneBGM
    {
        public string sceneName;
        public AudioClip bgmClip;
    }

    [Header("Scene BGM Settings")]
    [SerializeField] private SceneBGM[] sceneBGMs;

    private AudioSource audioSource;
    private bool isBGMOn = true;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // 2D sound

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        PlayBGMForScene(SceneManager.GetActiveScene().name);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayBGMForScene(scene.name);
    }

    private void PlayBGMForScene(string sceneName)
    {
        AudioClip nextClip = GetBGMClip(sceneName);

        if (nextClip == null)
        {
            return;
        }

        if (audioSource.clip == nextClip)
        {
            audioSource.mute = !isBGMOn;

            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }

            return;
        }

        audioSource.Stop();
        audioSource.clip = nextClip;
        audioSource.mute = !isBGMOn;
        audioSource.Play();
    }

    private AudioClip GetBGMClip(string sceneName)
    {
        foreach (SceneBGM sceneBGM in sceneBGMs)
        {
            if (sceneBGM.sceneName == sceneName)
            {
                return sceneBGM.bgmClip;
            }
        }

        return null;
    }

    public void SetBGM(bool on)
    {
        isBGMOn = on;
        audioSource.mute = !isBGMOn;

        if (isBGMOn && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    public void ToggleBGM()
    {
        SetBGM(!isBGMOn);
    }

    public bool IsBGMOn()
    {
        return isBGMOn;
    }
}