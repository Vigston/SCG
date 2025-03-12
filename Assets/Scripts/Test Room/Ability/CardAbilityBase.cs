using Cysharp.Threading.Tasks;
using System;

public abstract class CardAbilityBase : ICardAbility
{
	public bool IsRunning { get; private set; }
	public event Action<ICardAbility> OnCompleted;

	public async UniTask ExecuteAsync()
	{
		IsRunning = true;
		await Execute();
		IsRunning = false;
		OnCompleted?.Invoke(this);
	}

	protected abstract UniTask Execute();
}