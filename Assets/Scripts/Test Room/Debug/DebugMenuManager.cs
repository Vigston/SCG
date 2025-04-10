using System;
using System.Collections.Generic;
using UnityEngine;

public class DebugMenuManager : MonoBehaviour
{
	public static DebugMenuManager Instance;

	[SerializeField] private int debugAreaX = 10;        // GUIエリアのX座標
	[SerializeField] private int debugAreaY = 10;        // GUIエリアのY座標
	[SerializeField] private float debugAreaHeight = 500f;  // GUIエリアの高さ
	[SerializeField] private float debugAreaWidth = 300f;   // GUIエリアの幅

	private int variablesPerPage = 19; // 1ページに表示する変数の数
	private List<DebugVariable> variables = new();
	private int currentPage = 0;
	private bool isVisible = false;

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		//for (int i = 0; i < 90; i++)
		//{
		//	int index = i;
		//	DebugMenuManager.Instance.RegisterVariable($"Test Variable {index}", () => $"Value {index}");
		//}
	}

	private void Update()
	{
		// デバッグメニューの表示/非表示を切り替える
		if (Input.GetKeyDown(KeyCode.F1)) isVisible = !isVisible;

		// ページ切り替え処理
		if (variablesPerPage > 0)
		{
			if (Input.GetKeyDown(KeyCode.RightArrow)) currentPage++;
			if (Input.GetKeyDown(KeyCode.LeftArrow)) currentPage--;
		}
	}

	private void OnGUI()
	{
		if (!isVisible) return;

		GUILayout.BeginArea(new Rect(debugAreaX, debugAreaY, debugAreaWidth, debugAreaHeight), GUI.skin.box);

		// デバッグメニューのタイトル
		GUILayout.Label($"Debug Menu - Page {currentPage + 1}");

		// 変数が登録されていれば表示
		if (variablesPerPage > 0)
		{
			// 最大ページ数を計算し、currentPage を制限
			int maxPage = Mathf.Max(0, (variables.Count - 1) / variablesPerPage);
			currentPage = Mathf.Clamp(currentPage, 0, maxPage);

			// 現在のページで表示する変数の開始位置と終了位置を計算
			int start = currentPage * variablesPerPage;
			int end = Mathf.Min(start + variablesPerPage, variables.Count);

			// 変数を表示
			for (int i = start; i < end; i++)
			{
				var v = variables[i];
				GUILayout.Label($"{v.Name}: {v.ValueGetter()}");
			}
		}

		GUILayout.EndArea();
	}

	// 変数を登録する
	public void RegisterVariable(string name, Func<object> valueGetter)
	{
		variables.Add(new DebugVariable(name, valueGetter));
	}
}