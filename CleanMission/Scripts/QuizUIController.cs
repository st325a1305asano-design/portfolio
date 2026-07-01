using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static Enums;
using Unity.VisualScripting;
using System.Collections.Generic;

/// <summary>
/// クイズパネルのUI管理
/// </summary>
public class QuizUIController : MonoBehaviour
{
    #region 変数の宣言
    public static QuizUIController Instance; //インスタンス

    [Header("Panel")]
    [SerializeField] GameObject panel; //パネル
    [SerializeField] Image dirtImage1;  //汚れ画像1
    [SerializeField] Image dirtImage2; //汚れ画像2

    [Header("UI")]
    [SerializeField] Slider gauge; //掃除中ゲージ
    [SerializeField] TextMeshProUGUI resultText; //問題・結果表示テキスト

    [Header("Settings")]
    [SerializeField] float cleaningTime = 5f; //掃除にかかる時間

    Dirt currentDirt; //インタラクト中の汚れ
    bool isCleaning = false; //掃除中
    bool isPlayingQuiz = false; //クイズ中

    [SerializeField]
    Vector3 imagePos1 = new Vector3 (0, 55, 0); //汚れ画像が一枚の時の画像位置
    [SerializeField]
    Vector3 imagePos2 = new Vector3(320, 55, 0); //汚れ画像が二枚の時の画像位置

    float wait = 3f; //結果表示時間

    [SerializeField] GameObject[] quizButtonsGO; //クイズボタンを入れる配列
    #endregion

    enum DirtImageState
{
    BeforeCleaning, // 掃除前
    Cleaning,       // 掃除中
    Cleaned         // 掃除後
}

    //クイズ中かのゲッター
    public bool IsPlayingQuiz => isPlayingQuiz;

    private void Awake()
    {
        Instance = this; //インスタンス化

        panel.SetActive(false); //パネルの非表示
    }

    /// <summary>
    /// クイズ開始処理
    /// </summary>
    /// <param name="dirt"></param>
    // クイズ開始
    public void ShowQuiz(Dirt dirt)
    {
        currentDirt = dirt; //汚れを代入

        panel.SetActive(true); //パネル表示

        isPlayingQuiz = true; //クイズ中

        gauge.gameObject.SetActive(false); //ゲージ非表示

        ShowImage(DirtImageState.BeforeCleaning, currentDirt.hasSecondImage); //画像表示

        ActiveQuizButton(true); //クイズボタン表示

        resultText.text = $"{currentDirt.SwitchDirtTypeToJP()}に適した洗剤を選ぼう"; //問題文
        gauge.value = 0; //ゲージを0にする

        // プレイヤー停止
        FindAnyObjectByType<PlayerController>().ChangeStates(1);
    }

    /// <summary>
    /// 正誤判定
    /// </summary>
    /// <param name="type"></param>
    // ボタンから呼ぶ
    public void OnClickDetergent(int type)
    {
        if (isCleaning) return; //掃除中なら処理を止める

        DetergentType detergent = (DetergentType)type; //int型からenum型の洗剤に変換

        //デバッグ用
        Debug.Log(detergent.ToString());

        //危険判定通知
        GameManager.Instance.NotifyDetergentUse(currentDirt.roomType, detergent);

        //ゲームオーバーなら終了
        if (GameManager.Instance.GetIsGameOver())
        {
            Close(); // UI閉じる
            return;
        }

        //インタラクト中の汚れの適した洗剤と選択した洗剤があっているか
        bool success = (currentDirt.correctType == detergent);

        //判定処理
        FindAnyObjectByType<CleaningSystem>().UseDetergent(currentDirt, detergent);

        ActiveQuizButton(false); //クイズボタン非表示

        //掃除中表示
        StartCoroutine(CleaningProcess(success));
    }

    /// <summary>
    /// 掃除中処理
    /// </summary>
    /// <param name="success"></param>
    /// <returns></returns>
    IEnumerator CleaningProcess(bool success)
    {
        //ゲームオーバーなら終了
        if (GameManager.Instance.GetIsGameOver())
            yield break;

        isCleaning = true; //掃除中
        resultText.text = "掃除中..."; //掃除中表示

        ShowImage(DirtImageState.Cleaning, currentDirt.hasSecondImage); //画像表示

        gauge.gameObject.SetActive(true); //ゲージ表示

        gauge.enabled = true; //ゲージを操作可能に

        float time = 0; //カウントアップタイマー

        //タイマーが掃除時間より少ない間繰り返す
        while (time < cleaningTime)
        {
            time += Time.deltaTime; //タイマー増加
            gauge.value = time / cleaningTime; //ゲージ反映
            yield return null;
        }

        gauge.enabled = false; //ゲージを操作不可能に

        // 結果表示
        if (success)
        {
            resultText.text = "掃除成功！"; //成功時のテキスト
            ShowImage(DirtImageState.Cleaned, currentDirt.hasSecondImage); //画像表示

            SEManager.Instance.SEPlay(SEManager.SEType.Success); //成功した効果音再生
        }
        else
        {
            resultText.text = "もう一度挑戦しよう"; //失敗時のテキスト
            ShowImage(DirtImageState.BeforeCleaning, currentDirt.hasSecondImage); //画像表示

            SEManager.Instance.SEPlay(SEManager.SEType.Failure); //失敗した効果音再生
        }


        yield return new WaitForSeconds(wait); //3秒待つ

        Close(); //クイズ終了
    }

    /// <summary>
    /// クイズ終了処理
    /// </summary>
    void Close()
    {
        panel.SetActive(false); //パネル非表示
        isCleaning = false; //非掃除中
        isPlayingQuiz = false; //クイズ終了

        //ゲームが終了してなければ
        if (!GameManager.Instance.GetIsGameOver())
        {
            //プレイヤー復帰
            PlayerController pc =  FindAnyObjectByType<PlayerController>();
            if(pc != null)
            {
                pc.ChangeStates(PlayerController.ChangeStates(PlayerController.PlayerStates.Idle));
            }
        }
    }

    /// <summary>
    /// 画像表示 掃除前 掃除中 掃除後
    /// </summary>
    /// <param name="state"></param>
    /// <param name="hasSecondImage"></param>
    void ShowImage(DirtImageState state, bool hasSecondImage)
    {
        //掃除前なら
        if (state == DirtImageState.BeforeCleaning)
        {
            dirtImage1.sprite = currentDirt.dirtImage; //画像表示

            //汚れ画像が二枚あるとき
            if (currentDirt.hasSecondImage)
            {
                //画像位置調整
                dirtImage1.transform.localPosition = new Vector3(-imagePos2.x, imagePos2.y);
                dirtImage2.transform.localPosition = imagePos2;

                dirtImage2.gameObject.SetActive(true); //二枚目の画像表示
                dirtImage2.sprite = currentDirt.dirtImage2; //表示する画像を汚れから取ってくる
            }
            else
            {
                //画像位置調整
                dirtImage1.transform.localPosition = imagePos1;

                dirtImage2.gameObject.SetActive(false); //画像非表示
            }
        }
        //掃除中なら
        else if (state == DirtImageState.Cleaning)
        {
            dirtImage1.transform.localPosition = imagePos1; //画像位置調整

            dirtImage2.gameObject.SetActive(false); //画像非表示

            dirtImage1.sprite = currentDirt.cleaningImage; //掃除中画像表示
        }
        //掃除後なら
        else if (state == DirtImageState.Cleaned)
        {
            dirtImage1.sprite = currentDirt.cleanedImage; //画像表示

            //汚れ画像が二枚あるとき
            if (currentDirt.hasSecondImage)
            {
                //画像位置調整
                dirtImage1.transform.localPosition = new Vector3(-imagePos2.x, imagePos2.y);
                dirtImage2.transform.localPosition = imagePos2;

                dirtImage2.gameObject.SetActive(true); //二枚目の画像表示
                dirtImage2.sprite = currentDirt.cleanedImage2; //表示する画像を汚れから取ってくる
            }
            else
            {
                //画像位置調整
                dirtImage1.transform.localPosition = imagePos1;

                dirtImage2.gameObject.SetActive(false); //画像非表示
            }
        }
    }

    /// <summary>
    /// クイズボタンの表示管理
    /// </summary>
    /// <param name="flag"></param>
    public void ActiveQuizButton(bool flag)
    {
        //配列の中身が空ならデバッグログに表示
        if (quizButtonsGO == null) Debug.Log("クイズボタンが配列に登録されていません");

        //配列の中身があるなら
        if (quizButtonsGO.Length > 0)
        {
            //配列にあるクイズボタンを一つの変数のまとめる
            foreach (GameObject qbGO in quizButtonsGO)
            {
                qbGO.SetActive(flag); //ボタンの表示切替
            }
        }
        else
        {
            //デバッグ用
            Debug.Log("quizButtonsの中身０");
        }
    }
}