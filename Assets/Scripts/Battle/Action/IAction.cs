using Cysharp.Threading.Tasks;
using System.Threading;
using battleTypes;
using System;

public interface IAction
{
	// イベント
	public event Action<IAction> OnActionCompleted;

	// アクション実行処理
	UniTask Execute(CancellationToken _cancellationToken);

	// アクション完了時の処理
	void End(IAction _action);

	// アクションの完了状態
	bool IsCompleted { get; }
}
