using System.Collections;
using UnityEngine;
using static Enums;

// 汚れオブジェクトに付けるスクリプト
public class Dirt : MonoBehaviour
{
    #region 変数の宣言
    //この汚れの種類
    private DirtType dirtType;

    //正解の洗剤の種類
    private DetergentType correctType;

    //設置されている部屋の種類
    [SerializeField] private RoomType roomType;

    [Header("Dirt")]
    [SerializeField] private Sprite dirtImage; //汚れ画像
    [SerializeField] private Sprite dirtImage2; //汚れ画像2

    [SerializeField] private bool hasSecondImage; //二つ目の画像を持っているか

    [Header("Cleanig")]
    [SerializeField] private Sprite cleaningImage; //掃除中の画像

    [Header("Cleaned")]
    [SerializeField] private Sprite cleanedImage; //掃除後の画像
    [SerializeField] private Sprite cleanedImage2; //掃除後の画像

    [Header("DirtEffect")]
    [SerializeField] GameObject effect; //汚れエフェクト

    bool isCleaned = false; // 掃除されたかどうか
    float wait = 8f; //待ち時間
    string dirtJP; //日本語化した汚れの種類を入れる変数
    #endregion

    //掃除済みかを返すゲッター
    public bool IsCleaned() => isCleaned;

    private void Start()
    {
        //汚れ登録
        GameManager.Instance.RegisterDirt();
    }

    /// <summary>
    /// 正しい洗剤が使われた時
    /// </summary>
    public void Clean()
    {
        // すでに掃除済みなら何もしない
        if (isCleaned) return;

        isCleaned = true; //掃除済みにする

        StartCoroutine(WaitNotice(true));

        // デバッグログ
        Debug.Log("汚れを掃除しました");
    }

    /// <summary>
    /// 間違った洗剤を使った時
    /// </summary>
    public void Fail()
    {
        StartCoroutine(WaitNotice(false));

        Debug.Log("洗剤が違います！");
    }

    /// <summary>
    /// 結果判定
    /// </summary>
    /// <param name="success"></param>
    /// <returns></returns>
    IEnumerator WaitNotice(bool success)
    {
        yield return new WaitForSeconds(wait); //8秒待つ

        if(success == true)
        {
            //正解を記録
            GameManager.Instance.NotifyCleanSuccess();


            if (effect != null)
            {
                effect.SetActive(false);
            }
        }
        else
        {
            //ミスを記録
            GameManager.Instance.NotifyMistake();
        }
    }

    /// <summary>
    /// インタラクト時に呼ばれる処理
    /// </summary>
    public void OnSelected()
    {
        //クイズ開始
        QuizUIController.Instance.ShowQuiz(this);
    }

    /// <summary>
    /// 汚れ変数を日本語に変換
    /// </summary>
    /// <returns></returns>
    public string SwitchDirtTypeToJP()
    {
        switch (dirtType)
        {
            // カビ
            case DirtType.Mold:
                dirtJP = "カビ";
                break;

            // 尿石
            case DirtType.UrineStone:
                dirtJP = "尿石";
                break;

            //水垢
            case DirtType.WaterScale:
                dirtJP = "水垢";
                break;

            // ワックス
            case DirtType.WAX:
                dirtJP = "ワックス";
                break;

            // 油汚れ
            case DirtType.Oil:
                dirtJP = "油汚れ";
                break;

            // タバコ・ヤニ
            case DirtType.TobaccoTar:
                dirtJP = "タバコ・ヤニ汚れ";
                break;

            // シール
            case DirtType.Seal:
                dirtJP = "シール粘着汚れ";
                break;

            // 錆
            case DirtType.Rust:
                dirtJP = "錆";
                break;

            // 皮脂
            case DirtType.SebumStains:
                dirtJP = "皮脂汚れ";
                break;

            // その他の汚れ(カーペット)
            case DirtType.CarpetStain:
                dirtJP = "繊維に浸透した汚れ";
                break;

            // その他の汚れ(エアコン)
            case DirtType.AirconStain:
                dirtJP = "エアコン内部の殺菌";
                break;
        }

        return dirtJP;
    }
}