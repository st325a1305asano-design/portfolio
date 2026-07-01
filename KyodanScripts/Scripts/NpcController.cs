using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

/// <summary>
/// NPCの管理
/// </summary>
public class NpcController : MonoBehaviour
{
    #region　変数の宣言
    //巡回ルートのポイントを入れるリスト
    [SerializeField] List<Transform> routePoint = new List<Transform>();
    [SerializeField] NpcState state; //ステータス

    private NavMeshAgent agent; //NavMeshAgentを入れる変数
    private int currentPointIndex; //現在の巡回地点

    private Animator animator; //Animationを入れる変数
    static readonly int Idle = Animator.StringToHash("Idle");
    static readonly int Walk = Animator.StringToHash("Walk");
    static readonly int Call = Animator.StringToHash("Call");
    static readonly int Talk = Animator.StringToHash("Talk");
    static readonly int Look = Animator.StringToHash("Look");

    /// <summary>
    /// NPCのステータス
    /// </summary>
    enum NpcState
    {
        Idle = 0,
        Walk = 1,
        Call = 2,
        Talk = 3,
        Look = 4
    }
    #endregion

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        //巡回地点があるなら
        if(routePoint.Count > 0)
        {
            //0番目の地点を目的地に設定する
            agent.SetDestination(routePoint[0].position);
        }

        SetAnimation(state);
    }

    void Update()
    {
        //巡回地点がないなら処理を止める
        if(routePoint.Count == 0)
        {
            return;
        }

        //目的地までの経路計算中なら処理を止める
        if (agent.pathPending)
        {
            return;
        }

        Patrol();
    }

    /// <summary>
    /// 巡回処理
    /// </summary>
    void Patrol()
    {
        //停止距離より残りの距離が少なくなったら
        if(agent.remainingDistance <= agent.stoppingDistance)
        {
            //巡回地点を更新する（リストの要素数と同じになったら最初の地点に戻る）
            currentPointIndex = (currentPointIndex + 1) % routePoint.Count;

            //目的地更新
            agent.SetDestination(routePoint[currentPointIndex].position);
        }
    }

    /// <summary>
    /// アニメーションを設定
    /// </summary>
    void SetAnimation(NpcState npcState)
    {
        if (npcState == NpcState.Idle)
        {
            animator.SetTrigger(Idle);
        }
        else if(npcState == NpcState.Walk)
        {
            animator.SetTrigger(Walk);
        }
        else if(npcState == NpcState.Call)
        {
            animator.SetTrigger(Call);
        }
        else if(npcState == NpcState.Talk)
        {
            animator.SetTrigger(Talk);
        }
        else if(npcState == NpcState.Look)
        {
            animator.SetTrigger(Look);
        }
    }
}
