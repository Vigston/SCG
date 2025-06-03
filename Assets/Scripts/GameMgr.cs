using UnityEngine;

public class GameMgr : MonoBehaviour
{
	// インスタンス
	public static GameMgr instance;

	// 現在のターン数
	[SerializeField]
	private int m_TurmCnt;

	private void Awake()
	{
		// インスタンス生成
		CreateInstance();
	}

	// インスタンスを作成
	public bool CreateInstance()
	{
		// 既にインスタンスが作成されていなければ作成する
		if (!instance)
		{
			// 作成
			instance = this;
		}

		// インスタンスが作成済みなら終了
		if (instance) { return true; }

		Debug.LogError($"{this}のインスタンスが生成できませんでした");
		return false;
	}

	public int GetSetTurnCnt
	{
		get { return m_TurmCnt; }
		set { m_TurmCnt = value; }
	}
}
