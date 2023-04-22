using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightMgr : MonoBehaviour
{
    // =================�\����================
    public enum eSide
    {
        eSide_Player,
        eSide_Enemy,
        eSide_Max,
    }

    // =================�ϐ�================
    // �C���X�^���X
    public static FightMgr instance;
    // ��
    private eSide m_Side;

    private void Awake()
    {
        CreateInstance();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // =================�֐�================
    // �C���X�^���X���쐬
    public bool CreateInstance()
    {
        // ���ɃC���X�^���X���쐬����Ă��Ȃ���΍쐬����
        if(instance == null)
        {
            // �쐬
            instance = this;
            return true;
        }

        Debug.LogError("FightMgr�̃C���X�^���X�������ł��܂���ł���");
        return false;
    }

    // ����ݒ�
    public void SetSide(eSide _side)
    {
        m_Side = _side;
    }

    // �����擾
    public eSide GetSide()
    {
        return m_Side;
    }
}
