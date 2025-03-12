using Cysharp.Threading.Tasks;
using System;

public abstract class ActionBase
{
	public bool IsCompleted { get; protected set; }
	public event Action<ActionBase> OnCompleted;

	public async UniTask ExecuteAsync()
	{
		IsCompleted = false;
		await Execute();
		IsCompleted = true;
		OnCompleted?.Invoke(this);
	}

	protected abstract UniTask Execute();
}