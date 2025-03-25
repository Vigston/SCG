using Cysharp.Threading.Tasks;
using System;

[Serializable]
public abstract class GameAbilityBase : IGameAbility
{
	public bool IsRunning { get; private set; }
	public event Action<IGameAbility> OnCompleted;

	public async UniTask ExecuteAsync()
	{
		IsRunning = true;
		await Execute();
		IsRunning = false;
		OnCompleted?.Invoke(this);
	}

	protected abstract UniTask Execute();
}