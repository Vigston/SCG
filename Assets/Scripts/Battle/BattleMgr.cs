using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using battleTypes;
using TMPro;
using Cysharp.Threading.Tasks;

public class BattleMgr : MonoBehaviour
{
    // =================�N���X================

    // =================�\����================

    // =================�ϐ�================
    // �C���X�^���X
    public static BattleMgr instance;
    // ���݂̃^�[����
    [SerializeField]
    private Side m_TurnSide;
    // ���݂̃^�[����
    [SerializeField]
    private int m_TurnNum = 0;
    // ���݂̃t�F�C�Y
    [SerializeField]
    private PhaseType m_Phase;
    private PhaseType m_NextPhase;

    public TextMeshProUGUI m_TextTurnNum;
    public TextMeshProUGUI m_TextTurnSide;
    public TextMeshProUGUI m_TextPhase;

    // �X�V�t���O
    private bool m_UpdateFlag = false;
    // �t�F�C�Y�i�s�t���O
    private bool m_NextPhaseFlag = false;
    // �^�[���G���h�t���O
    private bool m_TurnEndFlag = false;

    // �t�F�C�Y�I�u�W�F�N�g
    public GameObject m_PhaseObject;

    private void Awake()
    {
        CreateInstance();
    }

    // Start is called before the first frame update
    void Start()
    {
        // �X�^�[�g�t�F�C�Y����n�߂�
        SetPhase(PhaseType.ePhaseType_Start);
        // �t�F�C�Y���[�v����
        PhaseLoop().Forget();
    }

    // Update is called once per frame
    void Update()
    {
        BattleMgrUpdate();

        switch (m_Phase)
        {
            case PhaseType.ePhaseType_Start:
                break;
            case PhaseType.ePhaseType_Join:
                break;
            case PhaseType.ePhaseType_Main:
                break;
            case PhaseType.ePhaseType_End:
                break;
            default:
                break;

        }
    }

    // =================�֐�================
    // BattleMgr�̍X�V�B
    void BattleMgrUpdate()
    {
        // �X�V�t���O�������Ă��Ȃ��Ȃ珈�����Ȃ��B
        if (!m_UpdateFlag) { return; }

        // ���X�V������
        m_TextTurnNum.text = "�^�[�����F" + m_TurnNum.ToString();
        m_TextTurnSide.text = "�^�[�����F" + m_TurnSide.ToString();
        m_TextPhase.text = "�t�F�C�Y�F" + m_Phase.ToString();

        // �X�V�������I������̂Ńt���O�~�낷�B
        if (m_UpdateFlag) { m_UpdateFlag = false; }
    }

    // �X�V�̃��N�G�X�g�B(����Update�ő���)
    public void UpdateRequest()
    {
        m_UpdateFlag = true;
    }

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
    async UniTask PhaseLoop()
    {
        Debug.Log("PhaseLoop�N��");
        while(true)
        {
            Debug.Log("�t�F�C�Y�X�V�I�I");
            // FightMgr�̍X�V���N�G�X�g
            UpdateRequest();
            // ���̃t�F�C�Y(�����ł͌��݂̃t�F�C�Y����)
            PhaseType nextPhase = GetPhase();
            // ���̃t�F�C�Y�Ɉړ�����t���O������
            m_NextPhaseFlag = false;

            switch(m_Phase)
            {
                case PhaseType.ePhaseType_Start:
                    Debug.Log("�X�^�[�g�t�F�C�Y");
                    nextPhase = await PhaseStart();
                    Debug.Log("���̃t�F�C�Y��");
                    break;
                case PhaseType.ePhaseType_Join:
                    Debug.Log("�W���C���t�F�C�Y");
                    nextPhase = await PhaseJoin();
                    Debug.Log("���̃t�F�C�Y��");
                    break;
                case PhaseType.ePhaseType_Main:
                    Debug.Log("���C���t�F�C�Y");
                    nextPhase = await PhaseMain();
                    Debug.Log("���̃t�F�C�Y��");
                    break;
                case PhaseType.ePhaseType_End:
                    Debug.Log("�G���h�t�F�C�Y");
                    nextPhase = await PhaseEnd();
                    Debug.Log("���̃t�F�C�Y��");
                    break;
                default:
                    Debug.Log("PhaseLoop�ɋL�ڂ���Ă��Ȃ��t�F�C�Y�ɑJ�ڂ��悤�Ƃ��Ă��܂�");
                    break;
            }

            // ���̃t�F�C�Y�����݂Ɠ����Ȃ�͂���
            if(nextPhase == m_Phase) { continue; }

            // ���̃t�F�C�Y��ݒ�
            SetPhase(nextPhase);


        }
    }
    // �X�^�[�g�t�F�C�Y
    async UniTask<PhaseType> PhaseStart()
    {
        // �^�[�����J�E���g
        m_TurnNum++;

        // �t�F�C�Y�I�u�W�F�N�g�ݒ�
        m_PhaseObject.AddComponent<StartPhase>();

        await UniTask.WaitUntil(() => IsNextPhaseFlag());

        // �t�F�C�Y�I�u�W�F�N�g�폜
        Destroy(m_PhaseObject.GetComponent<StartPhase>());

        // ���̃t�F�C�Y��
        return GetNextPhase();
    }
    // �W���C���t�F�C�Y
    async UniTask<PhaseType> PhaseJoin()
    {
        // �t�F�C�Y�I�u�W�F�N�g�ݒ�
        m_PhaseObject.AddComponent<JoinPhase>();

        await UniTask.WaitUntil(() => IsNextPhaseFlag());

        // �t�F�C�Y�I�u�W�F�N�g�폜
        Destroy(m_PhaseObject.GetComponent<JoinPhase>());
        // ���̃t�F�C�Y��
        return GetNextPhase();
    }
    // ���C���t�F�C�Y
    async UniTask<PhaseType> PhaseMain()
    {
        // �t�F�C�Y�I�u�W�F�N�g�ݒ�
        m_PhaseObject.AddComponent<MainPhase>();

        await UniTask.WaitUntil(() => IsNextPhaseFlag());

        // �t�F�C�Y�I�u�W�F�N�g�폜
        Destroy(m_PhaseObject.GetComponent<MainPhase>());
        // ���̃t�F�C�Y��
        return GetNextPhase();
    }
    // �G���h�t�F�C�Y
    async UniTask<PhaseType> PhaseEnd()
    {
        // �t�F�C�Y�I�u�W�F�N�g�ݒ�
        m_PhaseObject.AddComponent<EndPhase>();

        await UniTask.WaitUntil(() => IsNextPhaseFlag());

        // �t�F�C�Y�I�u�W�F�N�g�폜
        Destroy(m_PhaseObject.GetComponent<EndPhase>());

        // �^�[���I������
        TurnEnd();

        // ���̃t�F�C�Y��
        return GetNextPhase();
    }

    // �^�[���G���h�t���O�̐ݒ�
    public void SetTurnEndFlag()
    {
        m_TurnEndFlag = true;
    }

    // �^�[���G���h�t���O�������Ă��邩
    public bool IsTurnEndFlag()
    {
        return m_TurnEndFlag;
    }

    // �^�[���I��
    public void TurnEnd()
    {
        Side revSide = Common.GetRevSide(m_TurnSide);

        m_TurnSide = revSide;

        // �^�[���I�����N�G�X�g������
        m_TurnEndFlag = false;
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

    // �^�[�����擾
    public int GetTurnNum()
    {
        return m_TurnNum;
    }

    // -------�t�F�C�Y------
    // �t�F�C�Y�ݒ�B
    public void SetPhase(PhaseType _phase)
    {
        m_Phase = _phase;
    }
    public void SetNextPhase(PhaseType _nextPhase)
    {
        m_NextPhase = _nextPhase;
    }
    // �t�F�C�Y�擾�B
    public PhaseType GetPhase()
    {
        return m_Phase;
    }
    public PhaseType GetNextPhase()
    {
        return m_NextPhase;
    }
    // �w��̃t�F�C�Y���B
    public bool IsPhase(PhaseType _phase)
    {
        return m_Phase == _phase;
    }
    // ���̃t�F�C�Y�ɐi�ރt���O�𗧂Ă�
    public void SetNextPhaseFlag()
    {
        m_NextPhaseFlag = true;
    }
    // ���̃t�F�C�Y�ɐi�ރt���O�������Ă��邩
    public bool IsNextPhaseFlag()
    {
        return m_NextPhaseFlag;
    }
    // ���̃t�F�C�Y�ƃt���O��ݒ�
    public void SetNextPhaseAndFlag(PhaseType _nextPhase)
    {
        SetNextPhase(_nextPhase);
        SetNextPhaseFlag();
    }
}
