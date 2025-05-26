using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System;
using UnityEngine;
using Photon.Pun;
using System.Threading;
using UnityEditor.SceneManagement;

public class TestMainPhase : Phase
{
	// 独自のState列挙型
	public enum MainPhaseState
	{
		StartState,             // 開始
		MainState,              // メイン
		EndState,               // 終了

		TurnEndState = -1,      // ターン終了
	}

	// アクション
	AssignCardAction m_AssignCardAction;    // カード割り当てアクション

	private void Awake()
	{
		// ステートとアクションを辞書型で紐づける
		GetSetActionDict = new Dictionary<Enum, Func<UniTask>>()
		{
			{ MainPhaseState.StartState, StartStateAction },
			{ MainPhaseState.MainState, MainStateAction },
			{ MainPhaseState.EndState, EndStateAction }
		};
	}

	// フェイズ進行中の処理
	public override async UniTask UpdatePhase(CancellationToken token)
	{
		// ターン終了じゃなければフェイズ更新を行う
		while ((MainPhaseState)GetSetState != MainPhaseState.TurnEndState)
		{
			Enum bufferState = GetSetState;

			// マスタークライアントでアクションを実行
			if (PhotonNetwork.IsMasterClient || Test_DebugMgr.Instance.isSingleDebug)
			{
				// 状態遷移の処理はステートマシン側で行われるので、ここでアクションを実行
				if (GetSetActionDict.TryGetValue(GetSetState, out Func<UniTask> stateAction))
				{
					await stateAction();
				}
			}

			// フェイズフレーム加算
			GetSetPhaseFrame++;

			// ステート遷移しているならステート遷移時の処理を行って次のループへ
			if ((MainPhaseState)bufferState != (MainPhaseState)GetSetState)
			{
				CardAbilityManager abilityManager = CardAbilityManager.instance;
				// アビリティキューが空になるまで待機
				await UniTask.WaitUntil(() => abilityManager.IsQueueEmpty() && !abilityManager.IsExecuting());

				// ステート遷移時の処理
				OnSwitchState();
			}
			// ステート遷移していないならステートフレーム加算
			else
			{
				// ステートフレーム加算
				GetSetStateFrame++;
			}

			// 次フレーム待機
			await UniTask.Yield();
		}

		// タスク終了
		await UniTask.CompletedTask;
	}

	/////////////////////
	//////アクション/////
	/////////////////////
	// 状態ごとのアクションを定義
	private async UniTask StartStateAction()
	{
		if (IsFirstState())
		{
			Debug.Log($"{this}：{nameof(StartStateAction)}");
		}

		// メインへ
		SwitchState(MainPhaseState.MainState);
		await UniTask.Yield();
	}

	private async UniTask MainStateAction()
	{
		if (IsFirstState())
		{
			Debug.Log($"{this}：{nameof(MainStateAction)}");

			if(PhotonNetwork.IsMasterClient || Test_DebugMgr.Instance.isSingleDebug)
			{
				Test_CardArea targetCardArea = Test_StageMgr.instance.GetCardAreaFromIndex(1);
				m_AssignCardAction = ActionManager.instance.ActivateAction<AssignCardAction>(targetCardArea.transform);
			}
		}

		await ActionManager.instance.WaitForAction(m_AssignCardAction);

		// 終了へ
		SwitchState(MainPhaseState.EndState);
		await UniTask.Yield();
	}

	private async UniTask EndStateAction()
	{
		if (IsFirstState())
		{
			Debug.Log($"{this}：{nameof(EndStateAction)}");
		}

		// ターン終了
		SwitchState(MainPhaseState.TurnEndState);
		await UniTask.Yield();
	}

	// 初期状態
	public override Enum GetInitState()
	{
		// 開始
		return MainPhaseState.StartState;
	}

	// 終了状態
	public override Enum GetEndState()
	{
		// 終了
		return MainPhaseState.TurnEndState;
	}
}