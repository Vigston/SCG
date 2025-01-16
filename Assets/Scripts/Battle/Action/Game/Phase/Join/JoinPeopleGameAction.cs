using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using battleTypes;
using Photon.Pun;

// 国民を参加させるアクション
public class JoinPeopleGameAction : MonoBehaviourPunCallbacks, IGameAction
{
	// ===構造体===
	public enum State
	{
		eState_Init,		// 初期化
		eState_SelectField, // 参加する場を選択
		eState_JoinPeople,  // 国民カード追加
		eState_NetWorkSync,	// 通信同期
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
	// 通信同期フラグ
	bool m_NetWorkSyncFlag = false;

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
				case State.eState_NetWorkSync:
					NetWorkSync();
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
			Side operateSide = BattleUserMgr.instance.GetSetOperateSide;

			// 通信を行っていて
			if(PhotonNetwork.IsConnected)
			{
				// 自分が操作側なら
				if (BattleUserMgr.instance.IsOperateSide(Side.eSide_Player))
				{
					// 参加する場を選択へ
					SetNextStateAndFlag(State.eState_SelectField);
					return;
				}
				// 自分が操作側じゃないなら
				else
				{
					// 通信同期へ
					SetNextStateAndFlag(State.eState_NetWorkSync);
					return;
				}
			}
			// 通信を行っていないなら
			else
			{
				// 参加する場を選択へ
				SetNextStateAndFlag(State.eState_SelectField);
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
					Side operateSide = BattleUserMgr.instance.GetSetOperateSide;

					// ユーザーを取得(アクション側)
					BattleUser battleUser = BattleUserMgr.instance.GetUser(GetSetActionSide);
					// カードエリアを取得。
					GameObject cardAreaObject = hit.transform.gameObject;

					// 選択したカードエリア設定
					if (cardAreaObject)
					{
						CardArea cardArea = hit.transform.gameObject.GetComponent<CardArea>();

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

	// 国民カード追加
	void NetWorkSync()
	{
		// 操作側
		Side operateSide = BattleUserMgr.instance.GetSetOperateSide;

		if (m_StateValue == 1)
		{
			// 操作側なら
			if (operateSide == Side.eSide_Player)
			{
				int cardAreaViewID = m_SelectedCardArea.GetComponent<PhotonView>().ViewID;
				bool isSpy = BattleMgr.instance.IsNextJoinSpyFlag();

				m_NetWorkSyncFlag = true;

				photonView.RPC("OnJoinPeopleCard", RpcTarget.Others, cardAreaViewID, isSpy);
			}
			// それ以外なら通信同期待ち
		}

		if(m_NetWorkSyncFlag)
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
					Debug.Log("JoinPeopleGameAction：eState_Init");
					GetSetNextState = await StateInit();
					break;
				case State.eState_SelectField:
					Debug.Log("JoinPeopleGameAction：eState_SelectField");
					GetSetNextState = await StateSelectField();
					break;
				case State.eState_JoinPeople:
					Debug.Log("JoinPeopleGameAction：eState_JoinPeople");
					GetSetNextState = await StateJoinPeople();
					break;
				case State.eState_NetWorkSync:
					Debug.Log("JoinPeopleGameAction：eState_NetWorkSync");
					GetSetNextState = await StateNetWorkSync();
					break;
				case State.eState_End:
					Debug.Log("JoinPeopleGameAction：eState_End");
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
	// 通信同期
	async UniTask<State> StateNetWorkSync()
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

	/////Photon/////
	// 国民カードが追加された際に情報を他クライアントに送信
	[PunRPC]
	void OnJoinPeopleCard(int _cardAreaViewID, bool _isSpy)
	{
		if (PhotonView.Find(_cardAreaViewID) == null) { return; }

		CardArea cardArea = PhotonView.Find(_cardAreaViewID).GetComponent<CardArea>();

		if (cardArea != null) { return; }

		m_NetWorkSyncFlag = true;

		// 国民カード追加
		BattleCardCtr.instance.CreateBattleCard(cardArea, BattleCard.Kind.eKind_People, _isSpy);
	}

	// 通信同期が正常に行われているか確認
	[PunRPC]
	bool IsSuccessNetWorkSync(int _cardAreaViewID, bool _NetWorkSyncFlag)
	{
		// 1. 指定された ViewID の PhotonView が存在するか確認
		PhotonView view = PhotonView.Find(_cardAreaViewID);
		if (view == null)
		{
			Debug.LogError($"PhotonView with ViewID {_cardAreaViewID} not found.");
			return false;
		}

		// 2. 指定された ViewID が CardArea に関連付けられているか確認
		CardArea cardArea = view.GetComponent<CardArea>();
		if (cardArea == null)
		{
			Debug.LogError($"CardArea not found for ViewID {_cardAreaViewID}.");
			return false;
		}

		// 3. お互いの通信同期フラグが立っているか確認
		if (!m_NetWorkSyncFlag || !_NetWorkSyncFlag)
		{
			Debug.LogError("Network sync flag is not set.");
			return false;
		}

		// 4. カードエリアにカードが存在するか確認
		if (cardArea.IsCardEmpty())
		{
			Debug.LogError("CardArea is empty. Card sync failed.");
			return false;
		}

		// 5. 同期が成功している場合
		Debug.Log("Network sync successful: Card is present in the specified CardArea.");
		return true;
	}
}
