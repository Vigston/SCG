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
        eState_ThinkSelectCardArea,
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

    // ===思考結果変数===
    [SerializeField, ReadOnly]
    // 選択したカードエリア
    CardArea m_SelectedCardArea;

    // ===フラグ===
    // ステートの更新フラグ
    bool m_NextStateFlag = false;

    // カードエリア選択フラグ
    bool m_SelectCardAreaFlag = false;
    // カードがいないエリアを選択するフラグ
    bool m_SelectIsNoCardArea = false;

    // ユーザー終了フラグ
    bool m_UserEndFlag = false;

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
            case State.eState_ThinkSelectCardArea:
                ThinkSelectCardField();
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

        // 操作側
        Side operateSide = BattleUserMgr.instance.GetOperateUserSide();
        
        // 操作側じゃないならはじく
        if(operateSide != GetSide()) { return; }

        // カードエリア選択フラグが立っているなら
        if (m_SelectCardAreaFlag)
        {
            // カードエリア選択フラグへ
            SetNextStateAndFlag(State.eState_ThinkSelectCardArea);
            Debug.Log("メインステート処理終了");
            return;
        }

        // ユーザーの終了フラグが立っているなら
        if (m_UserEndFlag)
        {
            // 終了へ
            SetNextStateAndFlag(State.eState_End);
            Debug.Log("メインステート処理終了");
            return;
        }
    }

    // カードエリア選択思考処理
    void ThinkSelectCardField()
    {
        if (m_StateValue == 1)
        {
            // 選択したカードエリアを初期化。
            m_SelectedCardArea = null;
            Debug.Log("思考ステート処理開始");
        }

        // 左クリックをしたら。
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            foreach (RaycastHit hit in Physics.RaycastAll(ray))
            {
                // カードエリアをクリックしているなら。
                if (hit.collider.tag == "CardArea")
                {
                    // 操作側
                    Side operateSide = BattleUserMgr.instance.GetOperateUserSide();
                    // カードエリアを取得。
                    GameObject cardAreaObject = hit.transform.gameObject;

                    // 選択したカードエリア設定
                    if (cardAreaObject)
                    {
                        CardArea cardArea = hit.transform.gameObject.GetComponent<CardArea>();

                        // 操作側じゃないカードエリアを選択しているなら設定しない
                        if (operateSide != cardArea.GetSide()) { return; }
                        m_SelectedCardArea = cardArea;
                    }
                }
            }
        }

        // カードエリアを選択しているならメイン処理へ。
        if (m_SelectedCardArea != null)
        {
            // フラグの初期化
            m_SelectCardAreaFlag = false;

            // メイン処理へ
            SetNextStateAndFlag(State.eState_Main);
            Debug.Log("思考ステート処理終了");
            return;
        }
    }

    // 終了
    void End()
    {
        if (m_StateValue == 1)
        {
            Debug.Log("終了ステート処理開始");
            // メインフェイズに移動。
            BattleMgr.instance.SetNextPhaseAndFlag(PhaseType.ePhaseType_Main);
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
                case State.eState_Main:
                    Debug.Log("メインステート");
                    nextState = await StateMain();
                    Debug.Log("次のステートへ");
                    break;
                case State.eState_ThinkSelectCardArea:
                    Debug.Log("思考ステート");
                    nextState = await StateThinkSelectCardField();
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
    async UniTask<State> StateThinkSelectCardField()
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

    // ---思考結果---
    public CardArea GetSelectedCardArea()
    {
        return m_SelectedCardArea;
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

    // カードエリア選択開始
    public void StartThinkSelectCardArea(bool _selectFlag ,bool _isNoCardArea)
    {
        m_SelectCardAreaFlag = _selectFlag;
        m_SelectIsNoCardArea = _isNoCardArea;
    }

    // カードエリア選択フラグが立っているか。
    public bool IsSelectCardAreaFlag()
    {
        return m_SelectCardAreaFlag;
    }

    // ユーザー終了フラグ設定。
    public void SetUserEndFlag(bool _flag)
    {
        m_UserEndFlag = _flag;
    }
    // ユーザー終了フラグが立っているか。
    public bool IsUserEndFlag()
    {
        return m_UserEndFlag;
    }
}
