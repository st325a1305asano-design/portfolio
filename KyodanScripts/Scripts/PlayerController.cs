using UnityEngine;
using UnityEngine.InputSystem;
using static Enums;

/// <summary>
/// プレイヤーのアクションを管理するスクリプト
/// </summary>

//コンポーネントの自動追加と削除防止
[RequireComponent(typeof(PlayerInput), typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    #region 変数の宣言
    PlayerInput playerInput; //PlayerInputを入れる変数

    private Vector2 moveInput;  //入力された移動キーの値を入れる変数
    private Vector3 moveDirection; //プレイヤーの移動方向
    [SerializeField] float speed = 5f; //移動速度

    CharacterController characterController; //CharacterControllerを入れる変数
    [SerializeField] private float gravity = -9.81f; //重力の強さ
    [SerializeField] float onGroundGravity = -2f; //地面に軽く押しつけるよう
    [SerializeField] float jump = 2.0f; //ジャンプしたい高さ
    private float verticalVelocity;   //垂直方向の速度（ジャンプや重力）

    bool isGrounded; //地面に接触しているか

    Animator animator; //三人称のアニメーター
    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int Jump = Animator.StringToHash("Jump");
    private static readonly int IsGrounded = Animator.StringToHash("IsGrounded");

    [SerializeField]
    Animator fpsAnimator; //一人称のアニメーター

    float speedValue; //移動入力値
    #endregion

    void Start()
    {
        //このスクリプトがアタッチされているオブジェクトについているPlayerInputを取得
        playerInput = GetComponent<PlayerInput>();
        //このスクリプトがアタッチされているオブジェクトについているCharacterControllerを取得
        characterController = GetComponent<CharacterController>();
        //このスクリプトがアタッチされているオブジェクトについているAnimatorを取得
        animator = GetComponent<Animator>();
        //playerInputのActionMapをPlayerActionにする
        GameManager.Instance.SwitchActionMaps("PlayerAction");

        if (playerInput == null)
        {
            Debug.Log("PlayerInputが見つかりません");
            return;
        }

        //ゲームステートをPlayingにする
        GameManager.Instance.ChangeGameState(GameState.Playing);
    }

    void Update()
    {
        //プレイ状態じゃないなら処理を止める
        if (!GameManager.Instance.IsPlaying())
        {
            //Playerの動きを止める
            characterController.Move(Vector3.zero);
            return;
        }

        HandleMovement(); //キャラクター移動の処理を呼び出す
        HandleJump(); //キャラクタージャンプの処理を呼び出す
        UpdateAnimation(); //アニメーションの処理を呼び出す
        UpdateFPSAnimation(); //FPSアニメーションの処理を呼び出す

        //Debug.Log("PlayerPos:"+transform.position);
        //Debug.Log(characterController.velocity);
    }

    #region キャラクター操作処理
    /// <summary>
    /// キャラクターの移動処理
    /// </summary>
    void HandleMovement()
    {
        //ボタン入力を取得
        moveInput = playerInput.actions[InputActionNames.InputActionMove].ReadValue<Vector2>();

        //入力方向をカメラ基準(プレイヤーの向き)にする
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;
        moveDirection = (forward * moveInput.y + right * moveInput.x).normalized;

        //接地状態を取得
        isGrounded = characterController.isGrounded;

        //重力処理
        if (isGrounded == true)
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
        Vector3 finalMove = (moveDirection * speed + Vector3.up * verticalVelocity) * Time.deltaTime;

        // CharacterControllerで移動
        characterController.Move(finalMove);
    }

    /// <summary>
    /// キャラクターのジャンプ処理
    /// </summary>
    void HandleJump()
    {
        // ジャンプ入力を取得
        if (playerInput.actions[InputActionNames.InputActionJump].triggered && isGrounded)
        {
            //物理公式(等加速度運動) v = √(2 * g * h)
            //上に飛びたいためgravityに-をかける
            verticalVelocity = Mathf.Sqrt(-2f * gravity * jump);
            Debug.Log("ジャンプ！");
        }
    }
    #endregion

    /// <summary>
    /// アニメーションの実行関数
    /// </summary>
    void UpdateAnimation()
    {
        //移動しているなら
        speedValue = new Vector3(moveDirection.x, 0, moveDirection.z).magnitude;

        //アニメーターの項目にセット
        animator.SetFloat(Speed, speedValue);
        animator.SetBool(IsGrounded, characterController.isGrounded);

        if (playerInput.actions[InputActionNames.InputActionJump].triggered && isGrounded )
            animator.SetTrigger(Jump);
    }

    /// <summary>
    /// FPS用Bodyのアニメーションの実行関数
    /// </summary>
    void UpdateFPSAnimation()
    {
        //移動しているなら
        speedValue = new Vector3(moveDirection.x, 0, moveDirection.z).magnitude;

        //アニメーターの項目にセット
        fpsAnimator.SetFloat(Speed, speedValue);
        fpsAnimator.SetBool(IsGrounded, characterController.isGrounded);

        if (playerInput.actions[InputActionNames.InputActionJump].triggered && isGrounded)
            fpsAnimator.SetTrigger(Jump);
    }
}
