#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(User))]
public class UserEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		User user = (User)target;

		if (Application.isPlaying)
		{
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("=== 現在の資源量 ===", EditorStyles.boldLabel);

			var resourceMgr = user.GetResourceMgr();
			var allResources = resourceMgr.GetAllResources();
			foreach (var kv in allResources)
			{
				var resource = kv.Value;
				EditorGUILayout.LabelField($"{resource.m_Type}: {resource.m_Quantity}");
			}
		}
	}
}
#endif