using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using battleTypes;

public class BattleCursor : MonoBehaviour
{
    ////////////////変数///////////////////////
    // 位置
    [SerializeField]
    private Position m_Position;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    ////////////////関数///////////////////////
    ///// 位置
    public void SetPosiiton(Position _posiiton)
    {
        m_Position = _posiiton;
    }
    public Position GetPosition()
    {
        return m_Position;
    }
}
