using UnityEngine;

[ExecuteInEditMode] // エディター上でも動くようにする
public class TerrainTexturePainter : MonoBehaviour
{
	public Terrain terrain;
	public int targetTextureIndex = 1;

	// 公開メソッド（Editorから呼び出せる）
	public void PaintTextureInBox()
	{
		BoxCollider box = GetComponent<BoxCollider>();
		if (terrain == null || box == null)
		{
			Debug.LogWarning("TerrainまたはBoxColliderが見つかりません");
			return;
		}

		TerrainData terrainData = terrain.terrainData;
		Vector3 terrainPosition = terrain.transform.position;
		int mapWidth = terrainData.alphamapWidth;
		int mapHeight = terrainData.alphamapHeight;

		float[,,] alphas = terrainData.GetAlphamaps(0, 0, mapWidth, mapHeight);

		for (int y = 0; y < mapHeight; y++)
		{
			for (int x = 0; x < mapWidth; x++)
			{
				float worldX = terrainPosition.x + ((float)x / mapWidth) * terrainData.size.x;
				float worldZ = terrainPosition.z + ((float)y / mapHeight) * terrainData.size.z;
				Vector3 worldPos = new Vector3(worldX, 0, worldZ);

				if (box.bounds.Contains(worldPos))
				{
					for (int i = 0; i < terrainData.alphamapLayers; i++)
					{
						alphas[y, x, i] = (i == targetTextureIndex) ? 1 : 0;
					}
				}
			}
		}

		terrainData.SetAlphamaps(0, 0, alphas);
		Debug.Log("テクスチャをBoxCollider内に塗りました！");
	}
}