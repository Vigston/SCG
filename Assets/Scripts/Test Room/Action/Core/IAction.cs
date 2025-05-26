using Cysharp.Threading.Tasks;
using System;

public interface TestIAction
{
	bool IsRunning { get; }
	UniTask ExecuteAsync();
	event Action<TestIAction> OnCompleted;  // イベントを追加
}