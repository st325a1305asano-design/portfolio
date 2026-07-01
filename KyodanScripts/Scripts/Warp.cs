using System.Collections;
using UnityEngine;

/// <summary>
/// 触れたものをワープさせるスクリプト
/// </summary>
public class Warp : MonoBehaviour
{
    #region 変数の宣言

    [SerializeField] Transform warpPoint; //ワープ地点
    [SerializeField] GameObject player; //プレイヤー

    readonly float playerWait = 0.1f;//移動時のプレイヤー待機時間

    #endregion

    //ワープポイントのゲッター
    public Transform GetWarpPoint() => warpPoint;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("ゲートに触れました");

        //触れたオブジェクトのタグがBulletなら
        if (other.gameObject.CompareTag(GameTags.Bullet))
        {
            WarpProcess(other);
            Debug.Log("ワープさせます");
        }
        //触れたオブジェクトのタグがPlayerなら
        else if (other.gameObject.CompareTag(GameTags.Player))
        {
            StartCoroutine(MovePlayerPos(other));
        }
        else
        {
            Debug.Log("ワープ対象ではありません");
        }

            Debug.Log("処理終了");
    }

    /// <summary>
    /// ワープ処理
    /// </summary>
    /// <param name="other"></param>
    void WarpProcess(Collider other)
    {
        other.gameObject.transform.position = warpPoint.position; //触れたものをワープ

        Vector3 warpedRot = other.gameObject.transform.eulerAngles; //オブジェクトの回転を保存

        warpedRot.y = warpPoint.eulerAngles.y; //ワープ地点の正面を向くようにy軸を回転させる

        other.gameObject.transform.rotation = Quaternion.Euler(warpedRot); //回転させる

        //タグがBulletのとき
        if(other.gameObject.CompareTag(GameTags.Bullet))
        {
            //正面を変更する処理を呼ぶ
            other.GetComponent<Bullet>().ChangeDirectionByWarp(warpPoint);
        }
    }

    /// <summary>
    /// プレイヤーの移動処理
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    IEnumerator MovePlayerPos(Collider other)
    {
        //キャラクターコントローラーを無効にする
        player.GetComponent<CharacterController>().enabled = false;

        WarpProcess(other);

        yield return new WaitForSeconds(playerWait); //ディレイを入れる

        //キャラクターコントローラーを有効にする
        player.GetComponent<CharacterController>().enabled = true;
    }
}
