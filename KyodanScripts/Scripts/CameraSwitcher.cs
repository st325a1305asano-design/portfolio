using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 一人称視点と俯瞰視点を変更するスクリプト
/// </summary>
public class CameraSwitcher : MonoBehaviour
{
    #region 変数の宣言
    [SerializeField] Camera firstPersonCamera; //一人称視点カメラを入れる変数
    [SerializeField] Camera mapTopViewCamera; //俯瞰視点カメラを入れる変数

    //2階層以上ある場合最下層を除く各階を入れる変数
    [SerializeField] List<GameObject> floor = new List<GameObject>();

    PlayerInput playerInput; //PlayerInputを入れる変数

    bool isTopView = false; //俯瞰視点フラグ
    int currentFloorIndex; //非表示階層
    #endregion

    void Start()
    {
        //このスクリプトがアタッチされているオブジェクトについているPlayerInputを取得
        playerInput = GetComponent<PlayerInput>();

        //一人称にする
        firstPersonCamera.enabled = true;
        mapTopViewCamera.enabled = false;

        currentFloorIndex = floor.Count; //全表示状態に初期化
    }
    
    void Update()
    {
        //ボタン入力を取得
        if (playerInput.actions[InputActionNames.InputActionSwitchCamera].triggered == true)
        {
            //視点変更関数を呼び出す
            ToggleCamera();
        }

        if (floor == null)
        {
            return;
        }

        //俯瞰視点　かつ　ボタン入力がされたら
        if(isTopView && playerInput.actions[InputActionNames.InputActionChangeShownFloor].triggered == true)
        {
            DecreaseVisibleFloor();
        }
    }

    /// <summary>
    /// 視点を切り替える関数
    /// </summary>
    void ToggleCamera()
    {
        //俯瞰視点フラグを反対の状態にする
        isTopView = !isTopView;

        //一人称→俯瞰視点
        if (isTopView)//SelectCamera.Top;
        {
            //俯瞰にする
            firstPersonCamera.enabled = false;
            mapTopViewCamera.enabled = true;

            //Lookの入力を停止
            playerInput.actions[InputActionNames.Look].Disable();
        }
        //俯瞰視点→一人称
        else //SelectCamera.First;
        {
            ShowAllFloor();

            //一人称にする
            firstPersonCamera.enabled= true;
            mapTopViewCamera.enabled= false;

            //Lookの入力を再開
            playerInput.actions[InputActionNames.Look].Enable();
        }
    }

    #region 階層表示非表示
    /// <summary>
    /// 表示対象の階層を1つ下げて表示状態を更新する
    /// </summary>
    void DecreaseVisibleFloor()
    {
        currentFloorIndex--; //表示する階層を一階層減らす

        //2階層目より少なくなったら全階層表示
        if (currentFloorIndex < 0)
        {
            ShowAllFloor();
        }
        else
        {
            UpdateFloorVisibility();
        }
    }

    #region 非表示処理

    /// <summary>
    /// 現在の表示階層に応じて各階層の表示状態を更新する
    /// </summary>
    void UpdateFloorVisibility()
    {
        //現在のインデックスを基準に、上の階を累積して非表示にする
        for (int i = 0; i < floor.Count; i++)
        {
            //ターゲットの階層よりも下かどうか
            bool shouldShow = i < currentFloorIndex;

            //上の階層は非表示にする（当たり判定は残る）
            foreach (Renderer rend in floor[i].GetComponentsInChildren<Renderer>())
            {
                rend.enabled = shouldShow;
            }

            HideMirror(i, shouldShow);
        }
    }

    /// <summary>
    /// 現在の表示階層に応じて鏡の表示状態を更新する
    /// </summary>
    /// <param name="i"></param>
    /// <param name="shouldShow"></param>
    void HideMirror(int i, bool shouldShow)
    {
        //シーン内にあるMirrorFollowをソートせずに取得する
        MirrorFollow[] mirrors = FindObjectsByType<MirrorFollow>(FindObjectsSortMode.None);

        //全てのMirrorFollowを順番に処理する
        foreach (var mirror in mirrors)
        {
            //mirrorのAttachedTargetがnullなら次のループ
            if (mirror.AttachedTarget == null)
                continue;

            //mirrorの追従対象がi番目の階層の子なら
            if (mirror.AttachedTarget.IsChildOf(floor[i].transform))
            {
                //i番目より上の階層は非表示にする（当たり判定は残る）
                foreach (Renderer rend in mirror.GetComponentsInChildren<Renderer>())
                {
                    rend.enabled = shouldShow;
                }
            }
        }
    }

    #endregion

    /// <summary>
    /// 全階層表示
    /// </summary>
    void ShowAllFloor()
    {
        //全ての階層を
        foreach (GameObject floorObj in floor)
        {
            //表示する
            foreach (Renderer rend in floorObj.GetComponentsInChildren<Renderer>())
            {
                rend.enabled = true;
            }
        }

        //シーン内にあるMirrorFollowをソートせずに取得する
        MirrorFollow[] mirrors = FindObjectsByType<MirrorFollow>(FindObjectsSortMode.None);

        //全てのMirrorFollowを順番に処理する
        foreach (var mirror in mirrors)
        {            
            //表示する
            foreach (Renderer rend in mirror.GetComponentsInChildren<Renderer>())
            {
                rend.enabled = true;
            }
        }

        currentFloorIndex = floor.Count; //インデックスを初期状態に戻す
    }
    #endregion
}
