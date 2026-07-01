using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using static Enums;

/// <summary>
/// ボタン押下時処理
/// </summary>
public class ButtonEventController : MonoBehaviour
{
    #region 変数の宣言
    //読み込むシーン名
    [SerializeField] string SceneName;
    //表示切替のパネル
    [SerializeField] GameObject panel;
    #endregion

    void Start()
    {
        if (panel != null)
        {
            panel.SetActive(false);
        }
    }

    /// <summary>
    /// 指定のシーンのロード
    /// </summary>
    public void Load()
    {
        SceneManager.LoadScene(SceneName);
    }

    /// <summary>
    /// 次のシーンロード
    /// </summary>
    public void NextScene()
    {
        //現在のシーンのナンバーを取得
        int index = SceneManager.GetActiveScene().buildIndex;

        //次のシーンをロード
        index++;
        SceneManager.LoadScene(index);
    }

    /// <summary>
    /// 現在のシーンロード
    /// </summary>
    public void RetryScene()
    {
        //現在のシーンロード
        int index = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(index);
    }

    /// <summary>
    /// ゲーム終了
    /// </summary>
    public void ExitGame()
    {
        //UnityEditorならPlay終了
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif

        //ビルドならゲーム終了
        Application.Quit();
    }

    /// <summary>
    /// パネルの表示非表示
    /// </summary>
    public void TogglePanel()
    {
        //フラグのトグル
        //trueなら表示
        if (!panel.activeInHierarchy)
        {
            panel.SetActive(true);
        }
        //falseなら非表示
        else
        {
            panel.SetActive(false);
        }
    }

    /// <summary>
    /// パネルを隠す
    /// </summary>
    public void HidePanel()
    {
        panel.SetActive(false);
    }

    /// <summary>
    /// ボタンを選択状態にする関数
    /// </summary>
    /// <param name="button"></param>
    public void SetSelectButton(GameObject button)
    {
        EventSystem.current.SetSelectedGameObject(button);
    }

    /// <summary>
    /// ボタンの選択状態を解除する
    /// </summary>
    public void NullSelectButton()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void ChangeGameStateToPlaying()
    {
        GameManager.Instance.ChangeGameState(GameState.Playing);
    }
}
