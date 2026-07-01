using UnityEngine;
using TMPro;
using System.Collections;
using Unity.VisualScripting;

/// <summary>
/// チュートリアル管理
/// </summary>
public class TutorialUIController : MonoBehaviour
{
    #region 変数の宣言
    [SerializeField] TextMeshProUGUI subjectText; //目的や忠告を表示するためのテキスト
    [SerializeField] string subject; //目標や忠告を入れる変数

    Color startColor; //テキストの初期の色
    float waitShowText = 8f; //通常表示時間1
    float duration = 3f; //通常表示時間2
    float fadeOut = 3f; //フェードアウト時間
    bool isWaitShowText = false; //通常表示するか
    #endregion

    void Start()
    {
        startColor = subjectText.color; //スタート時のカラー保存
        isWaitShowText = false; //待ち時間なし
        StartCoroutine(ShowText(subject, isWaitShowText));
    }

    /// <summary>
    /// 外からShowText関数を呼び出すための関数
    /// </summary>
    /// <param name="advice"></param>
    public void ActiveText(string advice)
    {
        isWaitShowText = true; //待ち時間あり
        StartCoroutine(ShowText(advice, isWaitShowText));
    }

    /// <summary>
    /// テキスト表示コルーチン
    /// </summary>
    /// <param name="tutorial"></param>
    /// <param name="waitShowTextFlag"></param>
    /// <returns></returns>
    IEnumerator ShowText(string tutorial, bool waitShowTextFlag)
    {
        if (waitShowTextFlag == true)
        {
            yield return new WaitForSeconds(waitShowText);
        }

        float elapsed = 0; // 経過時間

        subjectText.gameObject.SetActive(true);

        subjectText.color = startColor;

        subjectText.text = tutorial;

        //3秒間待機
        yield return new WaitForSeconds(duration);

        //フェードアウト
        while (elapsed < fadeOut)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1.0f, 0.0f, elapsed / fadeOut); //時間経過でalphaの値が減少
            subjectText.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        // 最後に非表示にする
        subjectText.gameObject.SetActive(false);
    }
}
