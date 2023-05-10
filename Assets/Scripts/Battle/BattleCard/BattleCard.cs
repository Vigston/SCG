using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using battleTypes;

public class BattleCard : MonoBehaviour
{
    // í—Ş
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

    ////////////////•Ï”///////////////////////
    [SerializeField]
    private ePosition m_Position;
    [SerializeField]
    private eKind m_Kind;
    [SerializeField]
    private bool m_IsEnable = true;
    ////////////////ŠÖ”///////////////////////
    // ˆÊ’u
    public void SetPosiiton(ePosition _posiiton)
    {
        m_Position = _posiiton;
    }
    public ePosition GetPosiiton()
    {
        return m_Position;
    }
    // í—Ş
    public void SetKind(eKind _kind)
    {
        m_Kind = _kind;
    }
    public eKind GetKind()
    {
        return m_Kind;
    }
    // ƒtƒ‰ƒO
    public void SetEnable(bool _enable)
    {
        m_IsEnable = _enable;
    }
    public bool IsEnable()
    {
        return m_IsEnable;
    }
}
