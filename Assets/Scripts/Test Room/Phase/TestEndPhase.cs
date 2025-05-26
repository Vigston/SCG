using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System;
using UnityEngine;
using Photon.Pun;
using System.Threading;

public class TestEndPhase : Phase
{
	// 独自のState列挙型
	public enum EndPhaseState
	{
		StartState,             // 開始
		MainState,              // メイン
		EndState,               // 終了

		TurnEndState = -1,      // ターン終了
	}

	private void Awake()
	{
		// ステートとアクションを辞書型で紐づける
		GetSetActionDict = new Dictionary<Enum, Func<UniTask>>()
		{
			{ EndPhaseState.StartState, StartStateAction },
			{ EndPhaseState.MainState, MainStateAction },
			{ EndPhaseState.EndState, EndStateAction }
		};
	}

	// フェイズ進行中の処理
	public override async UniTask UpdatePhase(CancellationToken token)
	{
		// ターン終了じゃなければフェイズ更新を行う
		while ((EndPhaseState)GetSetState != EndPhaseState.TurnEndState)
		{
			Enum bufferState = GetSetState;

			// マスタークライアントでアクションを実行
			if (PhotonNetwork.IsMasterClient || Test_DebugMgr.Instance.isSingleDebug)
			{
				// 状態遷移の処理はステートマシン側で行われるので、ここでアクションを実行
				if (GetSetActionDict.TryGetValue(GetSetState, out Func<UniTask> stateAction))
				{
					stateAction();
				}
			}

			// フェイズフレーム加算
			GetSetPhaseFrame++;

			// ステート遷移しているならステート遷移時の処理を行って次のループへ
			if ((EndPhaseState)bufferState != (EndPhaseState)GetSetState)
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
		SwitchState(EndPhaseState.MainState);
		await UniTask.Yield();
	}

	private async UniTask MainStateAction()
	{
		if (IsFirstState())
		{
			Debug.Log($"{this}：{nameof(MainStateAction)}");
		}

		// 終了へ
		SwitchState(EndPhaseState.EndState);
		await UniTask.Yield();
	}

	private async UniTask EndStateAction()
	{
		if (IsFirstState())
		{
			Debug.Log($"{this}：{nameof(EndStateAction)}");
		}

		// ターン終了
		SwitchState(EndPhaseState.TurnEndState);
		await UniTask.Yield();
	}

	// 初期状態
	public override Enum GetInitState()
	{
		// 開始
		return EndPhaseState.StartState;
	}

	// 終了状態
	public override Enum GetEndState()
	{
		// 終了
		return EndPhaseState.TurnEndState;
	}
}