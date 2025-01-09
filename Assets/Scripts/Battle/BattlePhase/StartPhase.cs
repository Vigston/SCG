using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using battleTypes;

public class StartPhase : MonoBehaviour
{
    // ===構造体===
    public enum State
    {
        eState_Init,
        eState_Starting,
        eState_NetworkSync,
        eState_End,
    }

    // ===変数===
    // 現在のステート
    State m_State;
    // 次のステート
    State m_NextState;

    // ステートごとの実行回数
    int m_StateValue = 0;

    // ===フラグ===
    // ステートの更新フラグ
    bool m_NextStateFlag = false;

    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        // 初期化ステートから始める
        SetState(State.eState_Init);
        // ステートループ処理
        StateLoop().Forget();
    }

    // Update is called once per frame
    void Update()
    {
        m_StateValue++;
        switch (m_State)
        {
            case State.eState_Init:
                Init();
                break;
            case State.eState_Starting:
                Starting();
                break;
			case State.eState_NetworkSync:
				NetworkSync();
				break;
			case State.eState_End:
                End();
                break;
            default:
                Debug.Log($"登録されていないステートです：{m_State}");
                break;
        }
    }

    // 初期化
    void Init()
    {
        if(m_StateValue == 1)
        {
            Debug.Log("初期化ステート処理開始");
            // 開始時へ
            SetNextStateAndFlag(State.eState_Starting);
            Debug.Log("初期化ステート処理終了");
        }
    }
    // 開始時
    void Starting()
    {
        if(m_StateValue == 1)
        {
            Debug.Log("開始時ステート処理開始");

            // Gold追加処理
            // ターン側
            Side turnSide = BattleMgr.instance.GetSetTurnSide;
            // 場にいる商人数
            int merchantNum = BattleCardMgr.instance.GetCardNumFromAppendKind(turnSide, BattleCard.JobKind.eAppendKind_Merchant);
            // 追加されるGold
            int addGoldValue = merchantNum * Common.BattleConst.ADD_GOLD_EVERY_MERCHANT;

            // Gold追加
            BattleMgr.instance.AddGoldValue(turnSide, addGoldValue);

            // 通信同期へ
            SetNextStateAndFlag(State.eState_NetworkSync);
            Debug.Log("開始時ステート処理終了");
        }
    }
    // 通信同期
    void NetworkSync()
    {
		if (m_StateValue == 1)
		{
			Debug.Log("通信同期ステート処理開始");

			/////通信同期/////
			NetWorkSync.instance.GameInfoNetworkSync();

			// 終了へ
			SetNextStateAndFlag(State.eState_End);
			Debug.Log("通信同期ステート処理終了");
		}
	}
	// 終了
	void End()
    {
        if(m_StateValue == 1)
        {
            Debug.Log("終了ステート処理開始");
            // ジョインフェイズに移動。
            BattleMgr.instance.SetNextPhaseAndFlag(PhaseType.ePhaseType_Join);
            Debug.Log("終了ステート処理終了");
        }
    }

    // --システム--
    async UniTask StateLoop()
    {
        Debug.Log("StateLoop起動");
        while (true)
        {
            // ステート更新処理
            Debug.Log("ステート更新！！");
            // 次のステート更新(今のステートに設定)
            SetNextState(m_State);
            // 次のステート
            State nextState = GetNextState();
            // 次のステートに移動するフラグ初期化
            m_NextStateFlag = false;

            switch (m_State)
            {
                case State.eState_Init:
                    Debug.Log("初期化ステート");
                    nextState = await StateInit();
                    Debug.Log("次のステートへ");
                    break;
                case State.eState_Starting:
                    Debug.Log("開始時ステート");
                    nextState = await StateStarting();
                    Debug.Log("次のステートへ");
                    break;
				case State.eState_NetworkSync:
					Debug.Log("通信同期ステート");
					nextState = await StateNetworkSync();
					Debug.Log("次のステートへ");
					break;
				case State.eState_End:
                    Debug.Log("終了ステート");
                    nextState = await StateEnd();
                    Debug.Log("次のステートへ");
                    break;
                default:
                    Debug.Log("StateLoopに記載されていないステートに遷移しようとしています");
                    break;
            }

            // 次のステートが現在と同じならはじく
            if (nextState == m_State) { continue; }

            // 次のステートを設定
            SetState(nextState);
        }
    }
    // 初期化
    async UniTask<State> StateInit()
    {
        await UniTask.WaitUntil(() => IsNextStateFlag());
        // 次のステートへ
        return GetNextState();
    }
    // 開始時
    async UniTask<State> StateStarting()
    {
        await UniTask.WaitUntil(() => IsNextStateFlag());
        // 次のステートへ
        return GetNextState();
    }
	// 通信同期
	async UniTask<State> StateNetworkSync()
	{
		await UniTask.WaitUntil(() => IsNextStateFlag());
		// 次のステートへ
		return GetNextState();
	}
	// 終了
	async UniTask<State> StateEnd()
    {
        await UniTask.WaitUntil(() => IsNextStateFlag());

        // 次のステートへ
        return GetNextState();
    }

    // --ステート--
    // ステート設定
    public void SetState(State _state)
    {
        m_State = _state;
    }
    public void SetNextState(State _nextState)
    {
        m_NextState = _nextState;
    }
    // ステート取得
    public State GetState()
    {
        return m_State;
    }
    public State GetNextState()
    {
        return m_NextState;
    }
    // 指定のステートか
    public bool IsState(State _state)
    {
        return m_State == _state;
    }
    public bool IsNextState(State _nextState)
    {
        return m_NextState == _nextState;
    }

    // --フラグ--
    public void SetNextStateFlag()
    {
        m_NextStateFlag = true;
        m_StateValue = 0;
    }
    public bool IsNextStateFlag()
    {
        return m_NextStateFlag;
    }

    // 次のステートとフラグを設定
    public void SetNextStateAndFlag(State _nextState)
    {
        SetNextState(_nextState);
        SetNextStateFlag();
    }
}
