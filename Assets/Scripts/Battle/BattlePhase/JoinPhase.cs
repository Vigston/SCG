using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using battleTypes;
using System.Linq;

public class JoinPhase : MonoBehaviour
{
    // ===構造体===
    public enum State
    {
        eState_Init,
        eState_JoinPeople,
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
            case State.eState_JoinPeople:
                JoinPeople();
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
        Debug.Log("初期化ステート処理開始");
        // 国民カード追加へ
        SetNextStateAndFlag(State.eState_JoinPeople);
        Debug.Log("初期化ステート処理終了");
    }

    // 国民カード追加
    void JoinPeople()
    {
        Debug.Log("国民追加ステート処理開始");
        if (m_StateValue == 1)
        {
            // 現在のターン側
            Side turnSide = BattleMgr.instance.GetTurnSide();
            // ターン側のカードが入っていないカードエリアリスト
            List<CardArea> cardAreaList = BattleStageMgr.instance.GetCardAreaFromSideEmptyCard(turnSide);

            // 追加するカードエリア
            CardArea joinArea = cardAreaList.FirstOrDefault();

            if(joinArea != null)
            {
                Debug.Log($"{joinArea.GetSide()}の{joinArea.GetPosition()}に国民カードを追加");
                // 国民カード追加
                BattleCardCtr.instance.CreateBattleCard(joinArea, BattleCard.Kind.eKind_People);
            }
        }
        // 終了へ
        SetNextStateAndFlag(State.eState_End);
        Debug.Log("国民追加ステート処理終了");
    }

    // 終了
    void End()
    {
        Debug.Log("終了ステート処理開始");
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
                case State.eState_JoinPeople:
                    Debug.Log("国民追加ステート");
                    nextState = await StateInit();
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
    // 国民カード追加
    async UniTask<State> StateJoinPeople()
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
