using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using battleTypes;
using Cysharp.Threading.Tasks;

public class BattleMgr : MonoBehaviour
{
    // =================クラス================

    // =================構造体================

    // =================変数================
    // インスタンス
    public static BattleMgr instance;
    // 現在のターン側
    [SerializeField]
    private Side m_TurnSide;
    // 現在のターン数
    [SerializeField]
    private int m_TurnNum;
    // 現在のフェイズ
    [SerializeField]
    private PhaseType m_Phase;
    private PhaseType m_NextPhase;
    // 国民数
    [SerializeField]
    private int[] m_PeopleValue = new int[(int)Side.eSide_Max];
    // 研究数
    [SerializeField]
    private int[] m_ScienceValue = new int[(int)Side.eSide_Max];
    // 軍事数
    [SerializeField]
    private int[] m_MilitaryValue = new int[(int)Side.eSide_Max];
    // スパイ数
    [SerializeField]
    private int[] m_SpyValue = new int[(int)Side.eSide_Max];

    // 更新フラグ
    private bool m_UpdateFlag = false;
    // フェイズ進行フラグ
    private bool m_NextPhaseFlag = false;
    // ターンエンドフラグ
    private bool m_TurnEndFlag = false;

    // フェイズオブジェクト
    public GameObject m_PhaseObject;

    private void Awake()
    {
        CreateInstance();
    }

    // Start is called before the first frame update
    void Start()
    {
        // スタートフェイズから始める
        SetPhase(PhaseType.ePhaseType_Start);
        // フェイズループ処理
        PhaseLoop().Forget();
    }

    // Update is called once per frame
    void Update()
    {
        BattleMgrUpdate();

        switch (m_Phase)
        {
            case PhaseType.ePhaseType_Start:
                break;
            case PhaseType.ePhaseType_Join:
                break;
            case PhaseType.ePhaseType_Main:
                break;
            case PhaseType.ePhaseType_End:
                break;
            default:
                break;

        }
    }

    // =================関数================
    // BattleMgrの更新。
    void BattleMgrUpdate()
    {
        // 更新フラグが立っていないなら処理しない。
        if (!m_UpdateFlag) { return; }

        // ↓更新処理↓
        for (int i = 0; i < (int)Side.eSide_Max; i++)
        {
            Side side = (Side)i;

            // 国民数計算
            int peopleNum = BattleCardMgr.instance.GetCardNumFromKind(side, BattleCard.Kind.eKind_People);
            SetPeopleValue(side, peopleNum);
            // 研究数計算
            int scienceNum = BattleCardMgr.instance.GetCardNumFromAppendKind(side, BattleCard.AppendKind.eAppendKind_Science);
            SetScienceValue(side, scienceNum);
            // 軍事数計算
            int militaryNum = BattleCardMgr.instance.GetCardNumFromAppendKind(side, BattleCard.AppendKind.eAppendKind_Military);
            SetMilitaryValue(side, militaryNum);
            // スパイ数計算
            int spyNum = BattleCardMgr.instance.GetCardNumFromAppendKind(side, BattleCard.AppendKind.eAppendKind_Spy);
            SetSpyValue(side, spyNum);
        }

        // DebugMgrの更新リクエスト
        DebugMgr.instance.UpdateRequest();

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

        Debug.LogError("BattleMgrのインスタンスが生成できませんでした");
        return false;
    }
    // --システム--
    async UniTask PhaseLoop()
    {
        Debug.Log("PhaseLoop起動");
        while(true)
        {
            Debug.Log("フェイズ更新！！");
            // FightMgrの更新リクエスト
            UpdateRequest();
            // 次のフェイズ(ここでは現在のフェイズを代入)
            PhaseType nextPhase = GetPhaseType();
            // 次のフェイズに移動するフラグ初期化
            m_NextPhaseFlag = false;

            switch(m_Phase)
            {
                case PhaseType.ePhaseType_Start:
                    Debug.Log("スタートフェイズ");
                    nextPhase = await PhaseStart();
                    Debug.Log("次のフェイズへ");
                    break;
                case PhaseType.ePhaseType_Join:
                    Debug.Log("ジョインフェイズ");
                    nextPhase = await PhaseJoin();
                    Debug.Log("次のフェイズへ");
                    break;
                case PhaseType.ePhaseType_Main:
                    Debug.Log("メインフェイズ");
                    nextPhase = await PhaseMain();
                    Debug.Log("次のフェイズへ");
                    break;
                case PhaseType.ePhaseType_End:
                    Debug.Log("エンドフェイズ");
                    nextPhase = await PhaseEnd();
                    Debug.Log("次のフェイズへ");
                    break;
                default:
                    Debug.Log("PhaseLoopに記載されていないフェイズに遷移しようとしています");
                    break;
            }

            // 次のフェイズが現在と同じならはじく
            if(nextPhase == m_Phase) { continue; }

            // 次のフェイズを設定
            SetPhase(nextPhase);


        }
    }
    // スタートフェイズ
    async UniTask<PhaseType> PhaseStart()
    {
        // ターン数カウント
        m_TurnNum++;

        // フェイズオブジェクト設定
        m_PhaseObject.AddComponent<StartPhase>();

        await UniTask.WaitUntil(() => IsNextPhaseFlag());

        // フェイズオブジェクト削除
        Destroy(m_PhaseObject.GetComponent<StartPhase>());

        // 次のフェイズへ
        return GetNextPhaseType();
    }
    // ジョインフェイズ
    async UniTask<PhaseType> PhaseJoin()
    {
        // フェイズオブジェクト設定
        m_PhaseObject.AddComponent<JoinPhase>();

        await UniTask.WaitUntil(() => IsNextPhaseFlag());

        // フェイズオブジェクト削除
        Destroy(m_PhaseObject.GetComponent<JoinPhase>());
        // 次のフェイズへ
        return GetNextPhaseType();
    }
    // メインフェイズ
    async UniTask<PhaseType> PhaseMain()
    {
        // フェイズオブジェクト設定
        m_PhaseObject.AddComponent<MainPhase>();

        await UniTask.WaitUntil(() => IsNextPhaseFlag());

        // フェイズオブジェクト削除
        Destroy(m_PhaseObject.GetComponent<MainPhase>());
        // 次のフェイズへ
        return GetNextPhaseType();
    }
    // エンドフェイズ
    async UniTask<PhaseType> PhaseEnd()
    {
        // フェイズオブジェクト設定
        m_PhaseObject.AddComponent<EndPhase>();

        await UniTask.WaitUntil(() => IsNextPhaseFlag());

        // フェイズオブジェクト削除
        Destroy(m_PhaseObject.GetComponent<EndPhase>());

        // ターン終了処理
        TurnEnd();

        // 次のフェイズへ
        return GetNextPhaseType();
    }

    // ターンエンドフラグの設定
    public void SetTurnEndFlag()
    {
        m_TurnEndFlag = true;
    }

    // ターンエンドフラグが立っているか
    public bool IsTurnEndFlag()
    {
        return m_TurnEndFlag;
    }

    // ターン終了
    public void TurnEnd()
    {
        // 現在のターンを逆にする
        SetTurnSide(Common.GetRevSide(m_TurnSide));

        // ターン終了リクエスト初期化
        m_TurnEndFlag = false;

        Debug.Log($"'{m_TurnSide}'ターンに進む");
    }

    // -----側-----
    // ターン側を設定
    public void SetTurnSide(Side _side)
    {
        m_TurnSide = _side;
    }
    // ターン側を取得
    public Side GetTurnSide()
    {
        return m_TurnSide;
    }
    // 側を取得(indexから)
    public Side GetSide(int _index)
    {
        // 範囲外ならはじく
        if( _index < 0 || _index > GetSideMax())
        {
            Debug.LogError("範囲外のindex" + "[" + _index.ToString() + "]" + "のため'eSide_None'を返しました" );
            return Side.eSide_None;
        }

        // プレイヤー
        if (_index == (int)Side.eSide_Player)
        {
            return Side.eSide_Player;
        }
        // 敵
        else if (_index == (int)Side.eSide_Enemy)
        {
            return Side.eSide_Enemy;
        }

        return Side.eSide_None;
    }
    // 指定側のIndexを取得
    public int GetSideIndex(Side _side)
    {
        return (int)_side;
    }
    // 側の最大数を取得
    public int GetSideMax()
    {
        return (int)Side.eSide_Max;
    }

    // ターン数取得
    public int GetTurnNum()
    {
        return m_TurnNum;
    }

    // -------フェイズ------
    // フェイズ設定。
    public void SetPhase(PhaseType _phase)
    {
        m_Phase = _phase;
    }
    public void SetNextPhase(PhaseType _nextPhase)
    {
        m_NextPhase = _nextPhase;
    }
    // フェイズ取得。
    public PhaseType GetPhaseType()
    {
        return m_Phase;
    }
    public PhaseType GetNextPhaseType()
    {
        return m_NextPhase;
    }
    // 指定フェイズのオブジェクトを取得する
    public GameObject GetPhaseObject(PhaseType _phase)
    {
        // 指定フェイズじゃなければnullを返す
        switch (_phase)
        {
            case PhaseType.ePhaseType_Start:
                if(m_PhaseObject.GetComponent<StartPhase>() == null) { return null; }
                break;
            case PhaseType.ePhaseType_Join:
                if (m_PhaseObject.GetComponent<JoinPhase>() == null) { return null; }
                break;
            case PhaseType.ePhaseType_Main:
                if (m_PhaseObject.GetComponent<MainPhase>() == null) { return null; }
                break;
            case PhaseType.ePhaseType_End:
                if (m_PhaseObject.GetComponent<EndPhase>() == null) { return null; }
                break;
            default:
                Debug.Log("指定されたフェイズはGetPhaseObjectのSwitch文に記載されていません");
                break;
        }

        return m_PhaseObject;
    }
    // 指定のフェイズか。
    public bool IsPhase(PhaseType _phase)
    {
        return m_Phase == _phase;
    }
    // 次のフェイズに進むフラグを立てる
    public void SetNextPhaseFlag()
    {
        m_NextPhaseFlag = true;
    }
    // 次のフェイズに進むフラグが立っているか
    public bool IsNextPhaseFlag()
    {
        return m_NextPhaseFlag;
    }
    // 次のフェイズとフラグを設定
    public void SetNextPhaseAndFlag(PhaseType _nextPhase)
    {
        SetNextPhase(_nextPhase);
        SetNextPhaseFlag();
    }
    // ---国民---
    // 国民数設定
    public void SetPeopleValue(Side _side, int _value)
    {
        m_PeopleValue[(int)_side] = _value;
    }
    // 国民数取得
    public int GetPeopleValue(Side _side)
    {
        return m_PeopleValue[(int)_side];
    }
    // 国民数追加
    public void AddPeopleValue(Side _side, int _value)
    {
        m_PeopleValue[(int)_side] += _value;
    }

    // ---研究---
    // 研究数設定
    public void SetScienceValue(Side _side, int _value)
    {
        m_ScienceValue[(int)_side] = _value;
    }
    // 研究数取得
    public int GetScienceValue(Side _side)
    {
        return m_ScienceValue[(int)_side];
    }
    // 研究数追加
    public void AddScienceValue(Side _side, int _value)
    {
        m_ScienceValue[(int)_side] += _value;
    }

    // ---軍事---
    // 軍事数設定
    public void SetMilitaryValue(Side _side, int _value)
    {
        m_MilitaryValue[(int)_side] = _value;
    }
    // 軍事数取得
    public int GetMilitaryValue(Side _side)
    {
        return m_MilitaryValue[(int)_side];
    }
    // 軍事数追加
    public void AddMilitaryValue(Side _side, int _value)
    {
        m_MilitaryValue[(int)_side] += _value;
    }

    // ---スパイ---
    // スパイ数設定
    public void SetSpyValue(Side _side, int _value)
    {
        m_SpyValue[(int)_side] = _value;
    }
    // スパイ数取得
    public int GetSpyValue(Side _side)
    {
        return m_SpyValue[(int)_side];
    }
    // スパイ数追加
    public void AddSpyValue(Side _side, int _value)
    {
        m_SpyValue[(int)_side] += _value;
    }
}
