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

    // バトル情報テキスト
    public TextMeshProUGUI m_TextTurnNum;
    public TextMeshProUGUI m_TextTurnSide;
    public TextMeshProUGUI m_TextPhase;

    // プレイヤー情報テキスト
    public TextMeshProUGUI m_TextPeople_Player;
    public TextMeshProUGUI m_TextScience_Player;
    public TextMeshProUGUI m_TextMilitary_Player;
    public TextMeshProUGUI m_TextSpy_Player;

    // 敵情報テキスト
    public TextMeshProUGUI m_TextPeople_Enemy;
    public TextMeshProUGUI m_TextScience_Enemy;
    public TextMeshProUGUI m_TextMilitary_Enemy;
    public TextMeshProUGUI m_TextSpy_Enemy;

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
        m_TextTurnNum.text = "ターン数：" + BattleMgr.instance.GetTurnNum();
        m_TextTurnSide.text = "ターン側：" + BattleMgr.instance.GetTurnSide().ToString();
        m_TextPhase.text = "フェイズ：" + BattleMgr.instance.GetPhaseType().ToString();

        // プレイヤー情報
        m_TextPeople_Player.text = "国民：" + BattleMgr.instance.GetPeopleValue(Side.eSide_Player).ToString();
        m_TextScience_Player.text = "研究：" + BattleMgr.instance.GetScienceValue(Side.eSide_Player).ToString();
        m_TextMilitary_Player.text = "軍事：" + BattleMgr.instance.GetMilitaryValue(Side.eSide_Player).ToString();
        m_TextSpy_Player.text = "スパイ：" + BattleMgr.instance.GetSpyValue(Side.eSide_Player).ToString();

        // 敵情報
        m_TextPeople_Enemy.text = "国民：" + BattleMgr.instance.GetPeopleValue(Side.eSide_Enemy).ToString();
        m_TextScience_Enemy.text = "研究：" + BattleMgr.instance.GetScienceValue(Side.eSide_Enemy).ToString();
        m_TextMilitary_Enemy.text = "軍事：" + BattleMgr.instance.GetMilitaryValue(Side.eSide_Enemy).ToString();
        m_TextSpy_Enemy.text = "スパイ：" + BattleMgr.instance.GetSpyValue(Side.eSide_Enemy).ToString();

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
}
