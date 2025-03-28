using Cysharp.Threading.Tasks;

public class GainResourceCardAbility : CardAbilityBase
{
	public override int Id { get; protected set; }
	private int value;

	public GainResourceCardAbility(int _id, int _value)
	{
		this.Id = _id;
		this.value = _value;
	}

	protected override async UniTask Execute()
	{
		var action = ActionManager.instance.ActivateAction<GainResourceAction>(value);
		await action.ExecuteAsync();
	}
}