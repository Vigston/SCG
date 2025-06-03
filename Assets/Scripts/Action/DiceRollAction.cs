using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

public class DiceRollAction : ActionBase
{
	private const int DICE_FACE_COUNT = 6; // サイコロの目の数

	private int diceNum; // サイコロの数
	private int diceResultNum;

	public DiceRollAction(int _diceNum)
	{
		this.diceNum = _diceNum;
	}
	protected override async UniTask Execute()
	{
		List<GameObject> diceNumObjList = new List<GameObject>();

		GameObject diceObj_First = null;
		GameObject diceObj_Second = null;

		// プレハブの参照を保持する変数
		GameObject dicePrefab = null;

		var tcs = new UniTaskCompletionSource();

		AssetManager.Load<GameObject>("Dice_D6_White",
		prefab =>
		{
			dicePrefab = prefab;
			// カメラ前方4ユニットの位置に生成
			var camera = Camera.main;
			Vector3 spawnPos_First = camera.transform.position + camera.transform.forward * 10.0f;
			Vector3 spawnPos_Second = camera.transform.position + camera.transform.forward * 10.0f + camera.transform.right * 1.0f;

			// 2つ目のサイコロが必要な場合は、少し左にずらす
			if (diceNum == 2)
			{
				spawnPos_First -= camera.transform.right * 1.0f;
			}


			Quaternion spawnRot = Quaternion.identity;
			diceObj_First = GameObject.Instantiate(prefab, spawnPos_First, spawnRot) as GameObject;

			// 2つ目のサイコロが必要な場合にもう一つ生成
			if (diceNum == 2)
			{
				diceObj_Second = GameObject.Instantiate(prefab, spawnPos_Second, spawnRot) as GameObject;
			}
			tcs.TrySetResult(); // 非同期処理完了を通知
		},
		error =>
		{
			Debug.LogError($"Dice prefab load failed: {error}");
			tcs.TrySetException(error);
		});

		await tcs.Task; // AssetManager.Load の完了を待つ

		// サイコロのNULLチェック
		if (diceObj_First == null)
		{
			Debug.LogError("サイコロの生成に失敗しました。");
			return;
		}
		if(diceNum == 2 && diceObj_Second == null)
		{
			Debug.LogError("2つ目のサイコロの生成に失敗しました。");
			return;
		}

		if(diceNum == 1)
		{
			diceResultNum = Random.Range(1, DICE_FACE_COUNT + 1); // サイコロの目をランダムに決定
		}
		else if(diceNum == 2)
		{
			diceResultNum = Random.Range(7, (DICE_FACE_COUNT * 2) + 1); // サイコロの目をランダムに決定
		}
		else
		{
			Debug.LogError("サイコロの数は1または2でなければなりません。");
			return;
		}

		Debug.Log($"サイコロの目：{diceResultNum}");

		AnimationSequence animSeq = new AnimationSequence();

		animSeq.Add(new DiceRollAnim(diceObj_First.transform, diceResultNum > DICE_FACE_COUNT ? 6 : diceResultNum, 0.5f)); // 1つ目のサイコロのアニメーションを追加
		if (diceNum == 2)
		{
			animSeq.Add(new DiceRollAnim(diceObj_Second.transform, diceResultNum > DICE_FACE_COUNT ? diceResultNum - DICE_FACE_COUNT : 0, 0.5f)); // 2つ目のサイコロのアニメーションを追加
		}

		await animSeq.PlayParallelAsync(); // アニメーションを非同期で実行

		// サイコロを削除、アセット解放
		if (diceObj_First != null)
		{
			GameObject.Destroy(diceObj_First);		// 削除
		}
		if(diceObj_Second != null)
		{
			GameObject.Destroy(diceObj_Second);		// 削除
		}
		AssetManager.Release(dicePrefab);			// 解放
	}
}
