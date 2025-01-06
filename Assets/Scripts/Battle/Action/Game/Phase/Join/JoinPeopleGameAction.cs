using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

// 国民を参加させるアクション
public class JoinPeopleGameAction : IGameAction
{
	// 実行処理
	public async UniTask Execute(CancellationToken _cancellationToken)
	{
		// 5秒間待機(キャンセル可)
		await UniTask.Delay(5000, cancellationToken: _cancellationToken);
	}

	// アクション完了時の処理
	public void OnActionCompleted(IAction _action)
	{
		Debug.Log($"アクション完了時の処理：{_action}");
	}
}
