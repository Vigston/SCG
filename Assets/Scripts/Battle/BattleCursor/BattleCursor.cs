using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using battleTypes;

public class BattleCursor : MonoBehaviour
{
    ////////////////�ϐ�///////////////////////
    // �ʒu
    [SerializeField]
    private ePosition m_Position;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    ////////////////�֐�///////////////////////
    ///// �ʒu
    public void SetPosiiton(ePosition _posiiton)
    {
        m_Position = _posiiton;
    }
    public ePosition GetPosiiton()
    {
        return m_Position;
    }
}
