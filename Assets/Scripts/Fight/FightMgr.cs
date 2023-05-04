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

        eSide_None = -1,
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

    // -----��-----
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
    // �����擾(index����)
    public eSide GetSide(int _index)
    {
        // �͈͊O�Ȃ�͂���
        if( _index < 0 || _index > GetSideMax())
        {
            Debug.LogError("�͈͊O��index" + "[" + _index.ToString() + "]" + "�̂���'eSide_None'��Ԃ��܂���" );
            return eSide.eSide_None;
        }

        for(int i = 0; i < (int)eSide.eSide_Max; i++)
        {
            // �w��index�ȊO�͂͂���
            if(i == _index) { return eSide.eSide_None; }

            // �v���C���[
            if(_index == (int)eSide.eSide_Player)
            {
                return eSide.eSide_Player;
            }
            // �G
            else if (_index == (int)eSide.eSide_Enemy)
            {
                return eSide.eSide_Enemy;
            }
        }

        return eSide.eSide_None;
    }
    // �w�葤��Index���擾
    public int GetSideIndex(eSide _side)
    {
        return (int)_side;
    }
    // ���̍ő吔���擾
    public int GetSideMax()
    {
        return (int)eSide.eSide_Max;
    }
}
