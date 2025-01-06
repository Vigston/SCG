using Cysharp.Threading.Tasks;
using System.Threading;
using battleTypes;
using System;

public interface IAction
{
	// アクション実行処理
	UniTask Execute(CancellationToken _cancellationToken);

	// アクション完了時の処理
	void OnActionCompleted(IAction _action);
}
