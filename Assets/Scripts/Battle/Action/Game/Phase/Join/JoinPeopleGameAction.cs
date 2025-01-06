
using Cysharp.Threading.Tasks;
using System.Threading;

public class JoinPeopleGameAction : GameAction
{
	public override async UniTask Execute(CancellationToken _cancellationToken)
	{
		// 5秒間待機(キャンセル可)
		await UniTask.Delay(5000, cancellationToken: _cancellationToken);
	}
}
