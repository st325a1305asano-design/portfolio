using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// ゲームオーバーUI管理
/// </summary>
public class GameOverUIController : MonoBehaviour
{
    #region 変数の宣言
    public static GameOverUIController Instance; //インスタンス化

    [SerializeField] GameObject gameOverPanel; //ゲームオーバーUIのパネル

    [SerializeField] TextMeshProUGUI gameOverText; //ゲームオーバーテキスト
    [SerializeField] TextMeshProUGUI explainText; //原因説明テキスト
    [SerializeField] string explanationText = "塩素系薬品と酸性薬品を混ぜると有毒ガスが発生します\n" +
                            "同じものに使う場合や狭い部屋で使用する場合は\n" +
                            "薬品をしっかり洗い流し換気しましょう！\n" +
                            "このゲームでは15秒ほど時間を空けることで\n 次の薬品が使用できます";

    [SerializeField] Image image; //キャラ表示用Image
    [SerializeField] Sprite mascot; //説明してくれるマスコットキャラ

    [SerializeField] Vector3 imagePos = new Vector3(750, 83, 0); //キャラクターを置く位置
    #endregion

    private void Awake()
    {
        //Instanceが空なら
        if (Instance == null)
            Instance = this; //これを入れる
        //あるなら
        else
            Destroy(gameObject);//これを消す
    }

    void Start()
    {
        //パネル非表示
        gameOverPanel.SetActive(false);
    }

    /// <summary>
    /// UIのゲームオーバー処理
    /// </summary>
    public void ShowGameOver()
    {
        //パネル表示
        gameOverPanel.SetActive(true);

        //キャラの位置調整
        image.transform.localPosition = imagePos;
        image.sprite = mascot;

        //ゲームオーバーの強調
        gameOverText.text = "</B>GAMEOVER...!</B>";
        gameOverText.color = Color.red;

        //説明の表示
        explainText.color = Color.black;

        explainText.text = explanationText;
    }
}
