using Cysharp.Threading.Tasks;
using System;

public interface IGameAbility
{
	bool IsRunning { get; }
	UniTask ExecuteAsync();
	event Action<IGameAbility> OnCompleted;  // イベントを追加
}