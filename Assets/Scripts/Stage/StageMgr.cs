using System.Collections.Generic;
using UnityEngine;
using battleTypes;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class StageMgr : MonoBehaviour
{
    // =================変数=================
    public static StageMgr instance;

    // カードエリアフィールド
    public BoxCollider m_CardAreaField;

	// カードエリア親オブジェクト
	[SerializeField]
	private GameObject m_CardAreaParentObj;
	// カードエリアオブジェクト
	public GameObject cardAreaPrefab;
	// 縦に並ぶカード枚数
	public int heightNum;
	// 横に並ぶカード枚数
	public int widthNum;

	// カード間隔
	public float interval_H;
	public float interval_W;

	[SerializeField]
    private List<CardArea> m_CardAreaList = new List<CardArea>();



	// =================構造体=================

	void Awake()
    {
        // インスタンスを作成
        CreateInstance();
    }

    // Start is called before the first frame update
    void Start()
    {
	}

	// =================関数=================
	// インスタンスを作成
	public bool CreateInstance()
	{
		// 既にインスタンスが作成されていなければ作成する
		if (instance == null)
		{
			// 作成
			instance = this;
		}

		// インスタンスが作成済みなら終了
		if (instance != null) { return true; }

		Debug.LogError($"{this}のインスタンスが生成できませんでした");
		return false;
	}

	// ===エリア===
	// カードエリア作成
	public void CreateCardArea()
	{
		// コライダーサイズ取得
		BoxCollider cardCollider = cardAreaPrefab.GetComponent<BoxCollider>();
		float cardWidth = cardCollider.size.x * Mathf.Abs(cardAreaPrefab.transform.localScale.x);
		float cardDepth = cardCollider.size.z * Mathf.Abs(cardAreaPrefab.transform.localScale.z);

		Vector3 fieldCenter = m_CardAreaField.bounds.center;

		// 配置全体の幅・奥行き（カード中心基準）
		float totalWidth = cardWidth * widthNum + interval_W * (widthNum - 1);
		float totalDepth = cardDepth * heightNum + interval_H * (heightNum - 1);

		int index = 0;
		for (int j = 0; j < heightNum; j++)
		{
			for (int k = 0; k < widthNum; k++)
			{
				float x = fieldCenter.x - totalWidth / 2 + cardWidth / 2 + k * (cardWidth + interval_W);
				float y = fieldCenter.y + cardCollider.size.y * cardAreaPrefab.transform.localScale.y / 2;
				float z = fieldCenter.z + totalDepth / 2 - cardDepth / 2 - j * (cardDepth + interval_H);

				Vector3 areaVec = new Vector3(x, y, z);
				CreateCardArea(areaVec, index);
				index++;
			}
		}
	}

	// カードエリア生成
	public void CreateCardArea(Vector3 _position, int _Index)
	{
		GameObject cardAreaClone = Instantiate(cardAreaPrefab, _position, Quaternion.identity);

		// カードエリアの親オブジェクトを設定
		cardAreaClone.transform.SetParent(m_CardAreaParentObj.transform);

		CardArea cardArea = cardAreaClone.GetComponent<CardArea>();
		cardArea.GetSetIndex = _Index;
		cardArea.GetSetSide = Side.eSide_None;
		m_CardAreaList.Add(cardArea);
	}


    // 指定カードエリアの削除
    public void RemoveCardArea(int index)
    {
		CardArea cardArea = new CardArea();
        // 指定Indexのエリアを取得
        cardArea = m_CardAreaList[index];
        // 削除
        m_CardAreaList.Remove(cardArea);
    }

    // カードエリアリストを削除
    public void DeleteCardAreaList()
    {
#if UNITY_EDITOR
		Undo.RegisterCompleteObjectUndo(this, "Delete CardAreaList");
#endif
		// カードエリアリストの全てのカードエリアを削除
		foreach (CardArea cardArea in m_CardAreaList)
		{
#if UNITY_EDITOR
			// エディター上で即時削除
			Object.DestroyImmediate(cardArea.gameObject);
#else
			// プレイ中は通常削除
			Destroy(cardArea.gameObject);
#endif
		}
		// 全て削除
		m_CardAreaList.Clear();

#if UNITY_EDITOR
		EditorUtility.SetDirty(this); // オブジェクトの変更をUnityに通知
#endif
	}

	// 指定側のカードエリアリストを取得
	public List<CardArea> GetCardAreaListFromSide(Side _Side)
    {
        // 指定側のカードエリアリスト
        List<CardArea> cardAreaList = new List<CardArea>();

        foreach(CardArea cardArea in m_CardAreaList)
        {
            // 側が指定されているものと違うならはじく
            if(cardArea.GetSetSide != _Side) { continue; }

            // 指定された側のエリアを追加
            cardAreaList.Add(cardArea);
        }

        return cardAreaList;
    }

    // カードが入ってないカードエリアリストを取得
    public List<CardArea> GetCardAreaListFromEmptyCard()
    {
        // カードが入ってないカードエリアリスト
        List<CardArea> cardAreaList = new List<CardArea>();

        foreach (CardArea cardArea in m_CardAreaList)
        {
            // カードが入っているならはじく
            if (cardArea.GetCardList.Any()) { continue; }

            // 指定された側のエリアを追加
            cardAreaList.Add(cardArea);
        }

        return cardAreaList;
    }

    // カードが入ってないカードエリアリスト数を取得
    public int GetCardAreaNumFromEmptyCard()
    {
        // カードが入ってないカードエリアリスト
        List<CardArea> cardAreaList = new List<CardArea>();

        foreach (CardArea cardArea in m_CardAreaList)
        {
            // カードが入っているならはじく
            if (cardArea.GetCardList.Any()) { continue; }

            // 指定された側のエリアを追加
            cardAreaList.Add(cardArea);
        }

        return cardAreaList.Count;
    }

    // 指定側のカードが入っていないカードリストエリアを取得
    public List<CardArea> GetCardAreaFromSideEmptyCard(Side _Side)
    {
        // カードが入ってないカードエリアリスト
        List<CardArea> cardAreaList = new List<CardArea>();

        foreach (CardArea cardArea in m_CardAreaList)
        {
            // 側が指定されているものと違うならはじく
            if (cardArea.GetSetSide != _Side) { continue; }
            // カードが入っているならはじく
            if (cardArea.GetCardList.Any()) { continue; }

            // 指定された側のエリアを追加
            cardAreaList.Add(cardArea);
        }

        return cardAreaList;
    }

    // 指定番号のカードエリアを取得
    public CardArea GetCardAreaFromIndex(int _Index)
    {
		CardArea cardArea = m_CardAreaList.Find(x => x.GetSetIndex == _Index);

        return cardArea;
    }
}
