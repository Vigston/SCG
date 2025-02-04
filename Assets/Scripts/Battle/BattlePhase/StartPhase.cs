using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using battleTypes;
using Photon.Pun;

public class StartPhase : MonoBehaviour
{
    // ===構造体===
    public enum State
    {
        eState_Init,        // 初期化
        eState_Starting,    // 開始時
        eState_NetworkSync, // 通信同期
        eState_End,         // 終了
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
                Debug.Log($"{nameof(StartPhase)}" + $"登録されていないステートです：{m_State}");
                break;
        }
    }

    // 初期化
    void Init()
    {
		if (m_StateValue == 1)
        {
			Debug.Log($"{nameof(StartPhase)}" + "初期化ステート処理開始");
		}

		// 開始時へ
		SetNextStateAndFlag(State.eState_Starting);
	}
    // 開始時
    void Starting()
    {
        if(m_StateValue == 1)
        {
            Debug.Log($"{nameof(StartPhase)}" + "開始時ステート処理開始");

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
        }
    }
    // 通信同期
    void NetworkSync()
    {
		if (m_StateValue == 1)
        {
			Debug.Log($"{nameof(StartPhase)}" + "通信同期ステート処理開始");
		}

		/////通信同期/////
		NetWorkSync.instance.GameInfoNetworkSync();

		// 終了へ
		SetNextStateAndFlag(State.eState_End);
	}
	// 終了
	void End()
    {
		if (m_StateValue == 1)
        {
			Debug.Log($"{nameof(StartPhase)}" + "終了ステート処理開始");
		}

		if (PhotonNetwork.IsConnected)
		{
			if (PhotonNetwork.IsMasterClient)
			{
				// ジョインフェイズに移動。
				BattleMgr.instance.photonView.RPC(nameof(BattleMgr.instance.SetNextPhaseAndFlag), RpcTarget.All, PhaseType.ePhaseType_Join);
			}
		}
		else
		{
			// ジョインフェイズに移動。
			BattleMgr.instance.SetNextPhaseAndFlag(PhaseType.ePhaseType_Join);
		}
	}

    // --システム--
    async UniTask StateLoop()
    {
        while (true)
        {
            // ステート更新処理
            // 次のステート更新(今のステートに設定)
            SetNextState(m_State);
            // 次のステート
            State nextState = GetNextState();
            // 次のステートに移動するフラグ初期化
            m_NextStateFlag = false;

            switch (m_State)
            {
                case State.eState_Init:
                    nextState = await StateInit();
                    break;
                case State.eState_Starting:
                    nextState = await StateStarting();
                    break;
				case State.eState_NetworkSync:
					nextState = await StateNetworkSync();
					break;
				case State.eState_End:
                    nextState = await StateEnd();
                    break;
                default:
                    Debug.Log($"{nameof(StartPhase)}" + "StateLoopに記載されていないステートに遷移しようとしています");
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
