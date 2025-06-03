using UnityEngine;
using Cysharp.Threading.Tasks;

public class DamageCardAbility : CardAbilityBase
{
	public override int Id { get; protected set; }
	private int damage;

	public DamageCardAbility(int _id, int _damage)
	{
		this.Id = _id;
		this.damage = _damage;
	}

	protected override async UniTask Execute()
	{
		Debug.Log($"{this.ToString()} {nameof(Execute)}");
		var action = ActionMgr.instance.ActivateAction<DamageAction>(damage);
		await ActionMgr.instance.WaitForAction(action);
	}
}