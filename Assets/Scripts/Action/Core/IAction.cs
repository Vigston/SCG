﻿using Cysharp.Threading.Tasks;
using System;

public interface IAction
{
	bool IsRunning { get; }
	UniTask ExecuteAsync();
	event Action<IAction> OnCompleted;  // イベントを追加
}