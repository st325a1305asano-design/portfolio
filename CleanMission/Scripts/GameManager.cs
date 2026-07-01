using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Enums;

/// <summary>
/// ゲーム管理
/// </summary>
public class GameManager : MonoBehaviour
{
    #region 変数の宣言
    public static GameManager Instance; //インスタンス

    [Header("Time Settings")]
    [SerializeField] float startTime = 600f; //開始時間
    [SerializeField] float dangerInterval = 25f; //危険洗剤のインターバル

    float remainingTime; //現在時間

    int totalDirts; //合計汚れ数
    int cleanedCount; //正答数
    int cleanedRate = 100;
    int mistakeCount; //ミス数
    int mistakesLimit = * 20; //ミス数＊20

    RoomChemicalState[] roomStates; //各部屋の状態保存用配列

    bool isGameOver = false; //ゲーム終了管理フラグ
    #endregion

    public float GetRemainingTime() => remainingTime; //現在時間のゲッター
    public bool GetIsGameOver() => isGameOver; //ゲーム終了判定フラグのゲッター

    #region 開始処理
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        remainingTime = startTime; //現在時間に開始時間を代入する

        //EnumsスクリプトのRoomTypeの長さを取得
        int roomCount = System.Enum.GetValues(typeof(RoomType)).Length;

        //部屋状態を管理する配列を作成
        roomStates = new RoomChemicalState[roomCount];
        for (int i = 0; i < roomStates.Length; i++)
        {
            //各部屋に状態管理のオブジェクト(スクリプト)を割り当て
            roomStates[i] = new RoomChemicalState();
        }
    }
    #endregion

    void Update()
    {
        if (isGameOver) return; //ゲームが終了しているなら処理を止める

        remainingTime -= Time.deltaTime; //カウントダウン

        //現在時刻が0以下になったら
        if (remainingTime <= 0)
        {
            remainingTime = 0; //現在時間を0にする
            CheckGameClear(); //クリア判定
        }
    }

    /// <summary>
    /// 汚れの登録
    /// </summary>
    public void RegisterDirt()
    {
        totalDirts++; //汚れの数+1
    }

    /// <summary>
    /// 正答処理
    /// </summary>
    public void NotifyCleanSuccess()
    {
        cleanedCount++; //正答数+1
        CheckGameClear(); //クリア判定
    }

    /// <summary>
    /// 誤答処理
    /// </summary>
    public void NotifyMistake()
    {
        mistakeCount++; //誤答数+1
    }

    /// <summary>
    /// 洗剤使用の通知
    /// </summary>
    /// <param name="room"></param>
    /// <param name="detergent"></param>
    public void NotifyDetergentUse(RoomType room, DetergentType detergent)
    {
        //使用した洗剤が塩素系薬品や酸性薬品でない場合処理を止める
        if (detergent != DetergentType.Chlorine && detergent != DetergentType.Acid)
            return;

        //部屋の状態を取得
        RoomChemicalState state = roomStates[(int)room];

        //最後に使用した洗剤が塩素系、これから使用する洗剤が酸性(またはそれぞれが逆)の場合危険フラグを真にする
        bool isDanger =
            (state.lastType == DetergentType.Chlorine && detergent == DetergentType.Acid) ||
            (state.lastType == DetergentType.Acid && detergent == DetergentType.Chlorine);

        //危険フラグが真かつ最後に使用した時間から15秒以内なら
        if (isDanger && Time.time - state.lastUseTime <= dangerInterval)
        {
            //ゲームオーバー処理を呼び出す
            GameOver("塩素系薬品と酸性薬品を混ぜてしまいました");
            return;
        }

        //忠告表示
        FindAnyObjectByType<SubjectUIController>().ActiveText("少しの間この部屋であの洗剤を使わないほうがよさそうだ");

        state.lastType = detergent; //最終使用洗剤を今使用したものに変更
        state.lastUseTime = Time.time; //最終洗剤使用時刻を現在時刻に変更

        roomStates[(int)room] = state; //配列に変更点を記録する
    }

    /// <summary>
    /// クリア処理
    /// </summary>
    void CheckGameClear()
    {
        //正答数が合計汚れ数を超えるか時間切れになったら
        if(cleanedCount >= totalDirts || remainingTime <= 0)
        { 
            isGameOver = true; //ゲーム終了

            // プレイヤー停止
            FindAnyObjectByType<PlayerController>().ChangeStates(1);

            ShowResult(); //結果表示
        }
    }

    /// <summary>
    /// ゲームオーバー処理
    /// </summary>
    /// <param name="reason"></param>
    void GameOver(string reason)
    {
        if (isGameOver) return; //ゲーム終了なら処理しない

        isGameOver = true; //ゲーム終了

        GameOverUIController.Instance.ShowGameOver();

        Debug.Log("GAME OVER:" + reason);
    }

    /// <summary>
    /// リザルト表示
    /// </summary>
    void ShowResult()
    {
        int score = CalculateScore(); //スコア取得

        if(score <= 0)
        {
            score = 0;
        }

        //リザルトUI表示
        ResultUIController.Instance.ShowResult(score, cleanedCount, totalDirts, remainingTime);

        Debug.Log("score:" + score);
    }

    /// <summary>
    /// スコア計算
    /// </summary>
    /// <returns></returns>
    int CalculateScore()
    {
        int baseScore = cleanedCount * cleanedRate; 
        int timeBonus = Mathf.FloorToInt(remainingTime); //残り時間ボーナス(小数切り捨て)
        int penalty = mistakeCount * mistakesLimit; 

        return baseScore + timeBonus - penalty; //最終スコア
    }
}
