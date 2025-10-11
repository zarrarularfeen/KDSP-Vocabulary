// using TMPro;
// using Unity.Properties;
// using UnityEngine;
// using UnityEngine.Audio;
// using UnityEngine.UI;

// public class SettingsMenu : MonoBehaviour
// {
//     public static SettingsMenu Instance { get; private set; }
//     [Header("Volume Sliders")]
//     [SerializeField] private Slider masterVolumeSlider;
//     [SerializeField] private Slider musicVolumeSlider;
//     [SerializeField] private Slider sfxVolumeSlider;
//     [Header("Audio Mixer")]
//     [SerializeField] private AudioMixer audioMixer;
//     [Header("Repetition Slider")]
//     [SerializeField] private GameObject repetitionSlider;
//     [Header("Toggles")]
//     [SerializeField] private GameObject prsToggle;
//     [SerializeField] private GameObject nsToggle;
//     [SerializeField] private GameObject orderToggle;
//     [SerializeField] private GameObject showToggle;
//     [SerializeField] private GameObject touchMathHintToggle;
//     [SerializeField] private GameObject conditionToggle;
//     [SerializeField] private GameObject conditionSlider;

//     private static string currentRepKey; //repetition
//     private static string currentPRSKey; //positive reinforcement sound
//     private static string currentNSKey; //number sound
//     private static string currentTMHKey; //touch math hint
//     private static string currentOrderKey; //order/random
//     private static string currentShowKey; //show placeholders/numerals
//     private static string currentConditionKey; //require an action
//     private static string currentConditionSliderKey; //require an action

//     void Awake()
//     {
//         // Ensure only one instance exists
//         if (Instance != null && Instance != this)
//         {
//             Destroy(gameObject); // Destroy duplicate instances
//         }
//         Instance = this;
//     }

//     // Start is called once before the first execution of Update after the MonoBehaviour is created
//     void Start()
//     {
//         InitializePlayerPrefs();
//         InitializeVolume();
//         InitializeVolumeSliders();

//         SetupKeysAndInitialize();
//     }

//     #region Getter Setters

//     public static string CurrentRepKey
//     {
//         get { return currentRepKey; }
//         set { currentRepKey = value; }
//     }
//     public static string CurrentPRSKey
//     {
//         get { return currentPRSKey; }
//         set { currentPRSKey = value; }
//     }
//     public static string CurrentNSKey
//     {
//         get { return currentNSKey; }
//         set { currentNSKey = value; }
//     }
//     public static string CurrentShowKey
//     {
//         get { return currentShowKey; }
//         set { currentShowKey = value; }
//     }
//     public static string CurrentOrderKey
//     {
//         get { return currentOrderKey; }
//         set { currentOrderKey = value; }
//     }

//     public static string CurrentConditionKey
//     {
//         get { return currentConditionKey; }
//         set { currentConditionKey = value; }
//     }

//     public static string CurrentConditionSliderKey
//     {
//         get { return currentConditionSliderKey; }
//         set { currentConditionSliderKey = value; }
//     }

//     #endregion

//     #region Initialization

//     public void SetupKeysAndInitialize()
//     {
//         if (SceneController.currentScene == Scenes.TouchMath)
//         {
//             if (currentRepKey != "TouchMathRepetitions")
//             {
//                 currentRepKey = "TouchMathRepetitions";
//                 SetRepetitionsStaticVariable();
//             }

//             if (currentNSKey != "TouchMathNS")
//             {
//                 currentNSKey = "TouchMathNS";
//                 SetNSStaticVariable();
//             }

//             if (currentTMHKey != "TouchMathHints")
//             {
//                 currentTMHKey = "TouchMathHints";
//                 SetTouchMathHintStaticVariable();
//             }

//             InitializeRepetitionSlider();
//             InitializeNSToggle();
//             InitializeTMHToggle();
//         }

//         if (SceneController.currentScene == Scenes.LearningEquivalence)
//         {
//             if (currentRepKey != "LearningEquivalenceRepetitions")
//             {
//                 currentRepKey = "LearningEquivalenceRepetitions";
//                 SetRepetitionsStaticVariable();
//             }

//             if (currentPRSKey != "LearningEquivalencePRS")
//             {
//                 currentPRSKey = "LearningEquivalencePRS";
//                 SetPRSStaticVariable();
//             }
//             InitializeRepetitionSlider();
//             InitializePRSToggle();
//         }

//         if (SceneController.currentScene == Scenes.Counting)
//         {
//             if (currentRepKey != "CountingRepetitions")
//             {
//                 currentRepKey = "CountingRepetitions";
//                 SetRepetitionsStaticVariable();
//             }

//             if (currentPRSKey != "CountingPRS")
//             {
//                 currentPRSKey = "CountingPRS";
//                 SetPRSStaticVariable();
//             }

//             if (currentNSKey != "CountingNS")
//             {
//                 currentNSKey = "CountingNS";
//                 SetNSStaticVariable();
//             }

//             InitializeRepetitionSlider();
//             InitializePRSToggle();
//             InitializeNSToggle();
//         }

//         if (SceneController.currentScene == Scenes.CountingBlankSelection)
//         {
//             conditionSlider.GetComponentInChildren<Slider>().minValue = 1;
//             switch (Counting.countingOrder)
//             {
//                 case CountingOrder.Forward:
//                     conditionSlider.GetComponentInChildren<Slider>().maxValue = 8;
//                     if (currentConditionSliderKey != "CountingConditionForward")
//                     {
//                         currentConditionSliderKey = "CountingConditionForward";
//                         SetConditionSliderStaticVariable();
//                     }
//                     break;

//                 case CountingOrder.Backward:
//                     conditionSlider.GetComponentInChildren<Slider>().maxValue = 4;
//                     if (currentConditionSliderKey != "CountingConditionBackward")
//                     {
//                         currentConditionSliderKey = "CountingConditionBackward";
//                         SetConditionSliderStaticVariable();
//                     }
//                     break;
//             }

//             InitializeConditionSlider();
//         }

//         if (SceneController.currentScene == Scenes.AddSubDifficulty)
//         {
//             if (currentConditionSliderKey != "AddSubCondition")
//             {
//                 currentConditionSliderKey = "AddSubCondition";
//                 SetConditionSliderStaticVariable();
//             }

//             InitializeConditionSlider();
//         }

//         if (SceneController.currentScene == Scenes.AddSub)
//         {
//             if (currentNSKey != "AddSubNS")
//             {
//                 currentNSKey = "AddSubNS";
//                 SetNSStaticVariable();
//             }

//             if (currentTMHKey != "AddSubHints")
//             {
//                 currentTMHKey = "AddSubHints";
//                 SetTouchMathHintStaticVariable();
//             }

//             InitializeNSToggle();
//             InitializeTMHToggle();
//         }

//         if (SceneController.currentScene == Scenes.TestLinkingQuantityNumerals)
//         {

//             if (currentRepKey != "LQNRepetitions")
//             {
//                 currentRepKey = "LQNRepetitions";
//                 SetRepetitionsStaticVariable();
//             }

//             if (currentShowKey != "LQNShow")
//             {
//                 currentShowKey = "LQNShow";
//                 SetShowStaticVariable();
//             }

//             if (currentOrderKey != "LQNOrder")
//             {
//                 currentOrderKey = "LQNOrder";
//                 SetOrderStaticVariable(true);
//             }

//             InitializeRepetitionSlider();
//             InitializeShowToggle();
//             InitializeOrderToggle();
//         }

//         if (SceneController.currentScene == Scenes.LearningToCount)
//         {

//             if (currentRepKey != "LTCRepetitions")
//             {
//                 currentRepKey = "LTCRepetitions";
//                 SetRepetitionsStaticVariable();
//             }

//             if (currentPRSKey != "LTCPRS")
//             {
//                 currentPRSKey = "LTCPRS";
//                 SetPRSStaticVariable();
//             }

//             if (currentNSKey != "LTCNS")
//             {
//                 currentNSKey = "LTCNS";
//                 SetNSStaticVariable();
//             }

//             if (currentOrderKey != "LTCOrder")
//             {
//                 currentOrderKey = "LTCOrder";
//                 SetOrderStaticVariable(true);
//             }

//             if (currentConditionKey != "LTCCondition")
//             {
//                 currentConditionKey = "LTCCondition";
//                 SetConditionStaticVariable();
//             }

//             InitializeRepetitionSlider();
//             InitializePRSToggle();
//             InitializeNSToggle();
//             InitializeOrderToggle();
//             InitializeConditionToggle();
//         }

//         if (SceneController.currentScene == Scenes.GiveANumber)
//         {

//             if (currentRepKey != "GANRepetitions")
//             {
//                 currentRepKey = "GANRepetitions";
//                 SetRepetitionsStaticVariable();
//             }

//             if (currentShowKey != "GANShow")
//             {
//                 currentShowKey = "GANShow";
//                 SetShowStaticVariable();
//             }

//             if (currentOrderKey != "GANOrder")
//             {
//                 currentOrderKey = "GANOrder";
//                 SetOrderStaticVariable(true);
//             }

//             if (currentPRSKey != "GANPRS")
//             {
//                 currentPRSKey = "GANPRS";
//                 SetPRSStaticVariable();
//             }

//             if (currentNSKey != "GANNS")
//             {
//                 currentNSKey = "GANNS";
//                 SetNSStaticVariable();
//             }

//             InitializeRepetitionSlider();
//             InitializeShowToggle();
//             InitializeOrderToggle();
//             InitializePRSToggle();
//             InitializeNSToggle();
//         }

//         if (SceneController.currentScene == Scenes.NumberWords)
//         {

//             if (currentRepKey != "NWRepetitions")
//             {
//                 currentRepKey = "NWRepetitions";
//                 SetRepetitionsStaticVariable();
//             }

//             if (currentNSKey != "NWNS")
//             {
//                 currentNSKey = "NWNS";
//                 SetNSStaticVariable();
//             }

//             InitializeRepetitionSlider();
//             InitializeNSToggle();
//         }

//         if (SceneController.currentScene == Scenes.MatchingNumerals)
//         {
//             if (currentRepKey != "MNRepetitions")
//             {
//                 currentRepKey = "MNRepetitions";
//                 SetRepetitionsStaticVariable();
//             }

//             if (currentPRSKey != "MNPRS")
//             {
//                 currentPRSKey = "MNPRS";
//                 SetPRSStaticVariable();
//             }

//             if (currentOrderKey != "MNOrder")
//             {
//                 currentOrderKey = "MNOrder";
//                 SetOrderStaticVariable(true);
//             }

//             InitializeRepetitionSlider();
//             InitializePRSToggle();
//             InitializeOrderToggle();
//         }

//         if (SceneController.currentScene == Scenes.SelectingNumerals)
//         {
//             if (currentRepKey != "SNRepetitions")
//             {
//                 currentRepKey = "SNRepetitions";
//                 SetRepetitionsStaticVariable();
//             }

//             if (currentPRSKey != "SNPRS")
//             {
//                 currentPRSKey = "SNPRS";
//                 SetPRSStaticVariable();
//             }

//             if (currentOrderKey != "SNOrder")
//             {
//                 currentOrderKey = "SNOrder";
//                 SetOrderStaticVariable(true);
//             }

//             InitializeRepetitionSlider();
//             InitializePRSToggle();
//             InitializeOrderToggle();
//         }

//         if (SceneController.currentScene == Scenes.NamingNumerals)
//         {
//             if (currentRepKey != "NNRepetitions")
//             {
//                 currentRepKey = "NNRepetitions";
//                 SetRepetitionsStaticVariable();
//             }

//             if (currentNSKey != "NNNS")
//             {
//                 currentNSKey = "NNNS";
//                 SetNSStaticVariable();
//             }

//             if (currentOrderKey != "NNOrder")
//             {
//                 currentOrderKey = "NNOrder";
//                 SetOrderStaticVariable(true);
//             }

//             InitializeRepetitionSlider();
//             InitializeNSToggle();
//             InitializeOrderToggle();
//         }
//     }

//     public void InitializePlayerPrefs()
//     {
//         Debug.Log("InitializePlayerPrefs Called");

//         if (!PlayerPrefs.HasKey("MasterVolume"))
//         {
//             PlayerPrefs.SetFloat("MasterVolume", 0f);
//         }

//         if (!PlayerPrefs.HasKey("MusicVolume"))
//         {
//             PlayerPrefs.SetFloat("MusicVolume", 0f);
//         }

//         if (!PlayerPrefs.HasKey("SFXVolume"))
//         {
//             PlayerPrefs.SetFloat("SFXVolume", 0f);
//         }

//         if (!PlayerPrefs.HasKey("TouchMathHints"))
//         {
//             SetBool("TouchMathHints", false);
//         }

//         if (!PlayerPrefs.HasKey("TouchMathRepetitions"))
//         {
//             PlayerPrefs.SetInt("TouchMathRepetitions", 0);
//         }

//         if (!PlayerPrefs.HasKey("TouchMathNS"))
//         {
//             SetBool("TouchMathNS", true);
//         }

//         if (!PlayerPrefs.HasKey("AddSubHints"))
//         {
//             SetBool("AddSubHints", false);
//         }

//         if (!PlayerPrefs.HasKey("AddSubNS"))
//         {
//             SetBool("AddSubNS", true);
//         }

//         if (!PlayerPrefs.HasKey("AddSubCondition"))
//         {
//             PlayerPrefs.SetInt("AddSubCondition", 5);
//         }

//         if (!PlayerPrefs.HasKey("CountingRepetitions"))
//         {
//             PlayerPrefs.SetInt("CountingRepetitions", 0);
//         }

//         if (!PlayerPrefs.HasKey("CountingPRS"))
//         {
//             SetBool("CountingPRS", true);
//         }

//         if (!PlayerPrefs.HasKey("CountingNS"))
//         {
//             SetBool("CountingNS", false);
//         }

//         if (!PlayerPrefs.HasKey("CountingConditionForward"))
//         {
//             PlayerPrefs.SetInt("CountingConditionForward", 1);
//         }

//         if (!PlayerPrefs.HasKey("CountingConditionBackward"))
//         {
//             PlayerPrefs.SetInt("CountingConditionBackward", 1);
//         }

//         if (!PlayerPrefs.HasKey("LearningEquivalenceRepetitions"))
//         {
//             PlayerPrefs.SetInt("LearningEquivalenceRepetitions", 0);
//         }

//         if (!PlayerPrefs.HasKey("LearningEquivalencePRS"))
//         {
//             SetBool("LearningEquivalencePRS", true);
//         }

//         if (!PlayerPrefs.HasKey("LQNRepetitions"))
//         {
//             PlayerPrefs.SetInt("LQNRepetitions", 0);
//         }

//         if (!PlayerPrefs.HasKey("LQNShow"))
//         {
//             SetBool("LQNShow", true);
//         }

//         if (!PlayerPrefs.HasKey("LQNOrder"))
//         {
//             SetBool("LQNOrder", true);
//         }

//         if (!PlayerPrefs.HasKey("LTCRepetitions"))
//         {
//             PlayerPrefs.SetInt("LTCRepititions", 0);
//         }

//         if (!PlayerPrefs.HasKey("LTCPRS"))
//         {
//             SetBool("LTCPRS", true);
//         }

//         if (!PlayerPrefs.HasKey("LTCNS"))
//         {
//             SetBool("LTCNS", true);
//         }

//         if (!PlayerPrefs.HasKey("LTCOrder"))
//         {
//             SetBool("LTCOrder", true);
//         }

//         if (!PlayerPrefs.HasKey("LTCCondition"))
//         {
//             SetBool("LTCCondition", true);
//         }

//         if (!PlayerPrefs.HasKey("GANRepetitions"))
//         {
//             PlayerPrefs.SetInt("GANRepetitions", 0);
//         }

//         if (!PlayerPrefs.HasKey("GANOrder"))
//         {
//             SetBool("GANOrder", true);
//         }

//         if (!PlayerPrefs.HasKey("GANShow"))
//         {
//             SetBool("GANShow", true);
//         }

//         if (!PlayerPrefs.HasKey("GANPRS"))
//         {
//             SetBool("GANPRS", true);
//         }

//         if (!PlayerPrefs.HasKey("GANNS"))
//         {
//             SetBool("GANNS", true);
//         }

//         if (!PlayerPrefs.HasKey("NWRepetitions"))
//         {
//             PlayerPrefs.SetInt("NWRepititions", 0);
//         }

//         if (!PlayerPrefs.HasKey("NWNS"))
//         {
//             SetBool("NWNS", true);
//         }

//         if (!PlayerPrefs.HasKey("MNRepetitions"))
//         {
//             PlayerPrefs.SetInt("MNRepititions", 0);
//         }

//         if (!PlayerPrefs.HasKey("MNPRS"))
//         {
//             SetBool("MNPRS", true);
//         }

//         if (!PlayerPrefs.HasKey("MNOrder"))
//         {
//             SetBool("MNOrder", true);
//         }

//         if (!PlayerPrefs.HasKey("SNRepetitions"))
//         {
//             PlayerPrefs.SetInt("SNRepititions", 0);
//         }

//         if (!PlayerPrefs.HasKey("SNPRS"))
//         {
//             SetBool("SNPRS", true);
//         }

//         if (!PlayerPrefs.HasKey("SNOrder"))
//         {
//             SetBool("SNOrder", true);
//         }

//         if (!PlayerPrefs.HasKey("NNRepetitions"))
//         {
//             PlayerPrefs.SetInt("MNRepititions", 0);
//         }

//         if (!PlayerPrefs.HasKey("NNNS"))
//         {
//             SetBool("NNNS", true);
//         }

//         if (!PlayerPrefs.HasKey("NNOrder"))
//         {
//             SetBool("NNOrder", true);
//         }

//         PlayerPrefs.Save(); // Save all changes
//     }

//     public void InitializeVolume()
//     {
//         Debug.Log("Initialize Volume Called");
//         audioMixer.SetFloat("MasterVolume", PlayerPrefs.GetFloat("MasterVolume", 0f));
//         audioMixer.SetFloat("MusicVolume", PlayerPrefs.GetFloat("MusicVolume", 0f));
//         audioMixer.SetFloat("SFXVolume", PlayerPrefs.GetFloat("SFXVolume", 0f));
//     }

//     public void InitializeVolumeSliders()
//     {
//         Debug.Log("Initialize Volume Sliders Called");
//         masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 0f);
//         musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0f);
//         sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0f);

//         masterVolumeSlider.onValueChanged.AddListener(AdjustMasterVolume);
//         musicVolumeSlider.onValueChanged.AddListener(AdjustMusicVolume);
//         sfxVolumeSlider.onValueChanged.AddListener(AdjustSFXVolume);
//     }

//     public void InitializeRepetitionSlider()
//     {
//         repetitionSlider.GetComponentInChildren<Slider>().value = PlayerPrefs.GetInt(currentRepKey, 0);
//         repetitionSlider.GetComponentInChildren<TextMeshProUGUI>().text = PlayerPrefs.GetInt(currentRepKey, 0).ToString();
//         repetitionSlider.GetComponentInChildren<Slider>().onValueChanged.AddListener(AdjustRepetitions);
//     }

//     public void InitializePRSToggle()
//     {
//         prsToggle.GetComponent<Toggle>().isOn = GetBool(currentPRSKey);
//         prsToggle.GetComponent<Toggle>().onValueChanged.AddListener(PRSToggleValueChanged);
//         prsToggle.GetComponentInChildren<TextMeshProUGUI>().text = GiveOnOffText(GetBool(currentPRSKey));
//     }

//     public void InitializeNSToggle()
//     {
//         nsToggle.GetComponent<Toggle>().isOn = GetBool(currentNSKey);
//         nsToggle.GetComponent<Toggle>().onValueChanged.AddListener(NSToggleValueChanged);
//         nsToggle.GetComponentInChildren<TextMeshProUGUI>().text = GiveOnOffText(GetBool(currentNSKey));
//     }

//     public void InitializeTMHToggle()
//     {
//         touchMathHintToggle.GetComponent<Toggle>().isOn = GetBool(currentTMHKey);
//         touchMathHintToggle.GetComponent<Toggle>().onValueChanged.AddListener(TouchMathHintToggleValueChanged);
//         touchMathHintToggle.GetComponentInChildren<TextMeshProUGUI>().text = GiveOnOffText(GetBool(currentTMHKey));
//     }

//     public void InitializeShowToggle()
//     {
//         showToggle.GetComponent<Toggle>().isOn = GetBool(currentShowKey);
//         showToggle.GetComponent<Toggle>().onValueChanged.AddListener(ShowToggleValueChanged);
//         showToggle.GetComponentInChildren<TextMeshProUGUI>().text = GiveOnOffText(GetBool(currentShowKey));
//     }

//     public void InitializeOrderToggle()
//     {
//         Debug.Log("InitializeOrderToggle Called");
//         orderToggle.GetComponent<Toggle>().isOn = GetBool(currentOrderKey);
//         orderToggle.GetComponent<Toggle>().onValueChanged.AddListener(OrderToggleValueChanged);
//         orderToggle.GetComponentInChildren<TextMeshProUGUI>().text = GiveOnOffText(GetBool(currentOrderKey));
//     }

//     public void InitializeConditionToggle()
//     {
//         Debug.Log("InitializeConditionToggle Called");
//         conditionToggle.GetComponent<Toggle>().isOn = GetBool(currentConditionKey);
//         conditionToggle.GetComponent<Toggle>().onValueChanged.AddListener(ConditionToggleValueChanged);
//         conditionToggle.GetComponentInChildren<TextMeshProUGUI>().text = GiveOnOffText(GetBool(currentConditionKey));
//     }

//     public void InitializeConditionSlider()
//     {
//         Debug.Log("InitializeConditionSlider Called");
//         switch (currentConditionSliderKey)
//         {
//             case "AddSubCondition":
//                 conditionSlider.GetComponentInChildren<Slider>().value = PlayerPrefs.GetInt(currentConditionSliderKey, 5);
//                 conditionSlider.GetComponentInChildren<TextMeshProUGUI>().text = "WITHIN " + PlayerPrefs.GetInt(currentConditionSliderKey, 5).ToString();
//                 break;

//             case "CountingConditionForward":
//                 conditionSlider.GetComponentInChildren<Slider>().value = PlayerPrefs.GetInt(currentConditionSliderKey, 1);
//                 switch (PlayerPrefs.GetInt(currentConditionSliderKey, 1))
//                 {
//                     case 1:
//                         conditionSlider.GetComponentInChildren<TextMeshProUGUI>().text = PlayerPrefs.GetInt(currentConditionSliderKey, 1).ToString() + " BLANK";
//                         break;
//                     case > 1:
//                         conditionSlider.GetComponentInChildren<TextMeshProUGUI>().text = PlayerPrefs.GetInt(currentConditionSliderKey, 1).ToString() + " BLANKS";
//                         break;
//                 }
//                 break;

//             case "CountingConditionBackward":
//                 conditionSlider.GetComponentInChildren<Slider>().value = PlayerPrefs.GetInt(currentConditionSliderKey, 1);
//                 switch (PlayerPrefs.GetInt(currentConditionSliderKey, 1))
//                 {
//                     case 1:
//                         conditionSlider.GetComponentInChildren<TextMeshProUGUI>().text = PlayerPrefs.GetInt(currentConditionSliderKey, 1).ToString() + " BLANK";
//                         break;
//                     case > 1:
//                         conditionSlider.GetComponentInChildren<TextMeshProUGUI>().text = PlayerPrefs.GetInt(currentConditionSliderKey, 1).ToString() + " BLANKS";
//                         break;
//                 }
//                 break;

//             default:
//                 Debug.Log("Unknown Order state");
//                 break;
//         }
//         conditionSlider.GetComponentInChildren<Slider>().onValueChanged.AddListener(ConditionSliderValueChanged);
//     }

//     #endregion

//     // Update is called once per frame
//     void Update()
//     {

//     }

//     #region Value Change Functions (Listeners)

//     public void TouchMathHintToggleValueChanged(bool isOn)
//     {
//         SetBool(currentTMHKey, isOn);
//         SetTouchMathHintStaticVariable();
//         touchMathHintToggle.GetComponentInChildren<TextMeshProUGUI>().text = GiveOnOffText(GetBool(currentTMHKey));
//     }

//     public void AdjustMasterVolume(float value)
//     {
//         audioMixer.SetFloat("MasterVolume", value);
//         musicVolumeSlider.value = value;
//         sfxVolumeSlider.value = value;
//         PlayerPrefs.SetFloat("MasterVolume", value);
//     }

//     public void AdjustMusicVolume(float value)
//     {
//         audioMixer.SetFloat("MusicVolume", value);
//         PlayerPrefs.SetFloat("MusicVolume", value);
//     }

//     public void AdjustSFXVolume(float value)
//     {
//         audioMixer.SetFloat("SFXVolume", value);
//         PlayerPrefs.SetFloat("SFXVolume", value);
//     }

//     public void AdjustRepetitions(float value)
//     {
//         Debug.Log("Adjust Repetitions Called");
//         PlayerPrefs.SetInt(currentRepKey, (int)value);
//         repetitionSlider.GetComponentInChildren<TextMeshProUGUI>().text = PlayerPrefs.GetInt(currentRepKey, 0).ToString();
//         SetRepetitionsStaticVariable();
//     }

//     public void PRSToggleValueChanged(bool isOn)
//     {
//         SetBool(currentPRSKey, isOn);
//         SetPRSStaticVariable();
//         prsToggle.GetComponentInChildren<TextMeshProUGUI>().text = GiveOnOffText(GetBool(currentPRSKey));
//     }

//     public void NSToggleValueChanged(bool isOn)
//     {
//         SetBool(currentNSKey, isOn);
//         SetNSStaticVariable();
//         nsToggle.GetComponentInChildren<TextMeshProUGUI>().text = GiveOnOffText(GetBool(currentNSKey));
//     }

//     public void ShowToggleValueChanged(bool isOn)
//     {
//         SetBool(currentShowKey, isOn);
//         SetShowStaticVariable();
//         showToggle.GetComponentInChildren<TextMeshProUGUI>().text = GiveOnOffText(GetBool(currentShowKey));
//     }

//     public void OrderToggleValueChanged(bool isOn)
//     {
//         SetBool(currentOrderKey, isOn);
//         SetOrderStaticVariable(false);
//         orderToggle.GetComponentInChildren<TextMeshProUGUI>().text = GiveOnOffText(GetBool(currentOrderKey));
//     }

//     public void ConditionToggleValueChanged(bool isOn)
//     {
//         SetBool(currentConditionKey, isOn);
//         SetConditionStaticVariable();
//         conditionToggle.GetComponentInChildren<TextMeshProUGUI>().text = GiveOnOffText(GetBool(currentConditionKey));
//     }

//     public void ConditionSliderValueChanged(float value)
//     {
//         PlayerPrefs.SetInt(currentConditionSliderKey, (int)value);
//         SetConditionSliderStaticVariable();
//         switch (currentConditionSliderKey)
//         {
//             case "AddSubCondition":
//                 conditionSlider.GetComponentInChildren<TextMeshProUGUI>().text = "WITHIN " + PlayerPrefs.GetInt(currentConditionSliderKey, 5).ToString();
//                 break;

//             case "CountingConditionForward":
//                 switch (PlayerPrefs.GetInt(currentConditionSliderKey, 1))
//                 {
//                     case 1:
//                         conditionSlider.GetComponentInChildren<TextMeshProUGUI>().text = PlayerPrefs.GetInt(currentConditionSliderKey, 1).ToString() + " BLANK";
//                         break;
//                     case > 1:
//                         conditionSlider.GetComponentInChildren<TextMeshProUGUI>().text = PlayerPrefs.GetInt(currentConditionSliderKey, 1).ToString() + " BLANKS";
//                         break;
//                 }
//                 break;

//             case "CountingConditionBackward":
//                 switch (PlayerPrefs.GetInt(currentConditionSliderKey, 1))
//                 {
//                     case 1:
//                         conditionSlider.GetComponentInChildren<TextMeshProUGUI>().text = PlayerPrefs.GetInt(currentConditionSliderKey, 1).ToString() + " BLANK";
//                         break;
//                     case > 1:
//                         conditionSlider.GetComponentInChildren<TextMeshProUGUI>().text = PlayerPrefs.GetInt(currentConditionSliderKey, 1).ToString() + " BLANKS";
//                         break;
//                 }
//                 break;

//             default:
//                 Debug.Log("Unknown Order state");
//                 break;
//         }
//     }

//     #endregion

//     #region Set Static Variables

//     public void SetRepetitionsStaticVariable()
//     {
//         switch (currentRepKey)
//         {
//             case "LearningEquivalenceRepetitions":
//                 LearningEquivalence.Repetitions = PlayerPrefs.GetInt(currentRepKey, 0);
//                 break;

//             case "TouchMathRepetitions":
//                 ButtonManager.Repetitions = PlayerPrefs.GetInt(currentRepKey, 0);
//                 break;

//             case "CountingRepetitions":
//                 Counting.Repetitions = PlayerPrefs.GetInt(currentRepKey, 0);
//                 break;

//             case "LQNRepetitions":
//                 LinkingQuantityNumerals.Repetitions = PlayerPrefs.GetInt(currentRepKey, 0);
//                 break;

//             case "LTCRepetitions":
//                 LearningToCount.Repetitions = PlayerPrefs.GetInt(currentRepKey, 0);
//                 break;

//             case "GANRepetitions":
//                 GiveANumber.Repetitions = PlayerPrefs.GetInt(currentRepKey, 0);
//                 break;

//             case "NWRepetitions":
//                 NumberWord.Repetitions = PlayerPrefs.GetInt(currentRepKey, 0);
//                 break;

//             case "MNRepetitions":
//                 MatchingNumerals.Repetitions = PlayerPrefs.GetInt(currentRepKey, 0);
//                 break;

//             case "NNRepetitions":
//                 NamingNumerals.Repetitions = PlayerPrefs.GetInt(currentRepKey, 0);
//                 break;

//             case "SNRepetitions":
//                 SelectingNumerals.Repetitions = PlayerPrefs.GetInt(currentRepKey, 0);
//                 break;

//             default:
//                 Debug.Log("Unknown Rep state");
//                 break;
//         }
//     }

//     public void SetPRSStaticVariable()
//     {
//         switch (currentPRSKey)
//         {
//             case "LearningEquivalencePRS":
//                 LearningEquivalence.PRS = GetBool(currentPRSKey, true);
//                 break;

//             case "CountingPRS":
//                 Counting.PRS = GetBool(currentPRSKey, true);
//                 if (Counting.PRS == true)
//                 {
//                     SetBool(currentNSKey, false);
//                     SetNSStaticVariable();
//                     nsToggle.GetComponent<Toggle>().isOn = false;
//                     nsToggle.GetComponentInChildren<TextMeshProUGUI>().text = GiveOnOffText(GetBool(currentNSKey));
//                 }
//                 break;

//             case "LTCPRS":
//                 LearningToCount.PRS = GetBool(currentPRSKey, true);
//                 break;

//             case "GANPRS":
//                 GiveANumber.PRS = GetBool(currentPRSKey, true);
//                 break;

//             case "MNPRS":
//                 MatchingNumerals.PRS = GetBool(currentPRSKey, true);
//                 break;

//             case "SNPRS":
//                 SelectingNumerals.PRS = GetBool(currentPRSKey, true);
//                 break;

//             default:
//                 Debug.Log("Unknown PRS state");
//                 break;
//         }
//     }

//     public void SetNSStaticVariable()
//     {
//         switch (currentNSKey)
//         {
//             case "TouchMathNS":
//                 ButtonManager.NS = GetBool(currentNSKey, true);
//                 break;

//             case "AddSubNS":
//                 AddSub.NS = GetBool(currentNSKey, true);
//                 break;

//             case "LTCNS":
//                 LearningToCount.NS = GetBool(currentNSKey, true);
//                 break;

//             case "NWNS":
//                 NumberWord.NS = GetBool(currentNSKey, true);
//                 break;

//             case "NNNS":
//                 NamingNumerals.NS = GetBool(currentNSKey, true);
//                 break;

//             case "CountingNS":
//                 Counting.NS = GetBool(currentNSKey, true);
//                 if (Counting.NS == true)
//                 {
//                     SetBool(currentPRSKey, false);
//                     SetPRSStaticVariable();
//                     prsToggle.GetComponent<Toggle>().isOn = false;
//                     prsToggle.GetComponentInChildren<TextMeshProUGUI>().text = GiveOnOffText(GetBool(currentPRSKey));
//                 }
//                 break;

//             case "GANNS":
//                 GiveANumber.NS = GetBool(currentNSKey, true);
//                 break;

//             default:
//                 Debug.Log("Unknown NS state");
//                 break;
//         }
//     }

//     public void SetShowStaticVariable()
//     {
//         switch (currentShowKey)
//         {
//             case "LQNShow":
//                 LinkingQuantityNumerals.Show = GetBool(currentShowKey, true);
//                 if (LinkingQuantityNumerals.Instance != null)
//                 {
//                     LinkingQuantityNumerals.Instance.SetGridStatesShow();
//                 }
//                 break;

//             case "GANShow":
//                 GiveANumber.Show = GetBool(currentShowKey, true);
//                 if (GiveANumber.Instance != null)
//                 {
//                     GiveANumber.Instance.SetNumeralStateShow();
//                 }
//                 break;

//             default:
//                 Debug.Log("Unknown Show state");
//                 break;
//         }
//     }

//     public void SetOrderStaticVariable(bool calledFromStart)
//     {
//         switch (currentOrderKey)
//         {
//             case "LQNOrder":
//                 LinkingQuantityNumerals.Order = GetBool(currentOrderKey, true);
//                 if (LinkingQuantityNumerals.Instance != null && !calledFromStart)
//                 {
//                     LinkingQuantityNumerals.Instance.ReloadSceneOnOrderChange();
//                 }
//                 break;

//             case "LTCOrder":
//                 LearningToCount.Order = GetBool(currentOrderKey, true);
//                 if (LearningToCount.Instance != null && !calledFromStart)
//                 {
//                     LearningToCount.Instance.ReloadSceneOnOrderChange();
//                 }
//                 break;

//             case "GANOrder":
//                 GiveANumber.Order = GetBool(currentOrderKey, true);
//                 if (GiveANumber.Instance != null && !calledFromStart)
//                 {
//                     GiveANumber.Instance.ReloadSceneOnOrderChange();
//                 }
//                 break;

//             case "MNOrder":
//                 MatchingNumerals.Order = GetBool(currentOrderKey, true);
//                 if (MatchingNumerals.Instance != null && !calledFromStart)
//                 {
//                     MatchingNumerals.Instance.ReloadSceneOnOrderChange();
//                 }
//                 break;

//             case "NNOrder":
//                 NamingNumerals.Order = GetBool(currentOrderKey, true);
//                 if (NamingNumerals.Instance != null && !calledFromStart)
//                 {
//                     NamingNumerals.Instance.ReloadSceneOnOrderChange();
//                 }
//                 break;

//             case "SNOrder":
//                 SelectingNumerals.Order = GetBool(currentOrderKey, true);
//                 if (SelectingNumerals.Instance != null && !calledFromStart)
//                 {
//                     SelectingNumerals.Instance.ReloadSceneOnOrderChange();
//                 }
//                 break;

//             default:
//                 Debug.Log("Unknown Order state");
//                 break;
//         }
//     }

//     public void SetConditionStaticVariable()
//     {
//         switch (currentConditionKey)
//         {
//             case "LTCCondition":
//                 LearningToCount.Condition = GetBool(currentConditionKey, true);
//                 break;

//             default:
//                 Debug.Log("Unknown Order state");
//                 break;
//         }
//     }

//     public void SetConditionSliderStaticVariable()
//     {
//         switch (currentConditionSliderKey)
//         {
//             case "AddSubCondition":
//                 AddSub.Condition = PlayerPrefs.GetInt(currentConditionSliderKey, 5);
//                 break;

//             case "CountingConditionForward":
//                 Counting.Condition = PlayerPrefs.GetInt(currentConditionSliderKey, 1);
//                 break;

//             case "CountingConditionBackward":
//                 Counting.Condition = PlayerPrefs.GetInt(currentConditionSliderKey, 1);
//                 break;

//             default:
//                 Debug.Log("Unknown Order state");
//                 break;
//         }
//     }

//     public void SetTouchMathHintStaticVariable()
//     {
//         ButtonManager.IsHintEnabled = GetBool(currentTMHKey, false);
//     }

//     #endregion

//     #region Class Specific Functions

//     public void SetOrderToggleInteractable(bool interactable)
//     {
//         Debug.Log("SetOrderToggleInteractable Called");
//         if (orderToggle != null)
//         {
//             orderToggle.GetComponent<Toggle>().interactable = interactable;
//         }
//     }

//     #endregion

//     #region  Helper Functions

//     public string GiveOnOffText(bool isOn)
//     {
//         switch (isOn)
//         {
//             case false:
//                 return "Off";
//             case true:
//                 return "On";
//         }
//     }

//     public static void SetBool(string key, bool value)
//     {
//         PlayerPrefs.SetInt(key, value ? 1 : 0);
//     }

//     public static bool GetBool(string key, bool defaultValue = false)
//     {
//         return PlayerPrefs.GetInt(key, defaultValue ? 1 : 0) == 1;
//     }

//     #endregion

// }

// //Module Wise:

// // 1.Touch Math
// // TouchMathHints(bool)
// // TouchMathRepetitions (int)
// // TouchMathNS (bool)

// // 2. Addition & Subtraction
// // AddSubHints (bool) 
// // AddSubNS (bool) 

// // 3. Counting
// // CountingRepetitions (int) 
// // CountingPRS (bool) 
// // CountingNS (bool) 

// // 4. Learning Equivalence
// // LearningEquivalenceRepetitions (int)
// // LearningEquivalencePRS (bool)

// // 5. LQN (Likely “Largest Quantity Number” or similar)
// // LQNRepetitions (int)
// // LQNShow (bool) 
// // LQNOrder (bool) 

// // 6. LTC
// // LTCRepetitions (int)
// // LTCPRS (bool)
// // LTCNS (bool)
// // LTCOrder (bool)
// // LTCCondition (bool)

// // 7. GAN
// // GANRepetitions (int)
// // GANOrder (bool)
// // GANShow (bool)
// // GANPRS (bool)
// // GANNS (bool)

// // 8. NW
// // NWRepetitions (int)
// // NWNS (bool)

// // 9. MN
// // MNRepetitions (int)
// // MNPRS (bool)
// // MNOrder (bool)

// // 10. SN
// // SNRepetitions (int)
// // SNPRS (bool)
// // SNOrder (bool)

// // 11. NN
// // NNRepetitions (int)
// // NNNS (bool)
// // NNOrder (bool)

// //Settings Wise:

// // Uses Repetitions
// // Touch Math (TouchMathRepetitions)
// // Counting (CountingRepetitions)
// // Learning Equivalence (LearningEquivalenceRepetitions)
// // LQN (LQNRepetitions)
// // LTC (LTCRepetitions)
// // GAN (GANRepetitions)
// // NW (NWRepetitions)
// // MN (MNRepetitions)
// // SN (SNRepetitions)
// // NN (NNRepetitions)

// // Uses Order
// // LQN (LQNOrder)
// // LTC (LTCOrder)
// // GAN (GANOrder)
// // MN (MNOrder)
// // SN (SNOrder)
// // NN (NNOrder)

// // Uses Show
// // LQN (LQNShow)
// // GAN (GANShow)

// // Uses NS (Number Sense)
// // Touch Math (TouchMathNS)
// // Add/Sub (AddSubNS)
// // Counting (CountingNS)
// // LTC (LTCNS)
// // NW (NWNS)
// // NN (NNNS)
// // GAN (GANNS)

// // Uses Condition
// // LTC (LTCCondition)

// // Uses PRS
// // Counting (CountingPRS)
// // Learning Equivalence (LearningEquivalencePRS)
// // LTC (LTCPRS)
// // GAN (GANPRS)
// // MN (MNPRS)
// // SN (SNPRS)