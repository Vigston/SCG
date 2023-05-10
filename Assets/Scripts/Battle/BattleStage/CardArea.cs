using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using battleTypes;

public class CardArea : MonoBehaviour
{
    ///////////変数///////////////
    [SerializeField]
    private ePosition m_Position;
    [SerializeField]
    private List<BattleCard> m_CardList = new List<BattleCard>();

    ///////////関数///////////////
    // 位置
    public void SetPosiiton(ePosition _position)
    {
        m_Position = _position;
    }
    public ePosition GetPosiiton()
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
}
