using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 鏡のプレビュー機能
/// </summary>
public class MirrorPreview : MonoBehaviour
{
    #region 変数の宣言
    [SerializeField] GameObject previewPrefab; //プレビュー用鏡を入れる変数
    [SerializeField] GameObject wallPreviewPrefab; //プレビュー用壁掛け鏡を入れる変数
    [SerializeField] LayerMask groundLayer; //床レイヤー
    [SerializeField] LayerMask wallLayer; //壁レイヤー
    [SerializeField] Vector3 wallMirrorRotation; //壁掛け鏡の角度調整用
    [SerializeField] Vector2 offset; //プレビュー位置調整用

    PlayerInput playerInput; //PlayerInputを入れる変数
    Camera mainCamera; //MainCameraを入れる変数
    GameObject previewObj; //プレビューしている鏡を入れる変数

    [SerializeField] float rotateSpeed = 5f; //回転速度
    [SerializeField] float rayDistance = 50f; //光線距離

    CreateModeManager createModeManager; //CreateModeManagerを入れる変数

    Vector3 lastPos; //プレビューの最終位置を入れる関数
    Quaternion lastRot; //プレビューの最終回転を入れる関数
    bool isWall; //壁レイヤー判定フラグ

    const float Half = 0.5f; //中央座標を計算するための半分値
    const float DeadZone = 0.01f; //閾値

    [SerializeField] float debugRayLength = 2f;

    #endregion

    #region ゲッター
    public Vector3 GetPlacePosition() => lastPos;
    public Quaternion GetPlaceRotation() => lastRot;
    public RaycastHit LastHit { get; private set; }
    public bool IsWall => isWall;
    #endregion

    void Start()
    {
        //このスクリプトがアタッチされているオブジェクトについているPlayerInputを取得
        playerInput = GetComponent<PlayerInput>();
        //mainCameraにMainCamera(一人称視点カメラ)を取得
        mainCamera = Camera.main;
        //CreateModeManagerを取得
        createModeManager = GetComponent<CreateModeManager>();
    }

    void Update()
    {
        //CreateModeじゃないなら処理を止める
        if (!createModeManager.IsCreateMode)
        {
            //プレビューオブジェクトがあるなら破壊
            if (previewObj != null) DestroyPreview();

            return;
        }

        //プレビュー表示をする関数を呼び出す
        UpdatePreview();
    }

    #region プレビュー表示
    /// <summary>
    /// プレビュー表示をする関数
    /// </summary>
    void UpdatePreview()
    {
        //画面中央位置を取得
        Vector2 screenCenter = new Vector2(Screen.width * Half, Screen.height * Half);

        //画面中央から光線を出す
        Ray baseRay = mainCamera.ScreenPointToRay(screenCenter);

        //床＋壁をまとめる
        int placeLayerMask = groundLayer | wallLayer;

        //光線が床または壁レイヤーに当たったなら
        if (Physics.Raycast(baseRay, out RaycastHit hit, rayDistance, placeLayerMask))
        {

            //Debug.Log($"isWall={isWall}");
            //Debug.Log($"baseHit={hit.point}");

            //ビット演算による壁レイヤーの判定
            isWall = ((1 << hit.collider.gameObject.layer) & wallLayer) != 0;

            //地面レイヤー判定ならオフセット
            if (!isWall)
            {
                screenCenter += offset;
            }
        }
        //床、壁レイヤー以外なら
        else
        {
            //プレビューを表示しない
            if (previewObj != null) DestroyPreview();
            return;
        }

        //オフセットを考慮した位置から光線を出す
        Ray ray = mainCamera.ScreenPointToRay(screenCenter);

        //光線が床または壁レイヤーに当たったなら
        if(Physics.Raycast(ray, out hit, rayDistance, placeLayerMask))
        {
            //Debug.Log($"finalHit={hit.point}");

            lastPos = hit.point; //lastPosに当たった地点を入れる

            UpdatePreviewObject(hit);

            //lastRotにプレビューの回転値を入れる
            lastRot = previewObj.transform.rotation;

            //LastHitに当たったオブジェクトの情報を入れる
            LastHit = hit;
        }
        else
        {
            if (previewObj != null) DestroyPreview();
            return;
        }

        //Debug.Log(hit.point);
        Debug.DrawRay(ray.origin, ray.direction * debugRayLength, Color.red);
        //Debug.Log(screenCenter);
        Debug.Log($"CamPos={mainCamera.transform.position}");

        //Debug.Log($"CamRot={mainCamera.transform.eulerAngles}");
    }

    #region hit処理

    /// <summary>
    /// プレビューオブジェクトの更新
    /// </summary>
    /// <param name="hit"></param>
    void UpdatePreviewObject(RaycastHit hit)
    {
        //プレビューオブジェクトがあるなら
        if (previewObj != null)
        {
            CheckPreviewType();
        }
        //プレビューオブジェクトがnullなら
        else
        {
            //壁フラグに合わせてプレビューする
            previewObj = Instantiate(isWall ? wallPreviewPrefab : previewPrefab);
        }

        //previewObjの座標を当たった座標にする
        previewObj.transform.position = lastPos;

        //壁用は向きを合わせる
        if (isWall)
        {
            previewObj.transform.rotation = Quaternion.LookRotation(hit.normal) * Quaternion.Euler(wallMirrorRotation);
        }
        //地面用なら
        else
        {
            //回転できるようにする
            RotationPreview();
        }
    }

    /// <summary>
    /// 壁レイヤーとプレビューミラー(壁用か)のフラグを比較する
    /// </summary>
    void CheckPreviewType()
    {
        //プレビューオブジェクトのタグを比較
        bool isWallPreview = previewObj.CompareTag("WallMirrorPreview");

        //壁レイヤーフラグと壁掛け鏡フラグが違うなら
        if (isWall != isWallPreview)
        {
            //プレビューオブジェクトを破壊
            DestroyPreview();
        }
    }

    #endregion

    /// <summary>
    /// プレビューの回転処理
    /// </summary>
    void RotationPreview()
    {
        //回転量を取得
        float rotateInput = playerInput.actions["RotateMirror"].ReadValue<float>();

        //Debug.Log(rotateInput);

        //回転量の絶対値が0.01より大きいなら
        if (Mathf.Abs(rotateInput) > DeadZone)
        {
            //previewObjを回転
            previewObj.transform.Rotate(0, rotateInput * rotateSpeed, 0);
        }
    }
    #endregion

    /// <summary>
    /// 破壊処理
    /// </summary>
    void DestroyPreview()
    {
        if (previewObj == null) return;

        Destroy(previewObj);
        previewObj = null;
    }
}
