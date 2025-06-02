using UnityEditor;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;

[CustomEditor(typeof(AnimationTester))]
public class AnimationTesterEditor : Editor
{
	private int dicerollTestCount;

	// AnimationTypeと再生関数の紐付け
	private Dictionary<AnimationTester.AnimationType, Func<UniTask>> animationMap;

	private void OnEnable()
	{
		animationMap = new Dictionary<AnimationTester.AnimationType, Func<UniTask>>
		{
			{ AnimationTester.AnimationType.DiceRoll, PlayDiceRollAnim }
            // 追加アニメーションはここに登録
        };
	}

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		AnimationTester tester = (AnimationTester)target;

		if (GUILayout.Button("アニメーション再生"))
		{
			// プレイモードじゃないなら処理を行わない
			if (!EditorApplication.isPlaying) return;
			PlayAnimation(tester).Forget();
		}
	}

	private async UniTaskVoid PlayAnimation(AnimationTester tester)
	{
		if (animationMap != null && animationMap.TryGetValue(tester.animationType, out var playFunc))
		{
			await playFunc();
			Debug.Log("アニメーション完了");
		}
		else
		{
			Debug.LogWarning("未対応のアニメーションタイプです");
		}
	}

	// --- ここにアニメーションごとの関数を定義 ---
	private async UniTask PlayDiceRollAnim()
	{
		dicerollTestCount++;

		GameObject dicePrefab = null;
		GameObject diceObj_First = null;
		GameObject diceObj_Second = null;

		if (dicerollTestCount < 1) dicerollTestCount = 1;
		else if (dicerollTestCount > 12) dicerollTestCount = 1;

		int diceResult_First = dicerollTestCount > 6 ? 6 : dicerollTestCount; // サイコロの目の値を設定
		int diceResult_Second = dicerollTestCount > 6 ? dicerollTestCount - 6 : 0;

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
			if (dicerollTestCount > 6)
			{
				spawnPos_First -= camera.transform.right * 1.0f;
			}


			Quaternion spawnRot = Quaternion.identity;
			diceObj_First = GameObject.Instantiate(prefab, spawnPos_First, spawnRot) as GameObject;

			// 2つ目のサイコロが必要な場合にもう一つ生成
			if (dicerollTestCount > 6)
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

		Debug.Log($"サイコロの目：{dicerollTestCount}");

		AnimationSequence animSeq = new AnimationSequence();

		animSeq.Add(new DiceRollAnim(diceObj_First.transform, diceResult_First, 0.5f));
		if (dicerollTestCount > 6)
		{
			animSeq.Add(new DiceRollAnim(diceObj_Second.transform, diceResult_Second, 0.5f));
		}

		await animSeq.PlayParallelAsync();

		// サイコロを削除、アセット解放
		if (diceObj_First != null)
		{
			GameObject.Destroy(diceObj_First);        // 削除
			GameObject.Destroy(diceObj_Second);        // 削除
			AssetManager.Release(dicePrefab);   // 解放
		}
	}
}