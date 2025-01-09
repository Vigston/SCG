using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using battleTypes;

// 国民を参加させるアクション
public class JoinPeopleGameAction : IGameAction
{
	// ===構造体===
	public enum State
	{
		eState_Init,
		eState_Start,
		eState_SelectField,
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
	// 選択したカードエリア
	CardArea m_SelectedCardArea;

	// ===フラグ===
	// ステートの更新フラグ
	bool m_NextStateFlag = false;

	// イベント
	public event Action<IAction> OnActionCompleted;

	// 側
	public Side GetSetActionSide { get; set; }
	// アクションの完了状態
	public bool IsCompleted { get; private set; }

	// 実行処理
	public async UniTask Execute(CancellationToken _cancellationToken)
	{
		// 初期化ステートから始める
		GetSetState = State.eState_Init;
		// ステートループ処理
		StateLoop().Forget();
		StateUpdate().Forget();

		// アクションの処理が終わるまで待機
		await UniTask.WaitUntil(() => IsCompleted);
	}

	// --システム--
	async UniTask StateUpdate()
	{
		while(true)
		{
			m_StateValue++;
			switch (m_State)
			{
				case State.eState_Init:
					Init();
					break;
				case State.eState_SelectField:
					SelectField();
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

			await UniTask.Yield();
		}
	}

	// 初期化
	void Init()
	{
		if (m_StateValue == 1)
		{
			// 操作側
			Side operateSide = BattleUserMgr.instance.GetSetOperateUserSide;

			// 操作側なら参加する場を選択するステートに飛ばす
			if (operateSide == Side.eSide_Player)
			{
				// 参加する場を選択へ
				SetNextStateAndFlag(State.eState_SelectField);
			}
			// 操作側じゃないなら通信同期へ
			else
			{

			}
		}
	}
	// 参加する場を選択
	void SelectField()
	{
		if (m_StateValue == 1)
		{
			// 選択したカードエリアを初期化。
			m_SelectedCardArea = null;
		}

		// 場に空きがないなら終了へ
		if (BattleStageMgr.instance.GetCardAreaNumFromEmptyCard() <= 0)
		{
			// 終了へ
			SetNextStateAndFlag(State.eState_End);
			return;
		}

		// 左クリックをしたら。
		if (Input.GetMouseButtonDown(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			foreach (RaycastHit hit in Physics.RaycastAll(ray))
			{
				// カードエリアをクリックしているなら。
				if (hit.collider.tag == "CardArea")
				{
					// 操作側
					Side operateSide = BattleUserMgr.instance.GetSetOperateUserSide;

					// ユーザーを取得(アクション側)
					BattleUser battleUser = BattleUserMgr.instance.GetUser(GetSetActionSide);
					// カードエリアを取得。
					GameObject cardAreaObject = hit.transform.gameObject;

					// 選択したカードエリア設定
					if (cardAreaObject)
					{
						CardArea cardArea = hit.transform.gameObject.GetComponent<CardArea>();

						// 自分が操作側じゃないならカードエリアの選択を行わない
						if (operateSide != Side.eSide_Player) { return; }
						// 既にカードが存在しているならここは選択できない
						if (!cardArea.IsCardEmpty()) { return; }

						m_SelectedCardArea = cardArea;
					}
				}
			}
		}

		// カードエリアを選択しているならメイン処理へ。
		if (m_SelectedCardArea != null)
		{
			// 国民カード追加へ
			SetNextStateAndFlag(State.eState_JoinPeople);
			return;
		}
	}
	// 国民カード追加
	void JoinPeople()
	{
		if (m_StateValue == 1)
		{
			// 追加するカードエリア
			CardArea joinArea = m_SelectedCardArea;

			if (joinArea != null)
			{
				bool isSpy = BattleMgr.instance.IsNextJoinSpyFlag();
				// 国民カード追加
				BattleCardCtr.instance.CreateBattleCard(joinArea, BattleCard.Kind.eKind_People, isSpy);

				// スパイ追加フラグが立っているなら初期化
				if (isSpy)
				{
					BattleMgr.instance.SetNextJoinSpyFlag(false);
				}
			}
		}
		// 終了へ
		SetNextStateAndFlag(State.eState_End);
	}

	// 終了
	void End()
	{
		if (m_StateValue == 1)
		{
			// 実行完了後に完了フラグを立てる
			IsCompleted = true;
			// アクション完了の通知
			OnActionCompleted?.Invoke(this);
		}
	}

	async UniTask StateLoop()
	{
		while (true)
		{
			// ステート更新処理
			// 次のステート更新(今のステートに設定)
			GetSetNextState = GetSetState;
			// 次のステートに移動するフラグ初期化
			m_NextStateFlag = false;

			switch (GetSetState)
			{
				case State.eState_Init:
					GetSetNextState = await StateInit();
					break;
				case State.eState_SelectField:
					GetSetNextState = await StateSelectField();
					break;
				case State.eState_JoinPeople:
					GetSetNextState = await StateJoinPeople();
					break;
				case State.eState_End:
					GetSetNextState = await StateEnd();
					break;
				default:
					Debug.Log("StateLoopに記載されていないフェイズに遷移しようとしています");
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
		await UniTask.WaitUntil(() => GetSetNextStateFlag);
		// 次のステートへ
		return GetSetNextState;
	}
	// 参加させる場を選択
	async UniTask<State> StateSelectField()
	{
		await UniTask.WaitUntil(() => GetSetNextStateFlag);
		// 次のステートへ
		return GetSetNextState;
	}
	// 国民カード追加
	async UniTask<State> StateJoinPeople()
	{
		await UniTask.WaitUntil(() => GetSetNextStateFlag);
		// 次のステートへ
		return GetSetNextState;
	}
	// 終了
	async UniTask<State> StateEnd()
	{
		await UniTask.WaitUntil(() => GetSetNextStateFlag);
		// 次のステートへ
		return GetSetNextState;
	}

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
	public bool GetSetNextStateFlag
	{
		get { return m_NextStateFlag; }
		set { m_NextStateFlag = value; }
	}
	public int GetSetStateValue
	{
		get { return m_StateValue; }
		set { m_StateValue = value; }
	}

	// --フラグ--
	public void SetNextStateFlag()
	{
		GetSetNextStateFlag = true;
		GetSetStateValue = 0;
	}

	// 次のステートとフラグを設定
	public void SetNextStateAndFlag(State _nextState)
	{
		GetSetNextState = _nextState;
		SetNextStateFlag();
	}
}
