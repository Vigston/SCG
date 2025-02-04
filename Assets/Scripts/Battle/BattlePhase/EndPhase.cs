using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using battleTypes;
using Photon.Pun;

public class EndPhase : MonoBehaviour
{
    // ===構造体===
    public enum State
    {
        eState_Init,
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
            case State.eState_End:
                End();
                break;
            default:
                Debug.Log($"{nameof(EndPhase)}" + $"登録されていないステートです。：{m_State}");
                break;
        }
    }

    // 初期化
    void Init()
    {
		if (m_StateValue == 1)
		{
			Debug.Log($"{nameof(EndPhase)}" + "初期化ステート処理開始");
		}
		
        // 終了へ
        SetNextStateAndFlag(State.eState_End);
    }

    // 終了
    void End()
    {
        if (m_StateValue == 1)
        {
            Debug.Log($"{nameof(EndPhase)}" + "終了ステート処理開始");

            if(PhotonNetwork.IsConnected)
            {
                if(PhotonNetwork.IsMasterClient)
                {
					// スタートフェイズに移動。
					BattleMgr.instance.photonView.RPC(nameof(BattleMgr.instance.SetNextPhaseAndFlag), RpcTarget.All, PhaseType.ePhaseType_Start);
				}
            }
            else
            {
                // スタートフェイズに移動。
                BattleMgr.instance.SetNextPhaseAndFlag(PhaseType.ePhaseType_Start);
			}
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
                case State.eState_End:
                    nextState = await StateEnd();
                    break;
                default:
                    Debug.Log($"{nameof(EndPhase)}" + "StateLoopに記載されていないステートに遷移しようとしています");
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
