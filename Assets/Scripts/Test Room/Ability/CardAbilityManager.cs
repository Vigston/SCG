using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System;
using UnityEngine;

public class CardAbilityManager : MonoBehaviour
{
	// インスタンス
	public static CardAbilityManager instance;

	private List<ICardAbility> activeAbilities = new List<ICardAbility>();

	private void Awake()
	{
		// インスタンス生成
		CreateInstance();
	}

	// インスタンスを作成
	public bool CreateInstance()
	{
		// 既にインスタンスが作成されていなければ作成する
		if (!instance)
		{
			// 作成
			instance = this;
		}

		// インスタンスが作成済みなら終了
		if (instance) { return true; }

		Debug.LogError($"{this}のインスタンスが生成できませんでした");
		return false;
	}

	public T ActivateAbility<T>(params object[] args) where T : ICardAbility
	{
		var ability = (T)Activator.CreateInstance(typeof(T), args);
		Debug.Log($"{typeof(T).Name} を発動しました");

		activeAbilities.Add(ability);
		ability.ExecuteAsync().Forget(); // 非同期実行
		return ability;
	}

	public async UniTask WaitForAbility(ICardAbility ability)
	{
		if (!ability.IsRunning)
		{
			var tcs = new UniTaskCompletionSource();
			ability.OnCompleted += _ => tcs.TrySetResult();
			await tcs.Task;
		}
	}
}
