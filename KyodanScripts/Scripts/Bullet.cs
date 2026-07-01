using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Enums;

/// <summary>
/// 弾の進行と破壊のスクリプト
/// </summary>
public class Bullet : MonoBehaviour
{
    #region 変数の宣言
    //弾速
    [SerializeField] float speed = 10f;
    //最大射程
    [SerializeField] float maxDistance = 100f;
    //弾の進行方向
    private Vector3 direction;
    //移動した距離
    private float traveledDistance = 0f;

    //弾発射のフラグ
    private bool isLaunched = false; 
    //発射されたが、途中で止まった場合それは isLaunchedだけど移動中ではない状態になる　この状態をどう表すかは要検討

    BulletShooter bulletShooter;
    #endregion

    /// <summary>
    /// BulletShooterの設定
    /// </summary>
    /// <param name="s"></param>
    public void SetShooter(BulletShooter s)
    {
        bulletShooter = s;
    }

    void Start()
    {    
        direction = transform.forward; //移動方向を正面に設定
    }

    void Update()
    {
        //弾が発射状態じゃない　または　プレイ状態じゃないなら処理を止める
        if (!isLaunched || !GameManager.Instance.IsPlaying()) return;

        MoveBullet();
    }

    #region 弾の状態管理
    /// <summary>
    /// 弾を静止状態にする
    /// </summary>
    public void Stop()
    {
        isLaunched = false; //要検討
    }

    /// <summary>
    /// 弾を発射状態にする
    /// </summary>
    public void Launch()
    {
        isLaunched = true; //要検討
    }
    #endregion

    /// <summary>
    /// BulletShooterに弾がDestroyされたことを通知する
    /// </summary>
    public void NotifyOnBulletDestroyed() 
    {
        //shooterがあるなら
        if (bulletShooter != null)
        {
            bulletShooter.OnBulletDestroyed();
        }
    }

    /// <summary>
    /// 弾を動かす処理
    /// </summary>
    public void MoveBullet()
    {
        //1フレームの移動距離
        float moveDistance = speed * Time.deltaTime;

        //移動した距離が最大距離を超えたら
        if (traveledDistance >= maxDistance)
        {
            //破壊する
            Destroy(gameObject);

            //弾の破壊通知を送る
            NotifyOnBulletDestroyed();
            return;
        }

        //オブジェクトの判定を行う光線を作成
        Ray ray = new Ray(transform.position, direction);

        //一定距離まで伸びる光線が何かが当たったなら
        if (Physics.Raycast(ray, out RaycastHit hit, moveDistance))
        {
            //衝突点まで移動
            transform.position = hit.point;

            //ぶつかった相手の親にMirrorのコンポーネントがあるなら取得する
            Mirror mirror = hit.collider.GetComponentInParent<Mirror>();
            if (mirror != null)
            {
                ChangeDirectionByMirror(mirror, hit);
            }
            //当たったオブジェクトのタグが花瓶なら
            else if (hit.collider.gameObject.CompareTag(GameTags.Vase))
            {
                HitVase(hit);
            }
            //人なら
            else if (hit.collider.gameObject.CompareTag(GameTags.Character))
            {
                HitCharacter();
            }
            else
            {
                //タグがBrokenVaseなら処理を止める
                if (hit.collider.gameObject.CompareTag(GameTags.BrokenVase) || 
                        hit.collider.gameObject.CompareTag(GameTags.Warp)) return;

                //このオブジェクトを壊す
                Destroy(gameObject);

                //弾の破壊通知を送る
                NotifyOnBulletDestroyed();
            }
        }
        //何にも当たらなければ
        else
        {
            //前進
            transform.position += direction * moveDistance;
            traveledDistance += moveDistance;
        }
    }

    #region 弾丸のhit処理
    /// <summary>
    /// 反射方向を取得する処理
    /// </summary>
    /// <param name="mirror"></param>
    /// <param name="hit"></param>
    void ChangeDirectionByMirror(Mirror mirror, RaycastHit hit)
    {
        //反射方向をMirrorスクリプトから取得
        direction = mirror.GetReflectVectol(direction, hit.normal);
        transform.forward = direction;

        //鏡ヒット音再生
        SEManager.Instance.PlaySE(SE.hitMirror);
    }

    /// <summary>
    /// 弾が花瓶に当たった処理
    /// </summary>
    void HitVase(RaycastHit hit)
    {
        Debug.Log("Vaseに触れました");

        //BreakVaseAnimationを取得
        BreakVaseAnimation breakVaseAnimation = hit.collider.gameObject.GetComponentInParent<BreakVaseAnimation>();

        if (breakVaseAnimation != null)
        {
            breakVaseAnimation.Break();
        }

        //破壊SEを再生
        SEManager.Instance.PlaySE(SE.hitVase);

        //チュートリアルの時
        if (SceneManager.GetActiveScene().buildIndex == (int)SceneName.Tutorial)
        {
            //ステージ進行
            TutorialManager.Instance.UpdateStageNumber();
        }
        else
        {
            //クリア画面表示
            GameManager.Instance.ShowClearPanel();
        }
    }

    /// <summary>
    /// 人に当たった時の処理
    /// </summary>
    void HitCharacter()
    {
        //被弾音を再生
        SEManager.Instance.PlaySE(SE.hitCharacter);

        //チュートリアルのとき
        if (SceneManager.GetActiveScene().buildIndex == (int)SceneName.Tutorial)
        {
            GameManager.Instance.ShowGameOverPanel();

            Destroy(gameObject);

            //弾のDestroy通知を送る
            NotifyOnBulletDestroyed();
        }
        else
        {
            Debug.Log("GameOverProcessを呼びます");
            //ゲームオーバーにする
            GameManager.Instance.GameOverProcess();
            Debug.Log("GameOverProcessを呼びました");
        }
    }
    #endregion

    /// <summary>
    /// ワープ時にオブジェクトの正面を変更する処理
    /// </summary>
    /// <param name="warpPoint"></param>
    public void ChangeDirectionByWarp(Transform warpPoint)
    {
        direction = warpPoint.transform.forward;
        transform.forward = direction;
    }
}
