using UnityEngine;

/// <summary>
/// 設定保持
/// </summary>
public class Settings : MonoBehaviour
{
    #region 変数の宣言
    public static Settings Instance; //インスタンス

    [Header("感度")]
    private float sensitivity = 100f;

    [Header("反転")]
    private bool invertY = false;

    [Header("BGM音量")]
    private float bgmVolume = 0.1f;

    [Header("SE音量")]
    private float seVolume = 1.0f;
    #endregion

    #region　プロパティ
    public float Sensitivity { get => sensitivity; set => sensitivity = value; }
    public bool InvertY { get => invertY; set => invertY = value; }
    public float BgmVolume { get => bgmVolume; set => bgmVolume = value; }
    public float SEVolume { get => seVolume; set => seVolume = value; }
    #endregion

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); //ロードしてもゲームオブジェクトを壊さない
        }
        else
        {
            Destroy(gameObject); //Instanceがすでにあるなら破壊
        }
    }
}
