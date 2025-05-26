using System.Collections.Generic;
using UnityEngine;
using battleTypes;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class Test_StageMgr : MonoBehaviour
{
    // =================変数=================
    public static Test_StageMgr instance;

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
    private List<Test_CardArea> m_CardAreaList = new List<Test_CardArea>();



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
		// 基準座標
		Vector3 baseVec = new Vector3();

		BoxCollider cardCollider = cardAreaPrefab.GetComponent<BoxCollider>();

		// 左上の手前
		Vector3 vecCardLeftTopUp = Common.GetBoxColliderVertices(cardCollider)[0];
		// 左上の奥
		Vector3 vecCardLeftTopDown = Common.GetBoxColliderVertices(cardCollider)[3];
		// 左下の手前
		Vector3 vecCardLeftBottomUp = Common.GetBoxColliderVertices(cardCollider)[6];
		// 右上の手前
		Vector3 vecCardRightTopUp = Common.GetBoxColliderVertices(cardCollider)[1];

		// カード横幅の半分
		float cardHarfSize_x = Vector3.Distance(vecCardLeftTopUp, vecCardRightTopUp) / 2;

		//////////////////////////
		///// 中心座標を計算 /////
		//////////////////////////
		Vector3 vecLeftTopUp = Common.GetBoxColliderVertices(m_CardAreaField)[0];
		Vector3 vecRightBottomUp = Common.GetBoxColliderVertices(m_CardAreaField)[7];
		Vector3 playerAreaCenter = m_CardAreaField.bounds.center;

		baseVec = playerAreaCenter;
		// 横列数に応じてZ軸をカードエリアフィールドの中心に合わせる。
		baseVec.z += (Vector3.Distance(vecCardLeftTopUp, vecCardLeftBottomUp) * (heightNum - 1)) / 2;
		// 横列間隔の分だけ調整
		baseVec.z += (interval_H * (heightNum - 1)) / 2;

		// 縦列数に応じてX軸をカードエリアフィールドの中心に合わせる。
		baseVec.x -= (Vector3.Distance(vecCardLeftTopUp, vecCardRightTopUp) * (widthNum - 1)) / 2;
		// 縦列間隔の分だけ調整
		baseVec.x -= (interval_W * (widthNum - 1)) / 2;

		// カードの厚み
		float CardHeightSize = Vector3.Distance(vecCardLeftTopUp, vecCardLeftTopDown) / 2;

		// カードエリアを全て生成する
		int index = 0;	// ループカウンタインデックス
		// 縦列の分回す
		for (int j = 0; j < heightNum; j++)
		{
			// 横列の分回す
			for (int k = 0; k < widthNum; k++)
			{
				// エリアの表示位置
				Vector3 areaVec = baseVec;

				areaVec.x += (k * cardAreaPrefab.transform.localScale.x) + (k * interval_W);
				areaVec.y += CardHeightSize;
				areaVec.z -= j * cardAreaPrefab.transform.localScale.z + (j * interval_H);

				// カードエリア生成
				CreateCardArea(areaVec, index);
				index++;	// インクリメント
			}
		}
	}

	// カードエリア生成
	public void CreateCardArea(Vector3 _position, int _Index)
	{
		GameObject cardAreaClone = Instantiate(cardAreaPrefab, _position, Quaternion.identity);

		// カードエリアの親オブジェクトを設定
		cardAreaClone.transform.SetParent(m_CardAreaParentObj.transform);

		Test_CardArea cardArea = cardAreaClone.GetComponent<Test_CardArea>();
		cardArea.GetSetIndex = _Index;
		cardArea.GetSetSide = Side.eSide_None;
		m_CardAreaList.Add(cardArea);
	}


    // 指定カードエリアの削除
    public void RemoveCardArea(int index)
    {
		Test_CardArea cardArea = new Test_CardArea();
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
		foreach (Test_CardArea cardArea in m_CardAreaList)
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
	public List<Test_CardArea> GetCardAreaListFromSide(Side _Side)
    {
        // 指定側のカードエリアリスト
        List<Test_CardArea> cardAreaList = new List<Test_CardArea>();

        foreach(Test_CardArea cardArea in m_CardAreaList)
        {
            // 側が指定されているものと違うならはじく
            if(cardArea.GetSetSide != _Side) { continue; }

            // 指定された側のエリアを追加
            cardAreaList.Add(cardArea);
        }

        return cardAreaList;
    }

    // カードが入ってないカードエリアリストを取得
    public List<Test_CardArea> GetCardAreaListFromEmptyCard()
    {
        // カードが入ってないカードエリアリスト
        List<Test_CardArea> cardAreaList = new List<Test_CardArea>();

        foreach (Test_CardArea cardArea in m_CardAreaList)
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
        List<Test_CardArea> cardAreaList = new List<Test_CardArea>();

        foreach (Test_CardArea cardArea in m_CardAreaList)
        {
            // カードが入っているならはじく
            if (cardArea.GetCardList.Any()) { continue; }

            // 指定された側のエリアを追加
            cardAreaList.Add(cardArea);
        }

        return cardAreaList.Count;
    }

    // 指定側のカードが入っていないカードリストエリアを取得
    public List<Test_CardArea> GetCardAreaFromSideEmptyCard(Side _Side)
    {
        // カードが入ってないカードエリアリスト
        List<Test_CardArea> cardAreaList = new List<Test_CardArea>();

        foreach (Test_CardArea cardArea in m_CardAreaList)
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
    public Test_CardArea GetCardAreaFromIndex(int _Index)
    {
		Test_CardArea cardArea = m_CardAreaList.Find(x => x.GetSetIndex == _Index);

        return cardArea;
    }

	// カードエリアがカードエリアフィールドに収まるか
	private bool IsCardAreaFitInField()
	{
		float fieldWidth = m_CardAreaField.size.x;
		float fieldDepth = m_CardAreaField.size.z;

		float cardWidth = cardAreaPrefab.transform.localScale.x;
		float cardDepth = cardAreaPrefab.transform.localScale.z;

		float totalWidth = (cardWidth * widthNum) + (interval_W * (widthNum - 1));
		float totalDepth = (cardDepth * heightNum) + (interval_H * (heightNum - 1));

		return totalWidth <= fieldWidth && totalDepth <= fieldDepth;
	}
}
