using battleTypes;
using System.Collections.Generic;
using UnityEngine;

public class CardCondition : MonoBehaviour
{
	// 側
	[SerializeField]
	private Side m_CondSide;
	// 条件カードリスト
	[SerializeField]
	private List<BattleCard> m_CondCardList;

	// カード条件設定
	public void SetCondCardList(Side _side, List<BattleCard> _cardList)
	{
		// 初期化
		m_CondCardList.Clear();

		// 設定
		m_CondSide = _side;

		// 条件に合ったカードリストを作成
		foreach (var card in _cardList)
		{
			// 側
			if (card.GetSetSide != m_CondSide)
			{
				continue;
			}

			m_CondCardList.Add(card);
		}
	}
	public void AddCard(BattleCard _card)
	{
		//　既に存在しているならはじく
		if (m_CondCardList.Contains(_card)) { return; }

		// 条件一致チェック
		// 側
		if (_card.GetSetSide != m_CondSide) { return; }

		m_CondCardList.Add(_card);
	}

	// 条件一致するカードリストを返す
	public List<BattleCard> GetCardList
	{
		get { return m_CondCardList; }
	}

	// 条件側
	public Side GetSetSide
	{
		get { return m_CondSide; }
		set { m_CondSide = value; }
	}
}
