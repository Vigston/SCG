using UnityEditor;
using UnityEngine;

public static class TerrainDataGenerator
{
	[MenuItem("Assets/Create/TerrainData")]
	public static void CreateTerrainData()
	{
		// インスタンスを作成
		TerrainData data = new TerrainData();

		// ファイル名入力ダイアログを表示して保存
		string defaultName = "New TerrainData.asset";
		string path = AssetDatabase.GenerateUniqueAssetPath("Assets/Terrain/" + defaultName);

		// CreateAssetWithSavePrompt風の動き
		ProjectWindowUtil.CreateAsset(data, path);
	}
}