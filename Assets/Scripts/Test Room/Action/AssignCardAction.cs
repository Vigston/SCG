using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

public class AssignCardAction : ActionBase
{
	private Transform targetTransform;

	public AssignCardAction(Transform _targetTransform)
	{
		this.targetTransform = _targetTransform;
	}

	protected override async UniTask Execute()
	{
		// カード生成
		var card = CardMgr.Instance.CreateCard("Military Card 1");

		if (card == null) return;

		var anim = new AssignCardAnim(card.transform, targetTransform, 1f, 1.0f);
		await anim.PlayAsync(); // ←ここで待てる！
	}
}
