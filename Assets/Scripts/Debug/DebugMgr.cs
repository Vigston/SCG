using UnityEngine;

public class DebugMgr : MonoBehaviour
{
	// インスタンス
	public static DebugMgr Instance { get; private set; }

	// 通信を行わずにデバッグを行うか
	[SerializeField]
	public bool isSingleDebug;

	private void Awake()
	{
		// インスタンス生成
		if (Instance != null)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;
	}
}
