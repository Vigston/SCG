using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using battleTypes;

public class CardArea : MonoBehaviour
{
    ///////////�ϐ�///////////////
    [SerializeField]
    private ePosition m_Position;
    [SerializeField]
    private List<BattleCard> m_CardList = new List<BattleCard>();

    ///////////�֐�///////////////
    // �ʒu
    public void SetPosiiton(ePosition _position)
    {
        m_Position = _position;
    }
    public ePosition GetPosiiton()
    {
        return m_Position;
    }
    // �J�[�h
    public List<BattleCard> GetCardList()
    {
        return m_CardList;
    }
    public BattleCard GetCard(int _index)
    {
        return m_CardList[_index];
    }
}
