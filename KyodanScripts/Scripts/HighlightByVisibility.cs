using UnityEngine;

/// <summary>
/// 花瓶の強調表示管理
/// </summary>
public class HighlightByVisibility : MonoBehaviour
{
    #region 変数の宣言
    [SerializeField] private Camera fpsCamera; //一人称カメラ
    [SerializeField] private LayerMask obstacleLayer; //壁レイヤー
    [SerializeField] private Material normalMat; //通常マテリアル
    [SerializeField] private Material highlightMat; //強調表示マテリアル

    [SerializeField] private float camViewPoint = 0.5f;

    private Renderer rend; //Rendererを入れる変数
    #endregion

    void Awake()
    {
        rend = GetComponent<Renderer>();
        rend.material = highlightMat; //最初は強調
    }

    void Update()
    {
        //見えているなら
        if (IsDirectlyVisible())
        {
            Debug.Log("見えてる");
            rend.material = normalMat; //マテリアルを通常状態のモノにする
        }
        //見えていないなら
        else
        {
            Debug.Log("見えてない");
            rend.material = highlightMat; //マテリアルを強調表示状態のモノにする
        }
    }

    /// <summary>
    /// カメラに直接見えているか判定する関数
    /// </summary>
    /// <returns></returns>
    bool IsDirectlyVisible()
    {
        //画面中央の、カメラの少し前のワールド座標を変数fromに入れる
        Vector3 from = fpsCamera.ViewportToWorldPoint(
            new Vector3(camViewPoint, camViewPoint, fpsCamera.nearClipPlane)
        );

        Vector3 to = rend.bounds.center; //オブジェクトの見た目の中央座標を変数toに入れる
        Vector3 dir = to - from; //カメラからオブジェクトまでのベクトルを変数dirに入れる
        float dist = dir.magnitude; //ベクトルの長さを変数distに入れる

        //壁レイヤーと自分のレイヤーを変数mask(判定対象)に追加する
        LayerMask mask = obstacleLayer | (1 << gameObject.layer);

        //デバッグ用のRayを出す
        Debug.DrawRay(from, dir, Color.red);

        //カメラからオブジェクトまでmaskを判定しhitに入れるRayを飛ばす
        if (Physics.Raycast(from, dir.normalized, out RaycastHit hit, dist, mask))
        {
            //デバッグ用　Rayに最初に当たったColliderの名前を出す
            Debug.Log("hit object : " + hit.collider.gameObject.name);
            //このオブジェクトの名前を出す
            Debug.Log("this gameobject:" + gameObject.name);

            //最初に当たった物が自分の親オブジェクトならtrue
            return hit.collider.gameObject == transform.parent.gameObject;
        }

        return false;
    }

}
