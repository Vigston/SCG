using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using battleTypes;
using System.Linq;

public class CardArea : MonoBehaviour
{
	///////////変数///////////////
	[SerializeField, ReadOnly]
	private int m_Index;
	[SerializeField, ReadOnly]
	private Side m_Side;
	[SerializeField, ReadOnly]
	private List<Card> m_CardList;

	/// == GetSetプロパティ == ///
	public int GetSetIndex
	{
		get { return m_Index; }
		set { m_Index = value; }
	}
	public Side GetSetSide
	{
		get { return m_Side; }
		set { m_Side = value; }
	}
	public List<Card> GetCardList
	{
		get { return m_CardList; }
	}
}
