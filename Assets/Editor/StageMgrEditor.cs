using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(StageMgr))]
public class Test_StageMgrEditor : Editor
{
	public override void OnInspectorGUI()
	{
		// デフォルトのインスペクターを描画
		DrawDefaultInspector();

		// 対象取得
		StageMgr stageMgr = (StageMgr)target;
		AreaNumberUIMgr areaNumberUIMgr = FindFirstObjectByType<AreaNumberUIMgr>();

		// Nullチェック
		if (stageMgr.m_CardAreaField == null || stageMgr.cardAreaPrefab == null)
		{
			EditorGUILayout.HelpBox("m_CardAreaField または cardAreaPrefab が未設定です。", MessageType.Warning);
			return;
		}

		if (areaNumberUIMgr == null)
		{
			EditorGUILayout.HelpBox("AreaNumberUIMgr が見つかりません。", MessageType.Warning);
			return;
		}

		if (GUILayout.Button("カードエリア生成"))
		{
			stageMgr.DeleteCardAreaList();	// カードエリアリストを削除
			stageMgr.CreateCardArea();      // 実際の生成処理をここで呼ぶ
			areaNumberUIMgr.GenerateUIFromEditor();  // カードエリア番号UIを生成
		}
		if (GUILayout.Button("カードエリア削除"))
		{
			stageMgr.DeleteCardAreaList();  // カードエリアリストを削除
			areaNumberUIMgr.ClearUI();      // UIをクリア
		}

		// 計算
		// フィールドの実際のサイズ
		BoxCollider fieldCollider = stageMgr.m_CardAreaField;
		Vector3 fieldScale = fieldCollider.transform.lossyScale;
		float fieldWidth = fieldCollider.size.x * Mathf.Abs(fieldScale.x);
		float fieldDepth = fieldCollider.size.z * Mathf.Abs(fieldScale.z);

		// カードエリアの実際のサイズ
		BoxCollider cardCollider = stageMgr.cardAreaPrefab.GetComponent<BoxCollider>();
		float cardWidth = cardCollider.size.x * Mathf.Abs(stageMgr.cardAreaPrefab.transform.localScale.x);
		float cardDepth = cardCollider.size.z * Mathf.Abs(stageMgr.cardAreaPrefab.transform.localScale.z);

		// 必要な全体サイズ
		float totalWidth = (cardWidth * stageMgr.widthNum) + (stageMgr.interval_W * (stageMgr.widthNum - 1));
		float totalDepth = (cardDepth * stageMgr.heightNum) + (stageMgr.interval_H * (stageMgr.heightNum - 1));
		bool isFit = totalWidth <= fieldWidth && totalDepth <= fieldDepth;

		// 結果表示
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("=== カードエリア配置計算 ===", EditorStyles.boldLabel);
		EditorGUILayout.LabelField($"必要な幅: {totalWidth:F2} / フィールド幅: {fieldWidth:F2}");
		EditorGUILayout.LabelField($"必要な奥行: {totalDepth:F2} / フィールド奥行: {fieldDepth:F2}");
		EditorGUILayout.LabelField($"フィールドに収まるか: {(isFit ? "OK" : "NG")}", isFit ? EditorStyles.label : EditorStyles.boldLabel);
	}
}