using Cysharp.Threading.Tasks;
using System;

public class GainResourceCardAbility : CardAbilityBase
{
	private int value;

	public GainResourceCardAbility(int value)
	{
		this.value = value;
	}

	protected override async UniTask Execute()
	{
		var action = ActionManager.instance.ActivateAction<GainResourceAction>(value);
		await action.ExecuteAsync();
	}
}