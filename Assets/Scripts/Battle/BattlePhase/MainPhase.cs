using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using battleTypes;
using Photon.Pun;
using static Common;

public class MainPhase : MonoBehaviour
{
    // ===構造体===
    public enum State
    {
        eState_Init,    // 初期化
        eState_Main,    // メイン
        eState_End,     // 終了
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
		GetSetState = State.eState_Init;
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
            case State.eState_Main:
                Main();
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
		if (m_StateValue == 1)
		{
			Debug.Log($"{nameof(MainPhase)}" + "初期化ステート処理開始");
		}

		// 職業付与へ
		SetNextStateAndFlag(State.eState_Main);
    }

    void Main()
    {
		if (m_StateValue == 1)
		{
			Debug.Log($"{nameof(MainPhase)}" + "メインステート処理開始");

            // ターンエンドフラグが立っているなら終了する。
            if (BattleMgr.instance.GetSetTurnEndFlag)
            {
                Debug.Log("ここでターンエンドフラグ立ってるならバグ!!!!!");
            }
		}

		// ターンエンドフラグが立っているなら終了する。
		if (BattleMgr.instance.GetSetTurnEndFlag)
        {
            // 終了へ
            SetNextStateAndFlag(State.eState_End);
            return;
        }
    }

    // 終了
    void End()
    {
		if (m_StateValue == 1)
		{
			Debug.Log($"{nameof(MainPhase)}" + "終了ステート処理開始");
		}

		if (!BattleMgr.instance.GetSetNextPhaseFlag)
        {
			// エンドフェイズに移動。
			BattleMgr.instance.SetNextPhaseAndFlag(PhaseType.ePhaseType_End);
		}
	}

    // --システム--
    async UniTask StateLoop()
    {
        while (true)
        {
            // ステート更新処理
			// 次のステート更新(今のステートに設定)
			GetSetNextState = GetSetState;
            // 次のステートに移動するフラグ初期化
            m_NextStateFlag = false;

            switch (m_State)
            {
                case State.eState_Init:
					GetSetNextState = await StateInit();
                    break;
                case State.eState_Main:
					GetSetNextState = await StateMain();
                    break;
                case State.eState_End:
					GetSetNextState = await StateEnd();
                    break;
                default:
                    Debug.Log("StateLoopに記載されていないステートに遷移しようとしています");
                    break;
            }

            // 次のステートが現在と同じならはじく
            if (GetSetNextState == GetSetState) { continue; }

			// 次のステートを設定
			GetSetState = GetSetNextState;
        }
    }
    // 初期化
    async UniTask<State> StateInit()
    {
        await UniTask.WaitUntil(() => IsNextStateFlag());
        // 次のステートへ
        return GetNextState();
    }
    // メイン処理
    async UniTask<State> StateMain()
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
    public State GetSetState
    {
        get { return m_State; }
        set { m_State = value; }
    }
    public State GetSetNextState
    {
        get { return m_NextState; }
        set { m_NextState = value; }
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
		GetSetNextState = _nextState;
        SetNextStateFlag();
    }
}
