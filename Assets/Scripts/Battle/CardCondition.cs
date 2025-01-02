using battleTypes;
using System.Collections.Generic;
using UnityEngine;

public class CardCondition : MonoBehaviour
{
	[SerializeField]
	private Side m_CondSide;
	// カードリスト
	[SerializeField]
	private List<BattleCard> m_CardList;

	[SerializeField]
	private List<BattleCard> m_CondCardList;

	// カード条件
	public void SetCardCondition(Side side, List<BattleCard> cardList)
	{
		m_CondSide = side;
		m_CondCardList = cardList;
	}

	// 条件側
	public Side GetSetSide
	{
		get { return m_CondSide; }
		set { m_CondSide = value; }
	}
}
