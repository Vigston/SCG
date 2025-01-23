using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using battleTypes;
using System.Linq;
using Photon.Pun;

public class JoinPhase : MonoBehaviourPunCallbacks
{
    // ===構造体===
    public enum State
    {
        eState_Init,
        eState_Start,
		eState_JoinPeopleGameAction,
        eState_End,
    }

	// ===変数===
	// 現在のステート
	State m_State;
    // 次のステート
    State m_NextState;

    // ステートごとの実行回数
    int m_StateValue = 0;

    private GameObject m_JoinPeopleGameAction;

	// ===フラグ===
	// ステートの更新フラグ
	bool m_NextStateFlag = false;
    // JoinPeopleActionのAddフラグ
    bool m_JoinPeopleActionAddFlag = false;


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
            case State.eState_JoinPeopleGameAction:
				JoinPeopleGameAction();
                break;
            case State.eState_End:
                End();
                break;
            default:
                Debug.Log("登録されていないステートです。");
                break;
        }
    }

    // 初期化
    void Init()
    {
		// 国民を参加させるアクションへ
		SetNextStateAndFlag(State.eState_JoinPeopleGameAction);
	}
	// 国民を参加させるアクション
	void JoinPeopleGameAction()
    {
        // 場に空きがないなら終了へ
        if (BattleStageMgr.instance.GetCardAreaNumFromEmptyCard() <= 0)
        {
            Debug.Log("JoinPhase：場に空きがないので終了へ");
            // 終了へ
            SetNextStateAndFlag(State.eState_End);
            return;
        }

		if (m_StateValue == 1)
        {
			// 通信を行っていて
			if (PhotonNetwork.IsConnected)
            {
				// マスタークライアントなら
				if (PhotonNetwork.IsMasterClient)
                {
					m_JoinPeopleGameAction = PhotonNetwork.Instantiate(Common.prefabPath_GameAction, Vector3.zero, Quaternion.identity);
					// JoinPeopleGameActionをアタッチ
					m_JoinPeopleGameAction.AddComponent<JoinPeopleGameAction>();

					var actionObjViewID = m_JoinPeopleGameAction.GetComponent<PhotonView>().ViewID;

                    if(photonView == null)
                    {
                        Debug.Log("JoinPhaseのphotonViewがNULLです!!");
                        return;
					}

					photonView.RPC("AddJoinPeopleGameAction", RpcTarget.All, actionObjViewID);
				}
			}
            else
            {
				// 共通のGameActionプレハブを生成
				GameObject prefab = Resources.Load<GameObject>(Common.prefabPath_GameAction);
                if(prefab == null)
                {
					Debug.Log($"Prefab Path: {Common.prefabPath_GameAction}");
					Debug.Log("prefab == null");
                }

				m_JoinPeopleGameAction = Instantiate(prefab, Vector3.zero, Quaternion.identity);
				if (m_JoinPeopleGameAction == null)
				{
					Debug.Log("gameActionObj == null");
				}
				// JoinPeopleGameActionをアタッチ
				m_JoinPeopleGameAction.AddComponent<JoinPeopleGameAction>();

                JoinPeopleGameAction joinPeopleGameAction = m_JoinPeopleGameAction.GetComponent<JoinPeopleGameAction>();
				// ターン側を設定
				joinPeopleGameAction.GetSetActionSide = BattleMgr.instance.GetSetTurnSide;
				// アクション追加
				m_JoinPeopleActionAddFlag = true;
				ActionMgr.instance.AddAction(m_JoinPeopleGameAction);
            }
		}

        // アクション追加がされていないならはじく
        if (m_JoinPeopleActionAddFlag == false) { return; }

        // アクションが終了しているなら
        if (ActionMgr.instance.IsCompletedAction(m_JoinPeopleGameAction))
        {
			// 終了へ
			SetNextStateAndFlag(State.eState_End);
		}
	}

    // 終了
    void End()
    {
        if (m_StateValue == 1)
        {
            // メインフェイズに移動。
            BattleMgr.instance.SetNextPhaseAndFlag(PhaseType.ePhaseType_Main);
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
                    Debug.Log("JoinPhase：eState_Init");
                    nextState = await StateInit();
                    break;
                case State.eState_JoinPeopleGameAction:
					Debug.Log("JoinPhase：eState_JoinPeopleGameAction");
					nextState = await StateJoinPeopleGameAction();
                    break;
                case State.eState_End:
					Debug.Log("JoinPhase：eState_End");
					nextState = await StateEnd();
                    break;
                default:
                    Debug.Log("StateLoopに記載されていないフェイズに遷移しようとしています");
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
	// 国民を参加させるアクション
	async UniTask<State> StateJoinPeopleGameAction()
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

	[PunRPC]
	void AddJoinPeopleGameAction(int _actionObjId)
	{
        if(PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient)
        {
			// ViewID を基に GameObject を取得
			m_JoinPeopleGameAction = PhotonView.Find(_actionObjId)?.gameObject;
			//m_JoinPeopleGameAction.AddComponent<JoinPeopleGameAction>();
		}

		if (m_JoinPeopleGameAction == null)
		{
			Debug.LogError($"アクションオブジェクトが NULL です (ViewID: {_actionObjId})");
			return;
		}

		// JoinPeopleGameAction コンポーネントを取得
		JoinPeopleGameAction joinPeopleGameAction = m_JoinPeopleGameAction.GetComponent<JoinPeopleGameAction>();
		if (joinPeopleGameAction == null)
		{
			Debug.LogError($"JoinPeopleGameAction コンポーネントが見つかりません (GameObject: {m_JoinPeopleGameAction.name})");
			return;
		}

		// ターン側を設定
		joinPeopleGameAction.GetSetActionSide = BattleMgr.instance.GetSetTurnSide;

		// アクションを追加
		m_JoinPeopleActionAddFlag = true;
		ActionMgr.instance.AddAction(m_JoinPeopleGameAction);
	}
}