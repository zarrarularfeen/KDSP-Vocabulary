using TMPro;
using Unity.Properties;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public static SettingsMenu Instance { get; private set; }

    [Header("Volume Sliders")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("Toggles and Sliders")]
    [SerializeField] private GameObject prsToggle;
    [SerializeField] private GameObject conditionSlider;

    private static string currentPRSKey; //positive reinforcement sound
    private static string currentConditionSliderKey; //require an action

    void Awake()
    {
        // Ensure only one instance exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
        Instance = this;
        InitializePlayerPrefs();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializePlayerPrefs();
        InitializeVolume();
        InitializeVolumeSliders();

        SetupKeysAndInitialize();
    }

    #region Getter Setters

    public static string CurrentPRSKey
    {
        get { return currentPRSKey; }
        set { currentPRSKey = value; }
    }
    public static string CurrentConditionSliderKey
    {
        get { return currentConditionSliderKey; }
        set { currentConditionSliderKey = value; }
    }

    #endregion

    #region Initialization

    public void SetupKeysAndInitialize()
    {
        if (SceneController.currentScene == Scenes.SentencesLevel)
        {
            if (currentPRSKey != "SentencesLevelPRS")
            {
                currentPRSKey = "SentencesLevelPRS";
                SetPRSStaticVariable();
            }

            if (currentConditionSliderKey != "SentencesLevelBS")
            {
                currentConditionSliderKey = "SentencesLevelBS";
                SetConditionSliderStaticVariable();
            }

            InitializePRSToggle();
            InitializeConditionSlider();
        }
    }

    public void InitializePlayerPrefs()
    {
        Debug.Log("InitializePlayerPrefs Called");

        if (!PlayerPrefs.HasKey("MasterVolume"))
        {
            PlayerPrefs.SetFloat("MasterVolume", 0f);
        }

        if (!PlayerPrefs.HasKey("MusicVolume"))
        {
            PlayerPrefs.SetFloat("MusicVolume", 0f);
        }

        if (!PlayerPrefs.HasKey("SFXVolume"))
        {
            PlayerPrefs.SetFloat("SFXVolume", 0f);
        }

        if (!PlayerPrefs.HasKey("SentencesLevelPRS"))
        {
            SetBool("SentencesLevelPRS", true);
        }

        if (!PlayerPrefs.HasKey("SentencesLevelBS"))
        {
            PlayerPrefs.SetInt("SentencesLevelBS", 2);
        }

        PlayerPrefs.Save(); // Save all changes
    }

    public void InitializeVolume()
    {
        Debug.Log("Initialize Volume Called");
        audioMixer.SetFloat("MasterVolume", PlayerPrefs.GetFloat("MasterVolume", 0f));
        audioMixer.SetFloat("MusicVolume", PlayerPrefs.GetFloat("MusicVolume", 0f));
        audioMixer.SetFloat("SFXVolume", PlayerPrefs.GetFloat("SFXVolume", 0f));
    }

    public void InitializeVolumeSliders()
    {
        Debug.Log("Initialize Volume Sliders Called");
        masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 0f);
        musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0f);
        sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0f);

        masterVolumeSlider.onValueChanged.AddListener(AdjustMasterVolume);
        musicVolumeSlider.onValueChanged.AddListener(AdjustMusicVolume);
        sfxVolumeSlider.onValueChanged.AddListener(AdjustSFXVolume);
    }

    public void InitializePRSToggle()
    {
        prsToggle.GetComponent<Toggle>().isOn = GetBool(currentPRSKey);
        prsToggle.GetComponent<Toggle>().onValueChanged.AddListener(PRSToggleValueChanged);
        prsToggle.GetComponentInChildren<TextMeshProUGUI>().text = GiveOnOffText(GetBool(currentPRSKey));
    }

    public void InitializeConditionSlider()
    {
        conditionSlider.GetComponentInChildren<Slider>().value = PlayerPrefs.GetInt(currentConditionSliderKey, 2);
        conditionSlider.GetComponentInChildren<TextMeshProUGUI>().text = PlayerPrefs.GetInt(currentConditionSliderKey, 2).ToString();
        conditionSlider.GetComponentInChildren<Slider>().onValueChanged.AddListener(ConditionSliderValueChanged);
    }

    #endregion

    // Update is called once per frame
    void Update()
    {

    }

    #region Value Change Functions (Listeners)

    public void AdjustMasterVolume(float value)
    {
        audioMixer.SetFloat("MasterVolume", value);
        musicVolumeSlider.value = value;
        sfxVolumeSlider.value = value;
        PlayerPrefs.SetFloat("MasterVolume", value);
    }

    public void AdjustMusicVolume(float value)
    {
        audioMixer.SetFloat("MusicVolume", value);
        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    public void AdjustSFXVolume(float value)
    {
        audioMixer.SetFloat("SFXVolume", value);
        PlayerPrefs.SetFloat("SFXVolume", value);
    }

    public void PRSToggleValueChanged(bool isOn)
    {
        SetBool(currentPRSKey, isOn);
        SetPRSStaticVariable();
        prsToggle.GetComponentInChildren<TextMeshProUGUI>().text = GiveOnOffText(GetBool(currentPRSKey));
    }

    public void ConditionSliderValueChanged(float value)
    {
        PlayerPrefs.SetInt(currentConditionSliderKey, (int)value);
        SetConditionSliderStaticVariable();
        switch (currentConditionSliderKey)
        {
            case "SentencesLevel":
                conditionSlider.GetComponentInChildren<TextMeshProUGUI>().text = PlayerPrefs.GetInt(currentConditionSliderKey, 2).ToString();
                break;

            default:
                Debug.Log("Unknown Condition Slider state");
                break;
        }
    }

    #endregion

    #region Set Static Variables

    public void SetPRSStaticVariable()
    {
        switch (currentPRSKey)
        {
            case "SentencesLevelPRS":
                SentencesLevelManager.PRS = GetBool(currentPRSKey, true);
                break;

            default:
                Debug.Log("Unknown PRS state");
                break;
        }
    }

    public void SetConditionSliderStaticVariable()
    {
        switch (currentConditionSliderKey)
        {
            case "SentencesLevel":
                // SentencesLevelManager.BatchSize = PlayerPrefs.GetInt(currentConditionSliderKey, 2);
                break;

            default:
                Debug.Log("Unknown Condition Slider state");
                break;
        }
    }
    #endregion

    #region  Helper Functions

    public string GiveOnOffText(bool isOn)
    {
        switch (isOn)
        {
            case false:
                return "Off";
            case true:
                return "On";
        }
    }

    public static void SetBool(string key, bool value)
    {
        PlayerPrefs.SetInt(key, value ? 1 : 0);
    }

    public static bool GetBool(string key, bool defaultValue = false)
    {
        return PlayerPrefs.GetInt(key, defaultValue ? 1 : 0) == 1;
    }

    #endregion

}