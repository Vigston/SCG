using UnityEngine;
using Cysharp.Threading.Tasks;

public class DamageCardAbility : CardAbilityBase
{
	private int damage;

	public DamageCardAbility(int damage)
	{
		this.damage = damage;
	}

	protected override async UniTask Execute()
	{
		Debug.Log($"{this.ToString()} {nameof(Execute)}");
		var action = ActionManager.instance.ActivateAction<DamageAction>(damage);
		await action.ExecuteAsync();
	}
}