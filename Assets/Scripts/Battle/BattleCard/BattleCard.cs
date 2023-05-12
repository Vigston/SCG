using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using battleTypes;

public class BattleCard : MonoBehaviour
{
    // ���
    public enum Kind
    {
        eKind_People,
        eKind_Military,
        eKind_Science,
        eKind_Spy,
        eKind_Agent,
        eKind_Max,

        eKind_None = -1,
    }

    ////////////////�ϐ�///////////////////////
    [SerializeField]
    private Position m_Position;
    [SerializeField]
    private Kind m_Kind;
    [SerializeField]
    private bool m_IsEnable = true;
    ////////////////�֐�///////////////////////
    // �ʒu
    public void SetPosiiton(Position _posiiton)
    {
        m_Position = _posiiton;
    }
    public Position GetPosition()
    {
        return m_Position;
    }
    // ���
    public void SetKind(Kind _kind)
    {
        m_Kind = _kind;
    }
    public Kind GetKind()
    {
        return m_Kind;
    }
    // �t���O
    public void SetEnable(bool _enable)
    {
        m_IsEnable = _enable;
    }
    public bool IsEnable()
    {
        return m_IsEnable;
    }
}
