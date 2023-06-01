using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using battleTypes;
using Cysharp.Threading.Tasks;

public class BattleUser : MonoBehaviour
{
    // ===構造体===
    public enum State
    {
        eState_Init,
        eState_Main,
        eState_Think,
        eState_End,
    }

    // ===変数===
    // 側
    [SerializeField, ReadOnly]
    Side m_Side;
    // 現在のステート
    [SerializeField, ReadOnly]
    State m_State;
    // 次のステート
    State m_NextState;
    // ステートごとの実行回数
    int m_StateValue = 0;

    // ===フラグ===
    // ステートの更新フラグ
    bool m_NextStateFlag = false;
    // ユーザー終了フラグ
    bool m_UserEndFlag = false;
    // 思考フラグ
    bool m_ThinkFlag = false;

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
            case State.eState_Main:
                Main();
                break;
            case State.eState_Think:
                Think();
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
            Debug.Log("初期化ステート処理開始");
        }
        
        // メイン処理へ
        SetNextStateAndFlag(State.eState_Main);
        Debug.Log("初期化ステート処理終了");
    }

    // メイン処理
    void Main()
    {
        if(m_StateValue == 1)
        {
            Debug.Log("メインステート処理開始");
        }

        // 思考フラグが立っているなら
        if(m_ThinkFlag)
        {
            // 思考へ
            SetNextStateAndFlag(State.eState_Think);
            Debug.Log("メインステート処理終了");
            return;
        }

        // ユーザーの終了フラグが立っているなら
        if(m_UserEndFlag)
        {
            // 終了へ
            SetNextStateAndFlag(State.eState_End);
            Debug.Log("メインステート処理終了");
            return;
        }
    }

    // 思考処理
    void Think()
    {
        if (m_StateValue == 1)
        {
            Debug.Log("思考ステート処理開始");
        }

        // メイン処理へ
        SetNextStateAndFlag(State.eState_Main);
        Debug.Log("思考ステート処理終了");
    }

    // 終了
    void End()
    {
        if (m_StateValue == 1)
        {
            Debug.Log("終了ステート処理開始");
        }

        // メインフェイズに移動。
        BattleMgr.instance.SetNextPhaseAndFlag(PhaseType.ePhaseType_Main);
        Debug.Log("終了ステート処理終了");
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
                case State.eState_Main:
                    Debug.Log("メインステート");
                    nextState = await StateMain();
                    Debug.Log("次のステートへ");
                    break;
                case State.eState_Think:
                    Debug.Log("思考ステート");
                    nextState = await StateThink();
                    Debug.Log("次のステートへ");
                    break;
                case State.eState_End:
                    Debug.Log("終了ステート");
                    nextState = await StateEnd();
                    Debug.Log("次のステートへ");
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
    // メイン処理
    async UniTask<State> StateMain()
    {
        await UniTask.WaitUntil(() => IsNextStateFlag());
        // 次のステートへ
        return GetNextState();
    }
    // 思考処理
    async UniTask<State> StateThink()
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

    // --側--
    // 側設定
    public void SetSide(Side _side)
    {
        m_Side = _side;
    }
    // 側取得
    public Side GetSide()
    {
        return m_Side;
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
    // 指定の次のステートか
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

    public void SetUserEndFlag(bool _flag)
    {
        m_UserEndFlag = _flag;
    }
    public bool IsUserEndFlag()
    {
        return m_UserEndFlag;
    }
    public void SetThinkFlag(bool _flag)
    {
        m_ThinkFlag = _flag;
    }
    public bool IsThinkFlag()
    {
        return m_ThinkFlag;
    }
}
