using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

// 国民を参加させるアクション
[System.Serializable]
public class JoinPeopleGameAction : IGameAction
{
	// イベント
	public event Action<IAction> OnActionCompleted;

	public bool IsCompleted { get; private set; }

	// 実行処理
	public async UniTask Execute(CancellationToken _cancellationToken)
	{
		Debug.Log("JoinPeopleGameActionの実行処理開始");
		// 5秒間待機(キャンセル可)
		await UniTask.Delay(5000, cancellationToken: _cancellationToken);

		// 実行完了後に完了フラグを立てる
		IsCompleted = true;

		OnActionCompleted?.Invoke(this); // アクション完了の通知
		Debug.Log("JoinPeopleGameActionの実行処理終了");
	}

	// アクション完了時の処理
	public void End(IAction _action)
	{

	}
}
