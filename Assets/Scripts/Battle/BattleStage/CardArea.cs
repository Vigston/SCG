using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using battleTypes;
using System.Linq;

public class CardArea : MonoBehaviour
{
    ///////////変数///////////////
    [SerializeField]
    private Side m_Side;
    [SerializeField]
    private Position m_Position;
    [SerializeField]
    private List<BattleCard> m_CardList;

    ///////////関数///////////////
    // ---システム---
    // 何もカードが入っていないか
    public bool IsCardEmpty()
    {
        return !m_CardList.Any();
    }
    // ---側---
    // 側設定
    public void SetSide(Side _side)
    {
        m_Side = _side;
    }
    // 側取得
    public Side GetSide()
    {
        return m_Side;
    }
    // ---位置---
    // 位置設定
    public void SetPosiiton(Position _position)
    {
        m_Position = _position;
    }
    // 位置を取得
    public Position GetPosition()
    {
        return m_Position;
    }
    // ---カード---
    // カードリスト取得
    public List<BattleCard> GetCardList()
    {
        return m_CardList;
    }
    // カード取得
    public BattleCard GetCard(int _index)
    {
        return m_CardList[_index];
    }
    // カード追加
    public void AddCard(BattleCard _battleCard)
    {
        m_CardList.Add(_battleCard);
    }
    // カードリストのコピー
    public void CopyCardList(List<BattleCard> _battleCardList)
    {
        m_CardList = new List<BattleCard>(_battleCardList);
    }
    // カード削除
    public void RemoveCard(int _index)
    {
        m_CardList.RemoveAt(_index);
    }
    // カードリスト削除
    public void RemoveCardList()
    {
        m_CardList.Clear();
    }
}
