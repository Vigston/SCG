using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System;
using UnityEngine;

public class CardAbilityManager : MonoBehaviour
{
	// インスタンス
	public static CardAbilityManager instance;

	[SerializeReference]
	private List<ICardAbility> activeAbilityList;

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

		activeAbilityList.Add(ability);									// 追加
		ability.OnCompleted += _ => activeAbilityList.Remove(ability);	// 終了時にリストから削除
		ability.ExecuteAsync().Forget();								// 非同期実行
		return ability;
	}

	public async UniTask WaitForAbility(ICardAbility ability)
	{
		if(ability == null)
		{
			Debug.LogWarning($"{nameof(WaitForAbility)}の引数(ability)でNULLが検知されましたので処理を終了しました。");
			return;
		}

		if (!ability.IsRunning)
		{
			var tcs = new UniTaskCompletionSource();
			ability.OnCompleted += _ => tcs.TrySetResult();
			await tcs.Task;
		}
	}
}
