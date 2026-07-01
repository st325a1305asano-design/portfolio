using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// タイトル画面などにおける各ボタンのクリックイベント
/// （UIインタラクション）を処理するクラス
/// </summary>
public class ButtonEventHandler : MonoBehaviour
{
    //変数の宣言
    [SerializeField] GameObject creditPanel; //クレジットパネル

    /// <summary>
    /// 次のシーンロード
    /// </summary>
    public void NextScene()
    {
        int index = SceneManager.GetActiveScene().buildIndex; //シーン番号取得
        index++; //シーン番号+1
        SceneManager.LoadScene(index); //シーンロード
    }

    /// <summary>
    /// 同じシーンロード
    /// </summary>
    public void Retry()
    {
        int index = SceneManager.GetActiveScene().buildIndex; //シーン番号取得

        SceneManager.LoadScene(index); //シーンロード
    }

    /// <summary>
    /// タイトルシーンロード
    /// </summary>
    public void ToTitle()
    {
        int index = 0;
        SceneManager.LoadScene(index); //シーンロード
    }

    /// <summary>
    /// ゲーム終了
    /// </summary>
    public void Exit()
    {
        {
            //UnityEditorならPlay終了
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#endif

            //ビルドならゲーム終了
            Application.Quit();
        }
    }

    /// <summary>
    /// クレジット表示
    /// </summary>
    public void Credit()
    {
        //クレジットパネルがアクティブなら
        if (creditPanel.activeInHierarchy)
        {
            //非表示
            creditPanel.SetActive(false);
        }
        //非アクティブなら
        else
        {
            //表示
            creditPanel.SetActive(true);
        }
    }
}
