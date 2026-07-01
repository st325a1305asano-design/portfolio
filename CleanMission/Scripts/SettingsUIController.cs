using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static UnityEngine.Rendering.DebugUI;
//using UnityEngine.UIElements;

/// <summary>
/// 設定UI
/// </summary>
public class SettingsUIController : MonoBehaviour
{
    #region 変数の宣言
    [Header("UI")]
    [SerializeField] Toggle invertToggle; //カメラ反転トグル
    [SerializeField] Slider sensitivitySlider; //感度スライダー
    [SerializeField] Slider bgmSlider; //BGM音量スライダー
    [SerializeField] Slider seSlider; //SE音量スライダー
    [SerializeField] TextMeshProUGUI sensitivityText; //感度表示テキスト
    [SerializeField] TextMeshProUGUI bgmText; //BGM音量表示テキスト
    [SerializeField] TextMeshProUGUI seText; //SE音量表示テキスト
    #endregion

    [SerializeField] float maxBGMVolume = 100;
    [SerializeField] float maxSEVolume = 100;


    void Start()
    {
        //カメラ反転の連携
        invertToggle.isOn = SettingsManager.Instance.invertY;
        invertToggle.onValueChanged.AddListener(OnToggleChanged);

        //感度設定の連携
        sensitivitySlider.value = SettingsManager.Instance.sensitivity;
        sensitivitySlider.onValueChanged.AddListener(OnSensitivityValueChanged);

        //感度設定の連携
        bgmSlider.value = SettingsManager.Instance.bgmVolume;
        bgmSlider.onValueChanged.AddListener(OnBGMValueChanged);

        //感度設定の連携
        seSlider.value = SettingsManager.Instance.seVolume;
        seSlider.onValueChanged.AddListener(OnSEValueChanged);
    }

    void Update()
    {
        //数値をテキストに表示
        sensitivityText.text = SettingsManager.Instance.sensitivity.ToString("F0");
        bgmText.text = (SettingsManager.Instance.bgmVolume * maxBGMVolume).ToString("F0") + "%";
        seText.text = (SettingsManager.Instance.seVolume * maxSEVolume).ToString("F0") + "%";

    }

    /// <summary>
    /// カメラ反転の変更
    /// </summary>
    /// <param name="value"></param>
    void OnToggleChanged(bool value)
    {
        SettingsManager.Instance.invertY = value;
    }

    /// <summary>
    /// 感度設定の変更
    /// </summary>
    /// <param name="value"></param>
    void OnSensitivityValueChanged(float value)
    {
        SettingsManager.Instance.sensitivity = value;
    }

    /// <summary>
    /// BGM音量の変更
    /// </summary>
    /// <param name="value"></param>
    void OnBGMValueChanged(float value)
    {
        SettingsManager.Instance.bgmVolume = value;
    }

    /// <summary>
    /// SE音量の変更
    /// </summary>
    /// <param name="value"></param>
    void OnSEValueChanged(float value)
    {
        SettingsManager.Instance.seVolume = value;
    }
}