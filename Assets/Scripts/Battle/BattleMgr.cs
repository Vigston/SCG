using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using battleTypes;
using TMPro;
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
    // 研究ポイント
    [SerializeField]
    private int[] m_ResearchValue = new int[(int)Side.eSide_Max];

    public TextMeshProUGUI m_TextTurnNum;
    public TextMeshProUGUI m_TextTurnSide;
    public TextMeshProUGUI m_TextPhase;
    public TextMeshProUGUI m_TextResearch_Player;
    public TextMeshProUGUI m_TextResearch_Enemy;

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
        m_TextTurnNum.text = "ターン数：" + m_TurnNum.ToString();
        m_TextTurnSide.text = "ターン側：" + m_TurnSide.ToString();
        m_TextPhase.text = "フェイズ：" + m_Phase.ToString();
        m_TextResearch_Player.text = "自分の研究：" + GetResearchValue(Side.eSide_Player).ToString();
        m_TextResearch_Enemy.text = "相手の研究：" + GetResearchValue(Side.eSide_Enemy).ToString();

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
            PhaseType nextPhase = GetPhase();
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
        return GetNextPhase();
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
        return GetNextPhase();
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
        return GetNextPhase();
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
        return GetNextPhase();
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
    public PhaseType GetPhase()
    {
        return m_Phase;
    }
    public PhaseType GetNextPhase()
    {
        return m_NextPhase;
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

    // ---研究---
    // 研究ポイント設定
    public void SetResearchValue(Side _side, int _value)
    {
        m_ResearchValue[(int)_side] = _value;
    }
    // 研究ポイント取得
    public int GetResearchValue(Side _side)
    {
        return m_ResearchValue[(int)_side];
    }
    // 研究ポイント追加
    public void AddResearchValue(Side _side, int _value)
    {
        m_ResearchValue[(int)_side] += _value;
    }
}
