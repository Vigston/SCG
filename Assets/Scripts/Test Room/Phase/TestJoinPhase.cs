using System.Collections.Generic;
using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using static TestStartPhase;

public class TestJoinPhase : Phase
{
	// 独自のState列挙型
	public enum JoinPhaseState
	{
		StartState,             // 開始
		MainState,              // メイン
		EndState,               // 終了

		TurnEndState = -1,      // ターン終了
	}

	private void Awake()
	{
		// ステートとアクションを辞書型で紐づける
		GetSetActionDict = new Dictionary<Enum, Action>()
		{
			{ JoinPhaseState.StartState, StartStateAction },
			{ JoinPhaseState.MainState, MainStateAction },
			{ JoinPhaseState.EndState, EndStateAction }
		};
	}

	// フェイズ進行中の処理
	public override async UniTask UpdatePhase()
	{
		while ((JoinPhaseState)GetSetState != JoinPhaseState.TurnEndState)
		{
			Enum bufferState = GetSetState;

			// 状態遷移の処理はステートマシン側で行われるので、ここでアクションを実行
			if (GetSetActionDict.TryGetValue(GetSetState, out Action stateAction))
			{
				stateAction();
			}

			// フェイズフレーム加算
			GetSetPhaseFrame++;

			// ステート遷移しているならステート遷移時の処理を行って次のループへ
			if ((JoinPhaseState)bufferState != (JoinPhaseState)GetSetState)
			{
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
	private void StartStateAction()
	{
		if (IsFirstState())
		{
			Debug.Log($"{this}：{nameof(StartStateAction)}");
		}

		// メインへ
		SwitchState(JoinPhaseState.MainState);
	}

	private void MainStateAction()
	{
		if (IsFirstState())
		{
			Debug.Log($"{this}：{nameof(MainStateAction)}");
		}

		// 終了へ
		SwitchState(JoinPhaseState.EndState);
	}

	private void EndStateAction()
	{
		if (IsFirstState())
		{
			Debug.Log($"{this}：{nameof(EndStateAction)}");
		}

		// ターン終了
		SwitchState(JoinPhaseState.TurnEndState);
	}

	// 初期状態
	public override Enum GetInitState()
	{
		// 開始
		return JoinPhaseState.StartState;
	}
}