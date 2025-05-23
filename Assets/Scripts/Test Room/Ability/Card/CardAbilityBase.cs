﻿using Cysharp.Threading.Tasks;
using System;

[Serializable]
public abstract class CardAbilityBase : ICardAbility
{
	public abstract int Id { get; protected set; }
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