using UnityEngine;
using static Enums;

/// <summary>
/// 部屋状態クラス
/// </summary>
public class RoomChemicalState
{
    [SerializeField]
    private DetergentType lastType = DetergentType.Chlorine; //最終使用洗剤
    private float lastUseTime = -100; //最終洗剤使用時刻

    #region Property
    public DetergentType LastType { get { return lastType; } set { lastType = value; } }
    public float LastUseTime {get { return lastUseTime; } set{ lastUseTime = value; } }

    #endregion
}
