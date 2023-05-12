using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using battleTypes;

public class CardArea : MonoBehaviour
{
    ///////////変数///////////////
    [SerializeField]
    private Position m_Position;
    [SerializeField]
    private List<BattleCard> m_CardList = new List<BattleCard>();

    ///////////関数///////////////
    // 位置
    public void SetPosiiton(Position _position)
    {
        m_Position = _position;
    }
    public Position GetPosition()
    {
        return m_Position;
    }
    // カード
    public List<BattleCard> GetCardList()
    {
        return m_CardList;
    }
    public BattleCard GetCard(int _index)
    {
        return m_CardList[_index];
    }
    public void AddCard(BattleCard _battleCard)
    {
        m_CardList.Add(_battleCard);
    }
    public void CopyCardList(List<BattleCard> _battleCardList)
    {
        m_CardList = new List<BattleCard>(_battleCardList);
    }
    public void RemoveCard(int _index)
    {
        m_CardList.RemoveAt(_index);
    }
    public void RemoveCardList()
    {
        m_CardList.Clear();
    }
}
