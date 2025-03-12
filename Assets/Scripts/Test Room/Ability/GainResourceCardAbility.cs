using Cysharp.Threading.Tasks;

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