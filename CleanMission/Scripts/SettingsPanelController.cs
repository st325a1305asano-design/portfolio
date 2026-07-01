using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// 設定画面管理
/// </summary>
public class SettingsPanelController : MonoBehaviour
{
    #region　変数の宣言
    [SerializeField] GameObject settingsPanel; //設定パネル
    [SerializeField] string pauseAction = "Pause";

    bool isOpen = false; //開閉管理フラグ

    PlayerInput playerInput; //PlayerInputを入れる変数
    QuizUIController quizUIController; //QuizUIControllerを入れる変数
    #endregion

    void Start()
    {
        settingsPanel.SetActive(false); //パネル非表示

        //シーン内から探して変数に入れる
        playerInput = FindAnyObjectByType<PlayerInput>();
        quizUIController = FindAnyObjectByType<QuizUIController>();
    }

    void Update()
    {
        //ステージだったら
        if (SceneManager.GetActiveScene().buildIndex == 1)
        { 
            // Tabキーで開閉（ゲーム中）　かつ　クイズをしていないなら
            if (playerInput.actions[pauseAction].WasPressedThisFrame() && !quizUIController.IsPlayingQuiz)
            {
                Toggle(); //トグル
            } 
        }
    }

    /// <summary>
    /// 展開処理
    /// </summary>
    public void Open()
    {
        settingsPanel.SetActive(true); //パネル表示
        isOpen = true; //展開中

        // プレイヤー操作停止
        PlayerController playerCon = FindAnyObjectByType<PlayerController>();
        if (playerCon != null)
            playerCon.ChangeStates(1); //インタラクト中に変更
    }

    /// <summary>
    /// 収縮処理
    /// </summary>
    public void Close()
    {
        settingsPanel.SetActive(false); //パネル非表示
        isOpen = false; //縮小

        // プレイヤー操作復帰
        PlayerController playerCon = FindAnyObjectByType<PlayerController>();
        if (playerCon != null)
            playerCon.ChangeStates(0); //探索中に変更
    }

    /// <summary>
    /// 入力時の呼び出す関数切り替え
    /// </summary>
    public void Toggle()
    {
        if (isOpen)
            Close();
        else
            Open();
    }
}