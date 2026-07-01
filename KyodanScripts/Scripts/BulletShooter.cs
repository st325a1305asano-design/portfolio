using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static Enums;

/// <summary>
/// 弾の発射と生成を管理するスクリプト
/// </summary>
public class BulletShooter : MonoBehaviour
{
    #region 変数の宣言
    [SerializeField] GameObject bulletPrefab; //弾丸のPrefab
    [SerializeField] Transform firePoint; //弾丸の生成位置
    [SerializeField] int ammo = 3; //残弾数
    PlayerInput playerInput; //PlayerInputを入れる変数
    GameObject currentBullet; //現在ある弾丸を入れる変数

    #endregion

    //プロパティ
    public int Ammo { get; private set; }

    void Start()
    {
        //PlayerInputのあるオブジェクトを探してコンポーネント取得
        playerInput = FindAnyObjectByType<PlayerInput>();
        SpawnBullet(); 
        GameManager.Instance.UpdateAmmoText(ammo); //残弾数表示を更新
    }

    void Update()
    {
        //(残弾がある または チュートリアル) かつ　Fireキーを押されたなら
        if ((CanShoot() == true || SceneManager.GetActiveScene().buildIndex == (int)SceneName.Tutorial) 
            && playerInput.actions[InputActionNames.InputActionFire].triggered)
        {
            Fire(); //弾丸を発射
        }
    }

    #region 生成と発射
    /// <summary>
    /// 弾丸の生成をする処理
    /// </summary>
    void SpawnBullet()
    {
        //残弾数が0なら
        if (ammo == 0)
        {
            GameManager.Instance.GameOverProcess();
            return;
        }

        //弾丸を生成してcurrentBulletに入れる
        currentBullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        //弾丸の状態をBulletスクリプトより"停止状態"にする
        currentBullet.GetComponent<Bullet>().Stop();
        //Bulletスクリプトにこのスクリプトを渡す
        currentBullet.GetComponent<Bullet>().SetShooter(this);
        
        Debug.Log("弾が生成されました");

    }

    /// <summary>
    /// 弾丸の発射をする処理
    /// </summary>
    void Fire()
    {
        //弾丸がない状態なら処理を止める
        if (currentBullet == null) return;

        //弾丸の状態を"発射状態"にする
        currentBullet.GetComponent<Bullet>().Launch();
        //currentBulletの中身をnullにする
        currentBullet = null;

        Debug.Log("弾が発射されました");

        if (ammo > 0)
        {
            //残弾数を減らす
            ammo--;

            //残弾数表示を更新
            GameManager.Instance.UpdateAmmoText(ammo);
        }
    }
    #endregion

    /// <summary>
    /// 弾丸が壊れた時に呼び出される
    /// </summary>
    public void OnBulletDestroyed()
    {
        //弾丸を生成する
        SpawnBullet();
    }

    //弾丸を打てるかどうかを確認するメソッド
    public bool CanShoot()
    {
        return ammo > 0;
    }
}
