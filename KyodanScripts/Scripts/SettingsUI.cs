using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 設定UI連携
/// </summary>
public class SettingsUI : MonoBehaviour
{
    #region　変数の宣言
    [SerializeField] Slider sensitivitySlider; //視点感度スライダーを入れる変数
    [SerializeField] TextMeshProUGUI sensitivityValueText; //視点感度を表示するテキスト

    [SerializeField] Toggle invertToggle; //カメラ反転トグルを入れる変数

    [SerializeField] Slider bgmVolumeSlider; //BGM音量スライダーを入れる変数
    [SerializeField] TextMeshProUGUI bgmVolumeText; //BGM音量を表示するテキスト

    [SerializeField] Slider seVolumeSlider; //SE音量スライダーを入れる変数
    [SerializeField] TextMeshProUGUI seVolumeText; //SE音量を表示するテキスト

    [SerializeField] float maxBGM = 100f;
    [SerializeField] float maxSE = 100f;
    #endregion

    void Start()
    {
        Debug.Log($"sensitivitySlider = {sensitivitySlider}");
        Debug.Log($"Settings.Instance = {Settings.Instance}");

        //感度設定の連携
        sensitivitySlider.value = Settings.Instance.Sensitivity;
        sensitivitySlider.onValueChanged.AddListener(OnSensitivityValueChanged);

        //カメラ反転の連携
        invertToggle.isOn = Settings.Instance.InvertY;
        invertToggle.onValueChanged.AddListener(OnToggleChanged);

        bgmVolumeSlider.value = Settings.Instance.BgmVolume;
        bgmVolumeSlider.onValueChanged.AddListener(OnBGMValumeValueChanged);

        seVolumeSlider.value = Settings.Instance.SeVolume;
        seVolumeSlider.onValueChanged.AddListener(OnSEValumeValueChanged);
    }

    private void Update()
    {
        //数値をテキストに表示
        sensitivityValueText.text = Settings.Instance.Sensitivity.ToString("F0");
        bgmVolumeText.text = (Settings.Instance.BgmVolume * maxBGM).ToString("F0") + "%";
        seVolumeText.text = (Settings.Instance.SeVolume * maxSE).ToString("F0") + "%";
    }

    /// <summary>
    /// 感度設定の変更
    /// </summary>
    /// <param name="value"></param>
    void OnSensitivityValueChanged(float value)
    {
        Settings.Instance.Sensitivity = value;
    }

    /// <summary>
    /// カメラ反転の変更
    /// </summary>
    /// <param name="value"></param>
    void OnToggleChanged(bool value)
    {
        Settings.Instance.InvertY = value;
    }

    /// <summary>
    /// BGM音量設定の変更
    /// </summary>
    /// <param name="value"></param>
    void OnBGMValumeValueChanged(float value)
    {
        Settings.Instance.BgmVolume = value;
    }

    /// <summary>
    /// SE音量設定の変更
    /// </summary>
    /// <param name="value"></param>
    void OnSEValumeValueChanged(float value)
    {
        Settings.Instance.SEVolume = value;
    }
}
