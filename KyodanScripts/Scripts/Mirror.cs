using UnityEngine;

/// <summary>
/// ふれたものを反射させるスクリプト
/// </summary>
public class Mirror : MonoBehaviour
{
    #region 変数の宣言
    [Header("鏡の反射強度(1で完全反射)")]
    [SerializeField] float refrectPower = 1f;
    [SerializeField] float debugRayLength = 2f;

    #endregion

    /// <summary>
    /// 入射ベクトルと法線から反射ベクトルを返す関数
    /// </summary>
    /// <param name="comeDirection"></param>
    /// <param name="surfaceNormal"></param>
    /// <returns></returns>
    public Vector3 GetReflectVectol(Vector3 comeDirection, Vector3 surfaceNormal)
    {
        //地面と平行に反射するように法線の正規化
        Vector3 flatNormal = new Vector3(surfaceNormal.x, 0, surfaceNormal.z).normalized;
        //反射方向の計算
        Vector3 reflectDirection = Vector3.Reflect(comeDirection.normalized, flatNormal);
        //反射鏡度をかける(1未満で減衰反射)
        reflectDirection *= refrectPower;
        
        //デバッグ表示
        Debug.DrawRay(transform.position, reflectDirection * debugRayLength, Color.cyan, 1f);
        
        //reflectDirectionを返り値として返す
        return reflectDirection;
    }
}
