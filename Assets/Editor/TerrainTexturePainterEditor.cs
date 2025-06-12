using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TerrainTexturePainter))]
public class TerrainTexturePainterEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		TerrainTexturePainter painter = (TerrainTexturePainter)target;

		if (GUILayout.Button("範囲内にテクスチャを塗る"))
		{
			painter.PaintTextureInBox();
		}
	}
}