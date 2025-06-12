using UnityEngine;

[ExecuteInEditMode]
public class TerrainTexturePainter : MonoBehaviour
{
	public Terrain terrain;
	public int targetTextureIndex = 1;

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
		int layerCount = terrainData.alphamapLayers;

		// Terrain全体を0番テクスチャに初期化
		float[,,] alphas = new float[mapHeight, mapWidth, layerCount];
		for (int y = 0; y < mapHeight; y++)
		{
			for (int x = 0; x < mapWidth; x++)
			{
				for (int i = 0; i < layerCount; i++)
				{
					alphas[y, x, i] = (i == 0) ? 1f : 0f;
				}
			}
		}

		// BoxCollider内だけ targetTextureIndex に上書き
		for (int y = 0; y < mapHeight; y++)
		{
			for (int x = 0; x < mapWidth; x++)
			{
				float worldX = terrainPosition.x + ((float)x / mapWidth) * terrainData.size.x;
				float worldZ = terrainPosition.z + ((float)y / mapHeight) * terrainData.size.z;
				Vector3 worldPos = new Vector3(worldX, 0, worldZ);

				if (box.bounds.Contains(worldPos))
				{
					for (int i = 0; i < layerCount; i++)
					{
						alphas[y, x, i] = (i == targetTextureIndex) ? 1f : 0f;
					}
				}
			}
		}

		terrainData.SetAlphamaps(0, 0, alphas);
		Debug.Log("Terrain全体を初期化してから、BoxCollider内にテクスチャを塗りました！");
	}
}