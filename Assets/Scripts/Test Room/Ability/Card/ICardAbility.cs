using Cysharp.Threading.Tasks;
using System;

public interface ICardAbility
{
	bool IsRunning { get; }
	UniTask ExecuteAsync();
	event Action<ICardAbility> OnCompleted;  // イベントを追加
}