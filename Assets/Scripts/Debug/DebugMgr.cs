using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using battleTypes;

public class DebugMgr : MonoBehaviour
{
    // =================変数================
    // インスタンス
    public static DebugMgr instance;

    // デバッグモードフラグ
    public bool m_DebugModeFlag;

    // バトル情報テキスト
    public TextMeshProUGUI m_TextOperateUser;
    public TextMeshProUGUI m_TextTurnNum;
    public TextMeshProUGUI m_TextTurnSide;
    public TextMeshProUGUI m_TextPhase;
    public TextMeshProUGUI m_TextBattleResult;

    // プレイヤー情報テキスト
    public TextMeshProUGUI m_TextGoldValue_P;
    public TextMeshProUGUI m_TextPeopleCardNum_P;
    public TextMeshProUGUI m_TextScienceCardNum_P;
    public TextMeshProUGUI m_TextScienceValue_P;
    public TextMeshProUGUI m_TextMilitaryCardNum_P;
    public TextMeshProUGUI m_TextSpyCardNum_P;

    // 敵情報テキスト
    public TextMeshProUGUI m_TextGoldValue_E;
    public TextMeshProUGUI m_TextPeopleCardNum_E;
    public TextMeshProUGUI m_TextScienceCardNum_E;
    public TextMeshProUGUI m_TextScienceValue_E;
    public TextMeshProUGUI m_TextMilitaryCardNum_E;
    public TextMeshProUGUI m_TextSpyCardNum_E;

    // 更新フラグ
    private bool m_UpdateFlag = false;

    void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        // インスタンス生成
        CreateInstance();
    }

    // Update is called once per frame
    void Update()
    {
        // 更新
        DebugMgrUpdate();
    }

    // =================関数================
    // DebugMgrの更新。
    void DebugMgrUpdate()
    {
        // 更新フラグが立っていないなら処理しない。
        if (!m_UpdateFlag) { return; }

        // ↓更新処理↓
        // バトル情報
        m_TextOperateUser.text = "操作側：" + BattleUserMgr.instance.GetSetOperateUserSide.ToString();
        m_TextTurnNum.text = "ターン数：" + BattleMgr.instance.GetSetTurnNum;
        m_TextTurnSide.text = "ターン側：" + BattleMgr.instance.GetSetTurnSide.ToString();
        m_TextPhase.text = "フェイズ：" + BattleMgr.instance.GetSetPhaseType.ToString();
        m_TextBattleResult.text = "勝敗：" + BattleMgr.instance.GetSetBattleResult.ToString();

        // プレイヤー情報
        m_TextGoldValue_P.text = "Gold：" + BattleMgr.instance.GetGoldValue(Side.eSide_Player).ToString();
        m_TextPeopleCardNum_P.text = "国民カード：" + BattleMgr.instance.GetPeopleCardNum(Side.eSide_Player).ToString();
        m_TextScienceCardNum_P.text = "研究カード：" + BattleMgr.instance.GetScienceCardNum(Side.eSide_Player).ToString();
        m_TextScienceValue_P.text = "研究値：" + BattleMgr.instance.GetScienceValue(Side.eSide_Player).ToString();
        m_TextMilitaryCardNum_P.text = "軍事カード：" + BattleMgr.instance.GetMilitaryCardNum(Side.eSide_Player).ToString();
        m_TextSpyCardNum_P.text = "スパイカード：" + BattleMgr.instance.GetSpyCardNum(Side.eSide_Player).ToString();

        // 敵情報
        m_TextGoldValue_E.text = "Gold：" + BattleMgr.instance.GetGoldValue(Side.eSide_Enemy).ToString();
        m_TextPeopleCardNum_E.text = "国民カード：" + BattleMgr.instance.GetPeopleCardNum(Side.eSide_Enemy).ToString();
        m_TextScienceCardNum_E.text = "研究カード：" + BattleMgr.instance.GetScienceCardNum(Side.eSide_Enemy).ToString();
        m_TextScienceValue_E.text = "研究値：" + BattleMgr.instance.GetScienceValue(Side.eSide_Enemy).ToString();
        m_TextMilitaryCardNum_E.text = "軍事カード：" + BattleMgr.instance.GetMilitaryCardNum(Side.eSide_Enemy).ToString();
        m_TextSpyCardNum_E.text = "スパイカード：" + BattleMgr.instance.GetSpyCardNum(Side.eSide_Enemy).ToString();

        // 更新処理が終わったのでフラグ降ろす。
        if (m_UpdateFlag) { m_UpdateFlag = false; }
    }

    // 更新のリクエスト。(次にUpdateで走る)
    public void UpdateRequest()
    {
        m_UpdateFlag = true;
    }

    // インスタンスを作成
    public bool CreateInstance()
    {
        // 既にインスタンスが作成されていなければ作成する
        if (instance == null)
        {
            // 作成
            instance = this;
        }

        // インスタンスが作成済みなら終了
        if (instance != null) { return true; }

        Debug.LogError("DebugMgrのインスタンスが生成できませんでした");
        return false;
    }

    // ===関数===
    // デバッグモード設定
    public void SetDebugMode(bool _flag)
    {
        m_DebugModeFlag = _flag;
    }
    // デバッグモードか
    public bool IsDebugMode()
    {
        return m_DebugModeFlag;
    }
}
