using Cysharp.Threading.Tasks;
using System.Threading;

public abstract class Action
{
	// アクション実行処理
	public abstract UniTask Execute(CancellationToken _cancellationToken);
}
