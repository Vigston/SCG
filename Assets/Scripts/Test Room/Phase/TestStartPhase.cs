﻿using System.Collections.Generic;
using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Photon.Pun;

public class TestStartPhase : Phase
{
	// 独自のState列挙型
	public enum StartPhaseState
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
			{ StartPhaseState.StartState, StartStateAction },
			{ StartPhaseState.MainState, MainStateAction },
			{ StartPhaseState.EndState, EndStateAction }
		};
	}

	// フェイズ進行中の処理
	public override async UniTask UpdatePhase()
	{
		// ターン終了じゃなければフェイズ更新を行う
		while ((StartPhaseState)GetSetState != StartPhaseState.TurnEndState)
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
			if (((StartPhaseState)bufferState != (StartPhaseState)GetSetState))
			{
				Debug.Log($"ステート遷移検知。{this}：{nameof(UpdatePhase)}：{GetSetState.ToString()}");
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
		SwitchState(StartPhaseState.MainState);
		await UniTask.Yield();
	}

	private async UniTask MainStateAction()
	{
		if (IsFirstState())
		{
			Debug.Log($"{this}：{nameof(MainStateAction)}");
		}

		// 終了へ
		SwitchState(StartPhaseState.EndState);
		await UniTask.Yield();
	}

	private async UniTask EndStateAction()
	{
		if (IsFirstState())
		{
			Debug.Log($"{this}：{nameof(EndStateAction)}");
		}

		// ターン終了
		SwitchState(StartPhaseState.TurnEndState);
		await UniTask.Yield();
	}

	// 初期状態
	public override Enum GetInitState()
	{
		// 開始
		return StartPhaseState.StartState;
	}

	// 終了状態
	public override Enum GetEndState()
	{
		// 終了
		return StartPhaseState.TurnEndState;
	}
}