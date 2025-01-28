using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using battleTypes;
using Cysharp.Threading.Tasks;
using Photon.Pun;

public class BattleMgr : MonoBehaviourPun
{
    // =================クラス================

    // =================構造体================
    public enum BattleResult
    {
        eBattleResult_InBattle,
        eBattleResult_Win,
        eBattleResult_Lose,
        eBattleResult_Draw,
    }

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
    // 勝敗
    [SerializeField]
    BattleResult m_BattleResult;
    // 国民カード数
    [SerializeField]
    private int[] m_PeopleCardNum = new int[(int)Side.eSide_Max];
    // 研究カード数
    [SerializeField]
    private int[] m_ScienceCardNum = new int[(int)Side.eSide_Max];
    // 軍事カード数
    [SerializeField]
    private int[] m_MilitaryCardNum = new int[(int)Side.eSide_Max];
    // スパイカード数
    [SerializeField]
    private int[] m_SpyCardNum = new int[(int)Side.eSide_Max];

    // 研究値
    [SerializeField]
    private int[] m_ScienceValue = new int[(int)Side.eSide_Max];
    // Gold値
    [SerializeField]
    private int[] m_GoldValue = new int[(int)Side.eSide_Max];

    // 更新フラグ
    private bool m_UpdateFlag = false;
    // フェイズ進行フラグ
    private bool m_NextPhaseFlag = false;
    // ターンエンドフラグ
    private bool m_TurnEndFlag = false;

    // 次のターンに追加される国民カードにスパイを付与するフラグ
    private bool m_NextJoinSpyFlag = false;

    // フェイズオブジェクト
    public GameObject m_PhaseObject;

    private void Awake()
    {
        CreateInstance();
	}

    // Start is called before the first frame update
    void Start()
    {
		// バトルユーザー作成
		BattleUserCtr.instance.CreatePlayerUser();
        BattleUserCtr.instance.CreateEnemyUser();

		// 先行後攻を決める
		DecidePrecedingSecond();

		// スタートフェイズから始める
		GetSetPhaseType = PhaseType.ePhaseType_Start;
        // フェイズループ処理
        PhaseLoop().Forget();
        // アクション実行処理
        ActionMgr.instance.ExecuteActions().Forget();

		// 勝敗更新
		BattleResultUpdate().Forget();

        // BattleMgrの更新リクエスト
        UpdateRequest();
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
            Side Side = (Side)i;

            // 国民カード数計算
            int peopleNum = BattleCardMgr.instance.GetCardNumFromKind(Side, BattleCard.Kind.eKind_People);
            SetPeopleCardNum(Side, peopleNum);
            // 研究カード数計算
            int scienceNum = BattleCardMgr.instance.GetCardNumFromAppendKind(Side, BattleCard.JobKind.eAppendKind_Science);
            SetScienceCardNum(Side, scienceNum);
            // 軍事カード数計算
            int militaryNum = BattleCardMgr.instance.GetCardNumFromAppendKind(Side, BattleCard.JobKind.eAppendKind_Military);
            SetMilitaryCardNum(Side, militaryNum);
            // スパイカード数計算
            int spyNum = BattleCardMgr.instance.GetCardNumFromAppendKind(Side, BattleCard.JobKind.eAppendKind_Spy);
            SetSpyCardNum(Side, spyNum);

            // 研究値計算
            int scienceValue = MathScienceValue(Side);
            SetScienceValue(Side, scienceValue);
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
    // フェイズ更新
    async UniTask PhaseLoop()
    {
        Debug.Log("PhaseLoop起動");
        while(true)
        {
            Debug.Log("フェイズ更新！！");
            // FightMgrの更新リクエスト
            UpdateRequest();
            // 次のフェイズ(ここでは現在のフェイズを代入)
            PhaseType nextPhase = GetSetPhaseType;
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
			GetSetPhaseType = nextPhase;

			/////通信同期/////
			NetWorkSync.instance.GameInfoNetworkSync();
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
        return GetSetNextPhaseType;
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
        return GetSetNextPhaseType;
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
        return GetSetNextPhaseType;
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
		return GetSetNextPhaseType;
    }

    // 勝敗の更新
    async UniTask BattleResultUpdate()
    {
        while(true)
        {
            await CheckBattleResult();
        }
    }

    // 勝敗をチェックする
    async UniTask CheckBattleResult()
    {
        await UniTask.WaitUntil(() => IsWinCondition() || IsLoseCondition());
        // 勝利条件
        bool isWin = IsWinCondition();
        // 敗北条件
        bool isLose = IsLoseCondition();

        // 勝敗を設定
        if (isWin)
        {
			GetSetBattleResult = BattleResult.eBattleResult_Win;
        }
        else if (isLose)
        {
			GetSetBattleResult = BattleResult.eBattleResult_Lose;
        }
    }

    // 勝利条件を満たしているか
    public bool IsWinCondition()
    {
        bool isWin = false;

        // 自分の研究値が6以上で勝利
        if (GetScienceValue(Side.eSide_Player) >= 6)
        {
            isWin = true;
        }

        return isWin;
    }

    // 敗北条件を満たしているか
    public bool IsLoseCondition()
    {
        bool isLose = false;

        // 相手の研究値が6以上で敗北
        if (GetScienceValue(Side.eSide_Enemy) >= 6)
        {
            isLose = true;
        }

        return isLose;
    }
    // 勝敗
    public BattleResult GetSetBattleResult
    {
        get { return m_BattleResult; }
        set { m_BattleResult = value; }
    }

    // ターンエンドフラグの設定
    public void SetTurnEndFlag()
    {
        GameObject phaseObj = GetPhaseObject();
        // フェイズオブジェクトのnullチェック
        if (phaseObj == null) { return; }
        MainPhase mainPhase = phaseObj.GetComponent<MainPhase>();
        // メインフェイズじゃないならはじく
        if (mainPhase == null) { return; }
        // メインステートじゃなければはじく
        if(mainPhase.GetSetState != MainPhase.State.eState_Main) { return; }

        // ターンエンドフラグを立てる
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
        // 現在のターンと操作側を逆にする
        Side turnSide = GetSetTurnSide;
        Side operateSide = BattleUserMgr.instance.GetSetOperateSide;
		GetSetTurnSide = Common.GetRevSide(turnSide);

		BattleUserMgr.instance.GetSetOperateSide = Common.GetRevSide(operateSide);

		// ターン終了リクエスト初期化
		m_TurnEndFlag = false;

        // カードのターン情報初期化
        foreach(BattleCard battleCard in BattleCardMgr.instance.GetCardList())
        {
            // ターン情報初期化
            battleCard.InitTurnInfo();

            // ステータスを通常に設定する
            battleCard.GetSetStatus = BattleCard.Status.eStatus_Normal;
        }

        Debug.Log($"'{m_TurnSide}'ターンに進む");
    }

	// -----側-----
	// ターン側のGetSetメソッド
	public Side GetSetTurnSide
	{
		get { return m_TurnSide; }
		set { m_TurnSide = value; }
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
    // 側の最大数を取得
    public int GetSideMax()
    {
        return (int)Side.eSide_Max;
    }

    // ターン数
    public int GetSetTurnNum
    {
        get { return m_TurnNum; }
        set { m_TurnNum = value; }
    }
    public int GetTurnNum()
    {
        return m_TurnNum;
    }

	// 先行後攻を決める
	public void DecidePrecedingSecond()
	{
        if(PhotonNetwork.IsMasterClient == true)
        {
			// 最初のターン側
			Side firstTurnSide = Side.eSide_None;

			// 1～100までの乱数取得
			int rnd = Random.Range(1, 101);

			// 乱数値が0以下か100より大きければ異常値のためアサートを流してマスタークライアントを先行にする
			if (rnd <= 0 || rnd > 100)
			{
				Debug.LogWarning($"先行後攻を決める際に乱数の異常値を検知：['{rnd}']");
			}

			// 先行処理(1～50)
			if (rnd <= 50)
			{
				// ターン側を自分として設定
				firstTurnSide = Side.eSide_Player;
			}
			// 後攻処理(51～100)
			else
			{
				// ターン側を相手として設定
				firstTurnSide = Side.eSide_Enemy;
			}

			// ターン側設定
			GetSetTurnSide = firstTurnSide;
            // 操作側設定
            BattleUserMgr.instance.GetSetOperateSide = firstTurnSide;

            // マスタークライアント以外にターン側と操作側を渡す
            NetWorkSync.instance.photonView.RPC("SetSyncTurnSide", RpcTarget.Others, GetSetTurnSide);
			NetWorkSync.instance.photonView.RPC("SetSyncOperateUserSide", RpcTarget.Others, BattleUserMgr.instance.GetSetOperateSide);

			Debug.Log($"DecidePrecedingSecond()｜乱数値：{rnd}、最初のターン側：{firstTurnSide}");
		}
	}

	// -------フェイズ------
	public PhaseType GetSetPhaseType
    {
        get { return m_Phase; }
        set { m_Phase = value; }
    }
	public PhaseType GetSetNextPhaseType
	{
		get { return m_NextPhase; }
		set { m_NextPhase = value; }
	}
    // 指定フェイズのオブジェクトを取得する
    public GameObject GetPhaseObject()
    {
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
    [PunRPC]
    public void SetNextPhaseAndFlag(PhaseType _nextPhase)
    {
		GetSetNextPhaseType = _nextPhase;
        SetNextPhaseFlag();
    }
    [PunRPC]
    void SetNextPhaseFlag_RPC(bool _nextPhaseFlag)
    {
        m_NextPhaseFlag = _nextPhaseFlag;

	}
    // ---国民---
    // 国民カード数設定
    public void SetPeopleCardNum(Side _Side, int _value)
    {
        m_PeopleCardNum[(int)_Side] = _value;
    }
    // 国民カード数取得
    public int GetPeopleCardNum(Side _Side)
    {
        return m_PeopleCardNum[(int)_Side];
    }
    // 国民カード数追加
    public void AddPeopleCardNum(Side _Side, int _value)
    {
        m_PeopleCardNum[(int)_Side] += _value;
    }

    // ---研究---
    // 研究カード数設定
    public void SetScienceCardNum(Side _Side, int _value)
    {
        m_ScienceCardNum[(int)_Side] = _value;
    }
    // 研究カード数取得
    public int GetScienceCardNum(Side _Side)
    {
        return m_ScienceCardNum[(int)_Side];
    }
    // 研究カード数追加
    public void AddScienceCardNum(Side _Side, int _value)
    {
        m_ScienceCardNum[(int)_Side] += _value;
    }

    // 研究値設定
    public void SetScienceValue(Side _Side, int _value)
    {
        m_ScienceValue[(int)_Side] = _value;
    }
    // 研究値取得
    public int GetScienceValue(Side _Side)
    {
        return m_ScienceValue[(int)_Side];
    }
    // 研究値追加
    public void AddScienceValue(Side _Side, int _value)
    {
        m_ScienceValue[(int)_Side] += _value;
    }
    // 研究値計算
    public int MathScienceValue(Side Side)
    {
        List<BattleCard> cardList = new List<BattleCard>();

        var scienceCardList = BattleCardMgr.instance.GetCardListFromAppendKind(Side, BattleCard.JobKind.eAppendKind_Science);

        foreach(var battleCard in scienceCardList)
        {
            // スパイの追加種類を持っているならはじく
            if (battleCard.IsHaveAppendKind(BattleCard.JobKind.eAppendKind_Spy)) { continue; }

            // 追加
            cardList.Add(battleCard);
        }

        return cardList.Count;
    }

    // ---Gold---
    // Gold数設定
    public void SetGoldValue(Side _Side, int _value)
    {
        m_GoldValue[(int)_Side] = _value;
    }
    // Gold数取得
    public int GetGoldValue(Side _Side)
    {
        return m_GoldValue[(int)_Side];
    }
    // Gold数追加
    public void AddGoldValue(Side _Side, int _value)
    {
        m_GoldValue[(int)_Side] += _value;
    }
    // Gold数減らす
    public void ReduceGoldValue(Side _Side, int _value)
    {
        m_GoldValue[(int)_Side] -= _value;

        // 0より小さくなる場合には0とする
        if(m_GoldValue[(int)_Side] < 0)
        {
            m_GoldValue[(int)_Side] = 0;
        }
    }

    // ---軍事---
    // 軍事カード数設定
    public void SetMilitaryCardNum(Side _Side, int _value)
    {
        m_MilitaryCardNum[(int)_Side] = _value;
    }
    // 軍事カード数取得
    public int GetMilitaryCardNum(Side _Side)
    {
        return m_MilitaryCardNum[(int)_Side];
    }
    // 軍事カード数追加
    public void AddMilitaryCardNum(Side _Side, int _value)
    {
        m_MilitaryCardNum[(int)_Side] += _value;
    }

    // ---スパイ---
    // スパイカード数設定
    public void SetSpyCardNum(Side _Side, int _value)
    {
        m_SpyCardNum[(int)_Side] = _value;
    }
    // スパイカード数取得
    public int GetSpyCardNum(Side _Side)
    {
        return m_SpyCardNum[(int)_Side];
    }
    // スパイカード数追加
    public void AddSpyCardNum(Side _Side, int _value)
    {
        m_SpyCardNum[(int)_Side] += _value;
    }

    // 次のターンに追加される国民カードにスパイを付与するフラグを立てる
    public void SetNextJoinSpyFlag(bool _flag)
    {
        m_NextJoinSpyFlag = _flag;
    }
    // 次のターンに追加される国民カードにスパイを付与するフラグが立っているか
    public bool IsNextJoinSpyFlag()
    {
        return m_NextJoinSpyFlag;
    }
}
