using System.Collections;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

/// <summary>
/// プレイヤー操作
/// </summary>
public class PlayerController : MonoBehaviour
{
    #region 変数の宣言
    [Header("Movement")]
    [SerializeField] float moveSpeed = 5f; //移動速度
    [SerializeField] float onGroundGravity = -2f; //地面に軽く押しつけるよう
    [SerializeField] private float gravity = -9.81f; //重力の強さ

    [SerializeField] private string interactAction = "Interact";
    float verticalVelocity;
    float mouseSensitivity; //マウス感度

    [Header("Camera")]
    [SerializeField] Transform cameraTransform; //カメラ情報
    [SerializeField] float rayDistance = 3f; //インタラクト距離
    [SerializeField] LayerMask dirtLayer; //汚れレイヤー

    float xRotation = 0f; //x軸回転量
    float xMaxRotation = 90f; //最大回転
    CharacterController characterController; //キャラクター操作コンポーネント
    PlayerInput playerInput; //入力管理コンポーネント
    PlayerStates playerStates; //プレイヤー状態

    enum PlayerStates
    {
        Exploring,  //探索中
        Interacting, //インタラクト中
        Idle, //待機
        Locked
    }
    #endregion

    #region　開始処理
    private void Awake()
    {
        //各種コンポーネント取得
        characterController = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();

        ChangeStates(0); //探索状態化
    }

    void Start()
    {
        xRotation = 0f; //初期回転を無しにする
        transform.localRotation = Quaternion.identity; //回転そのまま
    }
    #endregion

    /// <summary>
    /// ステータス変更 0:Exploring(探索) 1:Intaracting(インタラクト)
    /// </summary>
    /// <param name="state"></param>
    public void ChangeStates(PlayerStates state)
    {
        if (state == PlayerStates.Idle)
        {
            //探索状態にしカーソル非表示
            playerStates = PlayerStates.Exploring;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else if (state == PlayerStates.Locked)
        {
            //インタラクト状態にしカーソル表示
            playerStates = PlayerStates.Interacting;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void Update()
    {
        //ゲームマネージャーインスタンスがなければ処理しない
        if (GameManager.Instance == null) return;

        //探索状態でなければ処理しない
        if (playerStates != PlayerStates.Exploring) return;

        Move(); //移動
        Look(); //視点操作
        DetectDirt(); //インタラクト

        //デバッグ用Ray
        Debug.DrawRay(cameraTransform.position, cameraTransform.forward * rayDistance, Color.red);
    }

    /// <summary>
    /// 移動
    /// </summary>
    private void Move()
    {
        //入力取得
        Vector2 moveValue = playerInput.actions["Move"].ReadValue<Vector2>();
        float moveX = moveValue.x;
        float moveZ = moveValue.y;

        //移動ベクトル算出
        Vector3 move = (transform.right * moveX + transform.forward * moveZ).normalized;

        //重力処理
        if (characterController.isGrounded == true)
        {
            //地面に接地しているときは落下しないように軽く押しつける程度で止める
            if (verticalVelocity < 0)
            {
                verticalVelocity = onGroundGravity;
            }
        }
        else
        {
            //空中なら重力を適用
            verticalVelocity += gravity * Time.deltaTime;
        }

        // 最終的な移動ベクトル
        Vector3 finalMove = (move * moveSpeed + Vector3.up * verticalVelocity) * Time.deltaTime;

        //移動
        characterController.Move(finalMove);
    }

    /// <summary>
    /// 視点操作
    /// </summary>
    void Look()
    {
        //入力取得
        Vector2 lookValue = playerInput.actions["Look"].ReadValue<Vector2>();

        //感度取得
        if (SettingsManager.Instance != null)
        {
            mouseSensitivity = SettingsManager.Instance.sensitivity;
        }

        //フレーム単位化
        float lookX = lookValue.x * mouseSensitivity * Time.deltaTime;
        float lookY = lookValue.y * mouseSensitivity * Time.deltaTime;

        //カメラ反転
        if (SettingsManager.Instance.invertY)
        {
            lookY = -lookY;
        }

        xRotation -= lookY; //カメラ上下順転
        xRotation = Mathf.Clamp(xRotation, -xMaxRotation, xMaxRotation); //上下視点上限

        //視点回転
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0, 0); //上下
        transform.Rotate(Vector3.up * lookX); //左右
    }

    /// <summary>
    /// インタラクト
    /// </summary>
    void DetectDirt()
    {
        //カメラ正面に光線を出す
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);

        //光線が汚れレイヤーに当たったら
        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance))
        {
            Debug.Log(hit.collider.name);

            //インタラクトが押されたら
            if (playerInput.actions[interactAction].WasPressedThisFrame())
            {
                Debug.Log("汚れにインタラクトしました");

                //汚れ判定
                hit.collider.GetComponent<CleanTarget>()?.OnSelected();

                //ドア判定
                hit.collider.GetComponent<Door>()?.ToggleDoor();
            }
        }
    }
}
