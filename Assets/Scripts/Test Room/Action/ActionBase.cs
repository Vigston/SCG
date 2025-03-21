using Cysharp.Threading.Tasks;
using System;

[Serializable]
public abstract class ActionBase : TestIAction
{
	public bool IsRunning { get; private set; }
	public event Action<TestIAction> OnCompleted;

	public async UniTask ExecuteAsync()
	{
		IsRunning = true;
		await Execute();
		IsRunning = false;
		OnCompleted?.Invoke(this);
	}

	protected abstract UniTask Execute();
}