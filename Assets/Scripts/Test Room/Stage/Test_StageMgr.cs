using System.Collections.Generic;
using UnityEngine;
using battleTypes;
using Photon.Pun;

public class Test_StageMgr : MonoBehaviour
{
    // =================変数=================
    public static Test_StageMgr instance;

    // プレイヤーエリア
    public BoxCollider PlayerArea;
    // エネミーエリア
    public BoxCollider EnemyArea;

	// カードエリア親オブジェクト
	[SerializeField]
	private GameObject m_PlayerCardAreaParent;
	[SerializeField]
	private GameObject m_EnemyCardAreaParent;
	// カードエリアオブジェクト
	[SerializeField]
	private GameObject cardAreaPrefab;
	// 縦に並ぶカード枚数
	[SerializeField]
	private int heightNum = 0;
	// 横に並ぶカード枚数
	[SerializeField]
	private int widthNum = 0;
	// カード間隔
	[SerializeField]
	private float interval_H;
	[SerializeField]
	private float interval_W;

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
		// カードエリアの作成
		CreateCardArea();
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
		// 相手と自分の分回す
		for (int i = 0; i < (int)Side.eSide_Max; i++)
		{
			// 基準座標
			Vector3 baseVec = new Vector3();

			// 側
			Side Side = (Side)i;

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
			// プレイヤー側
			if (Side == Side.eSide_Player)
			{
				Vector3 vecLeftTopUp = Common.GetBoxColliderVertices(PlayerArea)[0];
				Vector3 vecRightBottomUp = Common.GetBoxColliderVertices(PlayerArea)[7];
				Vector3 playerAreaCenter = PlayerArea.bounds.center;

				baseVec = playerAreaCenter;
				// 横列数に応じてZ軸をカードエリアの中心に合わせる。
				baseVec.z += (Vector3.Distance(vecCardLeftTopUp, vecCardLeftBottomUp) * (heightNum - 1)) / 2;
			}
			// 敵側
			else if (Side == Side.eSide_Enemy)
			{
				Vector3 vecLeftTopUp = Common.GetBoxColliderVertices(EnemyArea)[0];
				Vector3 vecLeftBottomUp = Common.GetBoxColliderVertices(EnemyArea)[6];
				Vector3 enemyAreaCenter = EnemyArea.bounds.center;

				baseVec = enemyAreaCenter;

				// 横列数に応じてZ軸をカードエリアの中心に合わせる。
				baseVec.z -= (Vector3.Distance(vecCardLeftTopUp, vecCardLeftBottomUp) * (heightNum - 1)) / 2;
			}

			// 縦列のカード数に応じてX軸をカードエリアの中心に合わせる。
			baseVec.x -= cardHarfSize_x * (widthNum - 1);
			baseVec.x -= interval_W * ((widthNum - 1) / 2);

			// 偶数
			if(widthNum % 2 == 0)
			{
				baseVec.x -= cardHarfSize_x / 2;
			}

			// カードの厚み
			float CardHeightSize = Vector3.Distance(vecCardLeftTopUp, vecCardLeftTopDown) / 2;

			// プレイヤー側のエリア作成
			if (Side == Side.eSide_Player)
			{
				// 縦列の分回す
				for (int j = 0; j < heightNum; j++)
				{
					// 横列の分回す
					for (int k = 0; k < widthNum; k++)
					{
						// エリアの表示位置
						Vector3 areaVec = baseVec;
						// エリア位置
						Position areaPos = (Position)((j * widthNum) + k);

						areaVec.x += (k * cardAreaPrefab.transform.localScale.x) + (k * interval_W);
						areaVec.y += CardHeightSize;
						areaVec.z -= j * cardAreaPrefab.transform.localScale.z + (j * interval_H);

						// カードエリア生成
						CreateCardArea(areaVec, Side, areaPos);
					}
				}
			}

			// 敵側のエリア作成
			if (Side == Side.eSide_Enemy)
			{
				// 縦列の分回す
				for (int j = 0; j < heightNum; j++)
				{
					int jrev = heightNum - j - 1;
					// 横列の分回す
					for (int k = 0; k < widthNum; k++)
					{
						int krev = widthNum - k - 1;
						// エリアの表示位置
						Vector3 areaVec = baseVec;
						// エリア位置
						Position areaPos = (Position)(((jrev) * widthNum) + krev);

						areaVec.x += (k * cardAreaPrefab.transform.localScale.x) + (k * interval_W);
						areaVec.y += CardHeightSize;
						areaVec.z += j * cardAreaPrefab.transform.localScale.z + (j * interval_H);

						// カードエリア生成
						CreateCardArea(areaVec, Side, areaPos);
					}
				}
			}
		}
	}

	// カードエリア生成
	public void CreateCardArea(Vector3 _position, Side _side, Position _posIndex)
	{
		GameObject cardAreaClone = Instantiate(cardAreaPrefab, _position, Quaternion.identity);

		// カードエリアの親オブジェクトを設定
		if(GetCardAreaParent(_side))
		{
			cardAreaClone.transform.SetParent(GetCardAreaParent(_side).transform);
		}

		Test_CardArea cardArea = cardAreaClone.GetComponent<Test_CardArea>();
		cardArea.SetSide(_side);
		cardArea.SetPosiiton(_posIndex);
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

    // 全てのカードエリアを削除
    public void AllRemoveCardArea()
    {
        // 全て削除
        m_CardAreaList.Clear();
    }

    // 指定側のカードエリアリストを取得
    public List<Test_CardArea> GetCardAreaListFromSide(Side _Side)
    {
        // 指定側のカードエリアリスト
        List<Test_CardArea> cardAreaList = new List<Test_CardArea>();

        foreach(Test_CardArea cardArea in m_CardAreaList)
        {
            // 側が指定されているものと違うならはじく
            if(cardArea.GetSide() != _Side) { continue; }

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
            if (!cardArea.IsCardEmpty()) { continue; }

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
            if (!cardArea.IsCardEmpty()) { continue; }

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
            if (cardArea.GetSide() != _Side) { continue; }
            // カードが入っているならはじく
            if (!cardArea.IsCardEmpty()) { continue; }

            // 指定された側のエリアを追加
            cardAreaList.Add(cardArea);
        }

        return cardAreaList;
    }

    // 指定位置のカードエリアを取得
    public Test_CardArea GetCardAreaFromPos(Side _Side, Position _pos)
    {
		Test_CardArea cardArea = m_CardAreaList.Find(x => x.GetSide() == _Side && x.GetPosition() == _pos);

        return cardArea;
    }
	////////////////////////
	/// GetSetプロパティ ///
	////////////////////////
	// カードエリア親オブジェクト取得
	private GameObject GetCardAreaParent(Side _side)
	{
		if (_side == Side.eSide_Player)
		{
			return m_PlayerCardAreaParent;
		}
		else if (_side == Side.eSide_Enemy)
		{
			return m_EnemyCardAreaParent;
		}

		// どちらでもない場合はエラー
		Debug.LogError($"{this}のGetCardAreaParentでエラーが発生しました");
		return null;
	}
}
