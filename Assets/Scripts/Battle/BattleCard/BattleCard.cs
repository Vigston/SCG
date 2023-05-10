using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using battleTypes;

public class BattleCard : MonoBehaviour
{
    // ���
    public enum eKind
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
    private ePosition m_Position;
    [SerializeField]
    private eKind m_Kind;
    [SerializeField]
    private bool m_IsEnable = true;
    ////////////////�֐�///////////////////////
    // �ʒu
    public void SetPosiiton(ePosition _posiiton)
    {
        m_Position = _posiiton;
    }
    public ePosition GetPosiiton()
    {
        return m_Position;
    }
    // ���
    public void SetKind(eKind _kind)
    {
        m_Kind = _kind;
    }
    public eKind GetKind()
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
