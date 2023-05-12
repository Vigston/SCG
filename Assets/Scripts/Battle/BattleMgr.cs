using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using battleTypes;
using TMPro;

public class BattleMgr : MonoBehaviour
{
    // =================�\����================
    enum PhaseType
    {

    }

    // =================�ϐ�================
    // �C���X�^���X
    public static BattleMgr instance;
    // �^�[����
    [SerializeField]
    private Side m_TurnSide;

    public TextMeshProUGUI text;

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
        text.text = "�^�[���� : " + m_TurnSide.ToString();
    }

    // =================�֐�================
    // �C���X�^���X���쐬
    public bool CreateInstance()
    {
        // ���ɃC���X�^���X���쐬����Ă��Ȃ���΍쐬����
        if (instance == null)
        {
            // �쐬
            instance = this;
        }

        // �C���X�^���X���쐬�ς݂Ȃ�I��
        if (instance != null) { return true; }

        Debug.LogError("BattleMgr�̃C���X�^���X�������ł��܂���ł���");
        return false;
    }
    // --�V�X�e��--
    // �^�[���I��
    public void TurnEnd()
    {
        Side revSide = Common.GetRevSide(m_TurnSide);

        m_TurnSide = revSide;
    }

    // -----��-----
    // �^�[������ݒ�
    public void SetSide(Side _side)
    {
        m_TurnSide = _side;
    }
    // �^�[�������擾
    public Side GetSide()
    {
        return m_TurnSide;
    }
    // �����擾(index����)
    public Side GetSide(int _index)
    {
        // �͈͊O�Ȃ�͂���
        if( _index < 0 || _index > GetSideMax())
        {
            Debug.LogError("�͈͊O��index" + "[" + _index.ToString() + "]" + "�̂���'eSide_None'��Ԃ��܂���" );
            return Side.eSide_None;
        }

        for(int i = 0; i < (int)Side.eSide_Max; i++)
        {
            // �w��index�ȊO�͂͂���
            if(i == _index) { return Side.eSide_None; }

            // �v���C���[
            if(_index == (int)Side.eSide_Player)
            {
                return Side.eSide_Player;
            }
            // �G
            else if (_index == (int)Side.eSide_Enemy)
            {
                return Side.eSide_Enemy;
            }
        }

        return Side.eSide_None;
    }
    // �w�葤��Index���擾
    public int GetSideIndex(Side _side)
    {
        return (int)_side;
    }
    // ���̍ő吔���擾
    public int GetSideMax()
    {
        return (int)Side.eSide_Max;
    }
}
